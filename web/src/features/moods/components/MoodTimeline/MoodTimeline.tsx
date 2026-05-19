import { UI } from '@/shared/constants/ui';
import type { MoodEntryView } from '../../types/dto';
import { TimelineCard } from './TimelineCard';

type MoodTimelineProps = {
  entries: ReadonlyArray<MoodEntryView>;
  isLoading: boolean;
  onCardActivate?: (entry: MoodEntryView) => void;
};

export function MoodTimeline({ entries, isLoading, onCardActivate }: MoodTimelineProps) {
  return (
    <section aria-label={UI.Aria.Timeline} className="flex flex-col gap-md">
      <header className="flex justify-between items-end border-b-heavy border-ink pb-sm">
        <h2 className="font-mono text-[11px] uppercase tracking-wide font-bold m-0">
          {UI.Labels.SevenDayLedger}
        </h2>
        <span className="font-mono text-[11px] text-ink-muted">N = {entries.length}</span>
      </header>

      {isLoading ? (
        <div className="font-mono text-xs text-ink-muted py-md">Loading…</div>
      ) : entries.length === 0 ? (
        <p className="font-display text-base text-ink-soft py-lg">{UI.Labels.EmptyTimeline}</p>
      ) : (
        <ol className="flex flex-col gap-sm list-none m-0 p-0" aria-label={UI.Aria.Timeline}>
          {entries.map((entry) => (
            <li key={entry.id}>
              <TimelineCard entry={entry} {...(onCardActivate ? { onActivate: onCardActivate } : {})} />
            </li>
          ))}
        </ol>
      )}
    </section>
  );
}
