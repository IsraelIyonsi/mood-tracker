import { HttpStatus } from '@/shared/constants/http';
import { UI } from '@/shared/constants/ui';
import type { ProblemDetails } from '../ProblemDetails';

export const ApiErrorKind = {
  Validation: 'validation',
  NotFound: 'not-found',
  RateLimited: 'rate-limited',
  Server: 'server',
  Network: 'network',
  Unknown: 'unknown',
} as const;
export type ApiErrorKind = (typeof ApiErrorKind)[keyof typeof ApiErrorKind];

export abstract class ApiError extends Error {
  abstract readonly kind: ApiErrorKind;

  protected constructor(
    message: string,
    public readonly status: number,
    public readonly correlationId: string
  ) {
    super(message);
    this.name = this.constructor.name;
    Object.setPrototypeOf(this, new.target.prototype);
  }

  abstract toUserMessage(): string;

  static async fromResponse(response: Response, correlationId: string): Promise<ApiError> {
    const problem = await ApiError.parseProblem(response);

    switch (response.status) {
      case HttpStatus.BadRequest:
      case HttpStatus.UnprocessableEntity:
        return new ValidationApiError(problem, correlationId);
      case HttpStatus.NotFound:
        return new NotFoundApiError(problem, correlationId);
      case HttpStatus.TooManyRequests:
        return new RateLimitedApiError(problem, correlationId);
      case HttpStatus.InternalServerError:
      case HttpStatus.BadGateway:
      case HttpStatus.ServiceUnavailable:
        return new ServerApiError(problem, correlationId);
      default:
        return new UnknownApiError(problem, response.status, correlationId);
    }
  }

  private static async parseProblem(response: Response): Promise<ProblemDetails> {
    try {
      return (await response.json()) as ProblemDetails;
    } catch {
      return { type: 'about:blank', title: response.statusText, status: response.status };
    }
  }
}

export class ValidationApiError extends ApiError {
  readonly kind = ApiErrorKind.Validation;
  readonly fieldErrors: Readonly<Record<string, ReadonlyArray<string>>>;

  constructor(problem: ProblemDetails, correlationId: string) {
    super(
      problem.detail ?? problem.title ?? UI.Errors.Validation,
      problem.status ?? HttpStatus.UnprocessableEntity,
      correlationId
    );
    this.fieldErrors = problem.errors ?? {};
  }

  toUserMessage(): string {
    return UI.Errors.Validation;
  }

  hasError(field: string): boolean {
    return field in this.fieldErrors;
  }

  errorsFor(field: string): ReadonlyArray<string> {
    return this.fieldErrors[field] ?? [];
  }
}

export class NotFoundApiError extends ApiError {
  readonly kind = ApiErrorKind.NotFound;
  constructor(problem: ProblemDetails, correlationId: string) {
    super(
      problem.detail ?? problem.title ?? UI.Errors.NotFound,
      HttpStatus.NotFound,
      correlationId
    );
  }
  toUserMessage(): string {
    return UI.Errors.NotFound;
  }
}

export class RateLimitedApiError extends ApiError {
  readonly kind = ApiErrorKind.RateLimited;
  constructor(problem: ProblemDetails, correlationId: string) {
    super(
      problem.detail ?? problem.title ?? UI.Errors.TooManyRequests,
      HttpStatus.TooManyRequests,
      correlationId
    );
  }
  toUserMessage(): string {
    return UI.Errors.TooManyRequests;
  }
}

export class ServerApiError extends ApiError {
  readonly kind = ApiErrorKind.Server;
  constructor(problem: ProblemDetails, correlationId: string) {
    super(
      problem.detail ?? problem.title ?? UI.Errors.ServerError,
      problem.status ?? HttpStatus.InternalServerError,
      correlationId
    );
  }
  toUserMessage(): string {
    return `${UI.Errors.ServerError} (ref: ${this.correlationId})`;
  }
}

export class NetworkApiError extends ApiError {
  readonly kind = ApiErrorKind.Network;
  constructor(message: string, correlationId: string) {
    super(message, 0, correlationId);
  }
  toUserMessage(): string {
    return UI.Errors.NetworkOffline;
  }
}

export class UnknownApiError extends ApiError {
  readonly kind = ApiErrorKind.Unknown;
  constructor(problem: ProblemDetails, status: number, correlationId: string) {
    super(problem.detail ?? problem.title ?? UI.Errors.Unknown, status, correlationId);
  }
  toUserMessage(): string {
    return `${UI.Errors.Unknown} (status: ${this.status}, ref: ${this.correlationId})`;
  }
}
