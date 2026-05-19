import { HttpHeader, HttpStatus, MediaType } from '@/shared/constants/http';
import { ApiError, NetworkApiError } from './errors/ApiError';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

export type ApiInit = Omit<RequestInit, 'headers'> & {
  headers?: Record<string, string>;
};

export type ApiFetch = <T>(path: string, init?: ApiInit) => Promise<T>;

export async function apiFetch<T>(path: string, init: ApiInit = {}): Promise<T> {
  const correlationId = crypto.randomUUID();

  let response: Response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...init,
      headers: {
        [HttpHeader.ContentType]: MediaType.Json,
        [HttpHeader.Accept]: MediaType.Json,
        [HttpHeader.CorrelationId]: correlationId,
        ...init.headers,
      },
    });
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Network error';
    throw new NetworkApiError(message, correlationId);
  }

  if (!response.ok) {
    throw await ApiError.fromResponse(response, correlationId);
  }

  if (response.status === HttpStatus.NoContent) {
    return undefined as T;
  }

  return (await response.json()) as T;
}
