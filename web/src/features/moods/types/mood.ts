import { z } from 'zod';

export const MoodSchema = z.enum(['happy', 'excited', 'neutral', 'anxious', 'sad']);
export type Mood = z.infer<typeof MoodSchema>;

export const Mood = MoodSchema.enum;

export const ALL_MOODS: ReadonlyArray<Mood> = [
  Mood.happy,
  Mood.excited,
  Mood.neutral,
  Mood.anxious,
  Mood.sad,
] as const;
