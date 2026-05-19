import { useCallback, useState, type FormEvent } from 'react';
import { UI } from '@/shared/constants/ui';
import { ApiError, ValidationApiError } from '@/shared/api/errors/ApiError';
import { useLogMood } from '../../data/useMoodRepository';
import { type Mood } from '../../types/mood';
import { validateLogMoodRequest, type FormErrors } from '../../lib/validation';
import { labelFor } from '../MoodPicker/moodHues';

export function useLogMoodForm() {
  const logMood = useLogMood();
  const [mood, setMood] = useState<Mood | null>(null);
  const [note, setNote] = useState('');
  const [errors, setErrors] = useState<FormErrors>({});
  const [feedback, setFeedback] = useState<string | null>(null);

  const reset = useCallback(() => {
    setMood(null);
    setNote('');
    setErrors({});
  }, []);

  const onSubmit = useCallback(
    async (event: FormEvent<HTMLFormElement>) => {
      event.preventDefault();
      setFeedback(null);
      const outcome = validateLogMoodRequest({ mood, note });

      await outcome.match({
        ok: async (request) => {
          try {
            await logMood.mutateAsync(request);
            reset();
            setFeedback(UI.Aria.LoggedSuccess);
          } catch (error) {
            if (error instanceof ValidationApiError) {
              const next: FormErrors = {};
              for (const [field, messages] of Object.entries(error.fieldErrors)) {
                const key = field.toLowerCase() === 'note' ? 'note' : 'mood';
                next[key] = messages[0] ?? UI.Errors.Validation;
              }
              setErrors(next);
            } else if (error instanceof ApiError) {
              setFeedback(error.toUserMessage());
            } else {
              setFeedback(UI.Errors.Unknown);
              throw error;
            }
          }
        },
        err: async (formErrors) => {
          setErrors(formErrors);
        },
      });
    },
    [mood, note, logMood, reset]
  );

  const canSubmit = mood !== null && !logMood.isPending;
  const submitLabel = logMood.isPending
    ? UI.Labels.Logging
    : mood
      ? `COMMIT ${labelFor(mood).toUpperCase()} ENTRY`
      : UI.Labels.CommitEntry;

  return {
    mood,
    setMood,
    note,
    setNote,
    errors,
    feedback,
    onSubmit,
    isSubmitting: logMood.isPending,
    canSubmit,
    submitLabel,
  };
}
