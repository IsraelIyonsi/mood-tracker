import { useCallback, useMemo, type CSSProperties, type KeyboardEvent } from 'react';
import { motion, useReducedMotion } from 'framer-motion';
import { cn } from '@/shared/design-system/utils/cn';
import { Key } from '@/shared/constants/keyboard';
import type { MoodEntryView } from '../../types/dto';
import { MoodEntryViewModel } from '../../view-models/MoodEntryViewModel';
import { MoodFace } from '../MoodFace/MoodFace';

const SWELL_TRANSFORM: string[] = [
  'translate(0px, 0px)',
  'translate(-8px, -8px)',
  'translate(2px, 2px)',
  'translate(0px, 0px)',
];

const SWELL_SHADOW: string[] = [
  '0 0 0 0 transparent',
  '14px 14px 0 0 var(--ink)',
  '-2px -2px 0 0 var(--ink)',
  '0 0 0 0 transparent',
];

const SWELL_TRANSITION = { duration: 0.6, ease: [0.65, 0, 0.35, 1] as const };

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
        'border-thick border-ink p-md min-w-[200px] max-w-[200px]',
        'flex flex-col gap-sm cursor-pointer outline-none',
        'transition-shadow duration-fast ease-snap',
        'hover:shadow-slab-lg focus-visible:shadow-slab-lg'
      )}
    >
      <header className="flex justify-between font-mono text-[10px] uppercase tracking-wide font-bold opacity-85">
        <span>{vm.shortDate.split(' ')[0]}</span>
        <span>{vm.relativeDate}</span>
      </header>
      <div
        className="bg-cream border-thick border-ink w-[72px] h-[72px] flex items-center justify-center self-start"
        style={{ color: 'var(--ink)' }}
      >
        <MoodFace mood={vm.mood} size={56} />
      </div>
      <h3 className="font-display font-bold text-xl uppercase tracking-tight m-0" style={{ color: vm.textColor }}>
        {vm.moodLabel}
      </h3>
      {vm.note && (
        <p className="font-display text-sm leading-snug line-clamp-3 opacity-95">{vm.note}</p>
      )}
    </motion.article>
  );
}
