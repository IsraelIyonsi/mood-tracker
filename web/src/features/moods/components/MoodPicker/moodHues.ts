import type { Mood } from '../../types/mood';

export const MOOD_HUE_700: Record<Mood, string> = {
  happy: '#B45309',
  excited: '#BE123C',
  neutral: '#475569',
  anxious: '#6D28D9',
  sad: '#1D4ED8',
};

export const MOOD_HUE_500: Record<Mood, string> = {
  happy: '#F59E0B',
  excited: '#F43F5E',
  neutral: '#64748B',
  anxious: '#8B5CF6',
  sad: '#3B82F6',
};

const MOOD_LABEL: Record<Mood, string> = {
  happy: 'Happy',
  excited: 'Excited',
  neutral: 'Neutral',
  anxious: 'Anxious',
  sad: 'Sad',
};

export function labelFor(mood: Mood): string {
  return MOOD_LABEL[mood];
}
