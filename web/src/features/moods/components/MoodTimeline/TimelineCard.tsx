import { useCallback, useMemo, type CSSProperties, type KeyboardEvent } from 'react';
import { motion, useReducedMotion } from 'framer-motion';
import { cn } from '@/shared/design-system/utils/cn';
import { Key } from '@/shared/constants/keyboard';
import type { MoodEntryView } from '../../types/dto';
import { MoodEntryViewModel } from '../../view-models/MoodEntryViewModel';
import { MoodFace } from '../MoodFace/MoodFace';

const SWELL_TRANSFORM: string[] = [
  'translate(0px, 0px)',
  'translate(-6px, -6px)',
  'translate(2px, 2px)',
  'translate(0px, 0px)',
];

const SWELL_SHADOW: string[] = [
  '0 0 0 0 transparent',
  '12px 12px 0 0 var(--ink)',
  '-2px -2px 0 0 var(--ink)',
  '0 0 0 0 transparent',
];

const SWELL_TRANSITION = { duration: 0.55, ease: [0.65, 0, 0.35, 1] as const };

type TimelineCardProps = {
  entry: MoodEntryView;
  onActivate?: (entry: MoodEntryView) => void;
};

export function TimelineCard({ entry, onActivate }: TimelineCardProps) {
  const vm = useMemo(() => new MoodEntryViewModel(entry), [entry]);
  const reduced = useReducedMotion();

  const handleActivate = useCallback(() => onActivate?.(entry), [entry, onActivate]);

  const handleKeyDown = useCallback(
    (event: KeyboardEvent) => {
      if (event.key === Key.Enter || event.key === Key.Space) {
        event.preventDefault();
        handleActivate();
      }
    },
    [handleActivate]
  );

  const cardStyle: CSSProperties = {
    backgroundColor: vm.accent,
    color: 'var(--bg-cream)',
  };

  return (
    <motion.article
      tabIndex={0}
      role="button"
      aria-label={vm.ariaLabel}
      style={cardStyle}
      onClick={handleActivate}
      onKeyDown={handleKeyDown}
      {...(reduced
        ? {}
        : {
            whileTap: {
              transform: SWELL_TRANSFORM,
              boxShadow: SWELL_SHADOW,
              transition: SWELL_TRANSITION,
            },
          })}
      className={cn(
        'border-thick border-ink p-md w-full',
        'flex gap-md items-start cursor-pointer outline-none',
        'transition-shadow duration-fast ease-snap',
        'hover:shadow-slab focus-visible:shadow-slab'
      )}
    >
      <div
        className="bg-cream border-thick border-ink w-[72px] h-[72px] shrink-0 flex items-center justify-center"
        style={{ color: 'var(--ink)' }}
      >
        <MoodFace mood={vm.mood} size={56} />
      </div>

      <div className="flex flex-col gap-xs flex-1 min-w-0">
        <header className="flex justify-between gap-md items-baseline">
          <h3 className="font-display font-bold text-xl uppercase tracking-tight m-0">
            {vm.moodLabel}
          </h3>
          <span className="font-mono text-[10px] uppercase tracking-wide font-bold opacity-85 shrink-0">
            {vm.shortDate} · {vm.relativeDate}
          </span>
        </header>
        {vm.note && (
          <p className="font-display text-sm leading-snug m-0 opacity-95 break-words">{vm.note}</p>
        )}
      </div>
    </motion.article>
  );
}
