import { cn } from '@/shared/design-system/utils/cn';
import { UI } from '@/shared/constants/ui';
import { ALL_MOODS, type Mood } from '../../types/mood';
import { MoodFace } from '../MoodFace/MoodFace';
import { labelFor } from './moodHues';
import { useMoodPicker } from './useMoodPicker';

type MoodPickerProps = {
  value: Mood | null;
  onChange: (mood: Mood) => void;
  disabled?: boolean;
};

export function MoodPicker({ value, onChange, disabled }: MoodPickerProps) {
  const { setRef, select, onKeyDown, tabIndexFor } = useMoodPicker(value, onChange);

  return (
    <div
      role="radiogroup"
      aria-label={UI.Aria.MoodPicker}
      aria-required="true"
      onKeyDown={onKeyDown}
      data-has-selection={value !== null}
      className="grid grid-cols-5 gap-sm"
    >
      {ALL_MOODS.map((mood, index) => {
        const isSelected = mood === value;
        return (
          <button
            key={mood}
            ref={setRef(index)}
            type="button"
            role="radio"
            aria-checked={isSelected}
            tabIndex={tabIndexFor(mood)}
            disabled={disabled}
            onClick={() => select(mood, false)}
            className={cn(
              'border-thick border-ink bg-card text-ink',
              'flex flex-col items-center gap-sm p-md',
              'transition-all duration-fast ease-snap',
              'enabled:hover:-translate-x-[2px] enabled:hover:-translate-y-[2px] enabled:hover:shadow-slab',
              'enabled:active:translate-x-0 enabled:active:translate-y-0 enabled:active:shadow-none',
              'disabled:opacity-70 disabled:cursor-not-allowed',
              isSelected && [
                'bg-accent text-cream',
                '-translate-x-[3px] -translate-y-[3px] shadow-slab-lg',
              ],
              value !== null && !isSelected && 'opacity-55'
            )}
          >
            <MoodFace mood={mood} size={64} />
            <span className="font-mono text-[10px] uppercase tracking-wide font-bold">
              {labelFor(mood)}
            </span>
          </button>
        );
      })}
    </div>
  );
}
