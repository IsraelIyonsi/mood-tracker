import { HttpHeader, HttpStatus, MediaType } from '@/shared/constants/http';
import { ApiError, NetworkApiError } from './errors/ApiError';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

export type ApiInit = Omit<RequestInit, 'headers'> & {
  headers?: Record<string, string>;
};

export type ApiFetch = <T>(path: string, init?: ApiInit) => Promise<T>;

function generateCorrelationId(): string {
  if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
    return crypto.randomUUID();
  }
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (char) => {
    const r = (Math.random() * 16) | 0;
    const v = char === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

export async function apiFetch<T>(path: string, init: ApiInit = {}): Promise<T> {
  const correlationId = generateCorrelationId();

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
