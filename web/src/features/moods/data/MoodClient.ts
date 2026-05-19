import type { ApiFetch } from '@/shared/api/apiFetch';
import { HttpMethod } from '@/shared/constants/http';
import { ApiRoutes, MoodLimits } from '../constants';
import {
  type LogMoodRequest,
  type LogMoodResponse,
  type GetRecentMoodsResponse,
  LogMoodRequestSchema,
  LogMoodResponseSchema,
  GetRecentMoodsResponseSchema,
} from '../types/dto';

export class MoodClient {
  readonly #fetch: ApiFetch;

  constructor(fetch: ApiFetch) {
    this.#fetch = fetch;
  }

  log = async (request: LogMoodRequest): Promise<LogMoodResponse> => {
    const validated = LogMoodRequestSchema.parse(request);
    const raw = await this.#fetch<unknown>(ApiRoutes.Moods, {
      method: HttpMethod.Post,
      body: JSON.stringify(validated),
    });
    return LogMoodResponseSchema.parse(raw);
  };

  getRecent = async (
    take: number = MoodLimits.RecentDefault
  ): Promise<GetRecentMoodsResponse> => {
    const raw = await this.#fetch<unknown>(`${ApiRoutes.Moods}?take=${take}`);
    return GetRecentMoodsResponseSchema.parse(raw);
  };
}
