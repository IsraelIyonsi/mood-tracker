import { useCallback, useEffect, useState } from 'react';
import { cn } from '@/shared/design-system/utils/cn';
import { UI } from '@/shared/constants/ui';
import type { MoodEntryView } from '../../types/dto';
import { TimelineCard } from './TimelineCard';

const ENTRIES_PER_PAGE = 3;

type MoodTimelineProps = {
  entries: ReadonlyArray<MoodEntryView>;
  isLoading: boolean;
  onCardActivate?: (entry: MoodEntryView) => void;
};

export function MoodTimeline({ entries, isLoading, onCardActivate }: MoodTimelineProps) {
  const [page, setPage] = useState(0);
  const totalPages = Math.max(1, Math.ceil(entries.length / ENTRIES_PER_PAGE));

  useEffect(() => {
    if (page >= totalPages) {
      setPage(0);
    }
  }, [page, totalPages]);

  const visible = entries.slice(page * ENTRIES_PER_PAGE, (page + 1) * ENTRIES_PER_PAGE);

  const goPrev = useCallback(() => setPage((current) => Math.max(0, current - 1)), []);
  const goNext = useCallback(
    () => setPage((current) => Math.min(totalPages - 1, current + 1)),
    [totalPages]
  );

  const hasPagination = totalPages > 1;
  const isFirst = page === 0;
  const isLast = page === totalPages - 1;

  return (
    <section aria-label={UI.Aria.Timeline} className="flex flex-col gap-md">
      <header className="flex justify-between items-end gap-md border-b-heavy border-ink pb-sm">
        <h2 className="font-mono text-[11px] uppercase tracking-wide font-bold m-0">
          {UI.Labels.SevenDayLedger}
        </h2>

        <div className="flex items-center gap-md font-mono text-[11px]">
          <span className="text-ink-muted">N = {entries.length}</span>
          {hasPagination && (
            <>
              <span className="text-ink-soft" aria-live="polite">
                {page + 1} / {totalPages}
              </span>
              <div className="flex gap-xs">
                <PagerButton onClick={goPrev} disabled={isFirst} label="Previous page" symbol="←" />
                <PagerButton onClick={goNext} disabled={isLast} label="Next page" symbol="→" />
              </div>
            </>
          )}
        </div>
      </header>

      {isLoading ? (
        <div className="font-mono text-xs text-ink-muted py-md">Loading…</div>
      ) : entries.length === 0 ? (
        <p className="font-display text-base text-ink-soft py-lg">{UI.Labels.EmptyTimeline}</p>
      ) : (
        <ol className="flex flex-col gap-sm list-none m-0 p-0" aria-label={UI.Aria.Timeline}>
          {visible.map((entry) => (
            <li key={entry.id}>
              <TimelineCard entry={entry} {...(onCardActivate ? { onActivate: onCardActivate } : {})} />
            </li>
          ))}
        </ol>
      )}
    </section>
  );
}

type PagerButtonProps = {
  onClick: () => void;
  disabled: boolean;
  label: string;
  symbol: string;
};

function PagerButton({ onClick, disabled, label, symbol }: PagerButtonProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled}
      aria-label={label}
      className={cn(
        'w-8 h-8 border-thick border-ink bg-card text-ink font-mono text-sm',
        'transition-all duration-fast ease-snap',
        'enabled:hover:-translate-x-[2px] enabled:hover:-translate-y-[2px] enabled:hover:shadow-slab',
        'enabled:active:translate-x-0 enabled:active:translate-y-0 enabled:active:shadow-none',
        'disabled:opacity-40 disabled:cursor-not-allowed'
      )}
    >
      {symbol}
    </button>
  );
}
