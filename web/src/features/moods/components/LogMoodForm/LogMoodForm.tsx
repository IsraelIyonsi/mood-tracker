import { UI } from '@/shared/constants/ui';
import { MoodLimits } from '../../constants';
import { Button } from '@/shared/design-system/components/Button';
import { Textarea } from '@/shared/design-system/components/Textarea';
import { type Mood } from '../../types/mood';
import { MoodPicker } from '../MoodPicker/MoodPicker';
import { useLogMoodForm } from './useLogMoodForm';

type LogMoodFormProps = {
  mood: Mood | null;
  onMoodChange: (mood: Mood | null) => void;
};

export function LogMoodForm({ mood, onMoodChange }: LogMoodFormProps) {
  const form = useLogMoodForm({ mood, onMoodChange });

  return (
    <form onSubmit={form.onSubmit} aria-busy={form.isSubmitting} noValidate className="flex flex-col gap-lg">
      <section>
        <p className="font-mono text-[11px] uppercase tracking-wide text-ink-muted mb-md">
          {UI.Labels.SelectMood}
        </p>
        <MoodPicker value={form.mood} onChange={form.setMood} disabled={form.isSubmitting} />
        {form.errors.mood && (
          <p role="alert" className="font-mono text-xs text-mood-excited-text mt-sm">
            {form.errors.mood}
          </p>
        )}
      </section>

      <section>
        <label htmlFor="mood-note" className="font-mono text-[11px] uppercase tracking-wide text-ink-muted mb-sm block">
          {UI.Labels.NoteOptional}
        </label>
        <Textarea
          id="mood-note"
          value={form.note}
          onChange={(event) => form.setNote(event.target.value)}
          maxLength={MoodLimits.NoteMaxLength}
          disabled={form.isSubmitting}
          invalid={Boolean(form.errors.note)}
          placeholder="write a line if you want"
          aria-describedby={form.errors.note ? 'mood-note-error' : undefined}
        />
        {form.errors.note && (
          <p id="mood-note-error" role="alert" className="font-mono text-xs text-mood-excited-text mt-sm">
            {form.errors.note}
          </p>
        )}
      </section>

      <Button
        type="submit"
        variant={form.mood ? 'mood' : 'slab'}
        disabled={!form.canSubmit}
        aria-disabled={!form.canSubmit}
      >
        <span>{form.submitLabel}</span>
        <span className="font-mono text-xl">→</span>
      </Button>

      {form.feedback && (
        <div role="status" aria-live="polite" className="font-mono text-xs text-ink-soft">
          {form.feedback}
        </div>
      )}
    </form>
  );
}
