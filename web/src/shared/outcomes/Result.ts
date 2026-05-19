type Handlers<T, E, R> = {
  ok: (value: T) => R;
  err: (error: E) => R;
};

export abstract class Result<T, E> {
  abstract readonly isOk: boolean;
  abstract readonly isErr: boolean;

  abstract map<U>(fn: (value: T) => U): Result<U, E>;
  abstract mapErr<F>(fn: (error: E) => F): Result<T, F>;
  abstract flatMap<U>(fn: (value: T) => Result<U, E>): Result<U, E>;
  abstract match<R>(handlers: Handlers<T, E, R>): R;
  abstract unwrapOr(fallback: T): T;

  static ok<T, E = never>(value: T): Result<T, E> {
    return new Ok(value);
  }

  static err<E, T = never>(error: E): Result<T, E> {
    return new Err(error);
  }

  static async tryAsync<T>(fn: () => Promise<T>): Promise<Result<T, unknown>> {
    try {
      return Result.ok(await fn());
    } catch (error) {
      return Result.err(error);
    }
  }
}

class Ok<T, E> extends Result<T, E> {
  readonly isOk = true as const;
  readonly isErr = false as const;

  constructor(private readonly value: T) {
    super();
  }

  map<U>(fn: (value: T) => U): Result<U, E> {
    return new Ok(fn(this.value));
  }

  mapErr<F>(_fn: (error: E) => F): Result<T, F> {
    return new Ok(this.value);
  }

  flatMap<U>(fn: (value: T) => Result<U, E>): Result<U, E> {
    return fn(this.value);
  }

  match<R>(handlers: Handlers<T, E, R>): R {
    return handlers.ok(this.value);
  }

  unwrapOr(_fallback: T): T {
    return this.value;
  }
}

class Err<T, E> extends Result<T, E> {
  readonly isOk = false as const;
  readonly isErr = true as const;

  constructor(private readonly error: E) {
    super();
  }

  map<U>(_fn: (value: T) => U): Result<U, E> {
    return new Err(this.error);
  }

  mapErr<F>(fn: (error: E) => F): Result<T, F> {
    return new Err(fn(this.error));
  }

  flatMap<U>(_fn: (value: T) => Result<U, E>): Result<U, E> {
    return new Err(this.error);
  }

  match<R>(handlers: Handlers<T, E, R>): R {
    return handlers.err(this.error);
  }

  unwrapOr(fallback: T): T {
    return fallback;
  }
}
