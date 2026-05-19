export const ApiRoutes = {
  Moods: '/api/v1/moods',
} as const;

export const MoodLimits = {
  NoteMaxLength: 280,
  RecentDefault: 7,
  RecentMax: 30,
} as const;

export const QueryKeys = {
  moods: {
    all: ['moods'] as const,
    recent: (take?: number) =>
      ['moods', 'recent', take ?? MoodLimits.RecentDefault] as const,
  },
} as const;
