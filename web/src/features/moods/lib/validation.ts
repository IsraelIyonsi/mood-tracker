import { Result } from '@/shared/outcomes/Result';
import { LogMoodRequestSchema, type LogMoodRequest } from '../types/dto';
import type { Mood } from '../types/mood';

export type FormErrors = Partial<Record<'mood' | 'note', string>>;

type FormInput = {
  mood: Mood | null;
  note: string;
};

export function validateLogMoodRequest(input: FormInput): Result<LogMoodRequest, FormErrors> {
  if (input.mood === null) {
    return Result.err({ mood: 'Pick a mood first' });
  }

  const candidate = {
    mood: input.mood,
    note: input.note.trim() ? input.note.trim() : undefined,
  };

  const parsed = LogMoodRequestSchema.safeParse(candidate);
  if (parsed.success) {
    return Result.ok(parsed.data);
  }

  const errors: FormErrors = {};
  for (const issue of parsed.error.issues) {
    const field = issue.path[0] === 'note' ? 'note' : 'mood';
    errors[field] = issue.message;
  }
  return Result.err(errors);
}
