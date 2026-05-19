export const UI = {
  Labels: {
    HowAreYou: 'how are you,',
    Really: 'really?',
    SelectMood: '[ select one — required ]',
    NoteOptional: '[ note — optional / 280 max ]',
    CommitEntry: 'COMMIT ENTRY',
    Logging: 'LOGGING…',
    SevenDayLedger: '[ ledger / 7-day ]',
    EmptyTimeline: 'No moods logged yet. Pick one above to start.',
  },
  Errors: {
    SubmitFailed: "Couldn't save your mood. Try again.",
    NetworkOffline: 'Network unavailable. Check your connection.',
    Validation: 'Please correct the highlighted fields.',
    Unknown: 'Something went wrong.',
    ServerError: 'Server error.',
    NotFound: 'Not found.',
    TooManyRequests: 'Too many requests. Slow down a moment.',
  },
  Aria: {
    MoodPicker: 'Pick a mood',
    Timeline: 'Last 7 mood entries',
    Submitting: 'Saving your mood',
    LoggedSuccess: 'Mood logged successfully',
  },
} as const;
