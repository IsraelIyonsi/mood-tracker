import { z } from 'zod';
import { MoodSchema } from './mood';
import { MoodLimits } from '../constants';

export const LogMoodRequestSchema = z.object({
  mood: MoodSchema,
  note: z.string().trim().max(MoodLimits.NoteMaxLength).optional(),
  loggedAt: z.string().datetime({ offset: true }).optional(),
});
export type LogMoodRequest = z.infer<typeof LogMoodRequestSchema>;

const NullableNote = z
  .string()
  .nullish()
  .transform((value) => value ?? null);

export const LogMoodResponseSchema = z.object({
  id: z.string().uuid(),
  mood: MoodSchema,
  note: NullableNote,
  loggedAt: z.string().datetime({ offset: true }),
  createdAt: z.string().datetime({ offset: true }),
});
export type LogMoodResponse = z.infer<typeof LogMoodResponseSchema>;

export const MoodEntryViewSchema = z.object({
  id: z.string().uuid(),
  mood: MoodSchema,
  note: NullableNote,
  loggedAt: z.string().datetime({ offset: true }),
});
export type MoodEntryView = z.infer<typeof MoodEntryViewSchema>;

export const GetRecentMoodsResponseSchema = z.object({
  entries: z.array(MoodEntryViewSchema),
  count: z.number().int().nonnegative(),
});
export type GetRecentMoodsResponse = z.infer<typeof GetRecentMoodsResponseSchema>;
