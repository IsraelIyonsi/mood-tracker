export type ProblemDetails<TExtensions = Record<string, unknown>> = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, ReadonlyArray<string>>;
} & TExtensions;
