import { useCallback, useEffect, useRef } from 'react';
import { Key } from '@/shared/constants/keyboard';
import { ALL_MOODS, type Mood } from '../../types/mood';
import { MOOD_HUE_700 } from './moodHues';

const ACCENT_PROPERTY = '--accent-mood';

export function useMoodPicker(value: Mood | null, onChange: (next: Mood) => void) {
  const refs = useRef<Array<HTMLButtonElement | null>>([]);

  const indexFor = (mood: Mood) => ALL_MOODS.indexOf(mood);

  useEffect(() => {
    if (value === null) {
      document.body.style.removeProperty(ACCENT_PROPERTY);
      return;
    }
    document.body.style.setProperty(ACCENT_PROPERTY, MOOD_HUE_700[value]);
  }, [value]);

  const setRef = useCallback((index: number) => (element: HTMLButtonElement | null) => {
    refs.current[index] = element;
  }, []);

  const select = useCallback(
    (mood: Mood, focusOption: boolean) => {
      onChange(mood);
      if (focusOption) {
        const target = refs.current[indexFor(mood)];
        target?.focus();
      }
    },
    [onChange]
  );

  const onKeyDown = useCallback(
    (event: React.KeyboardEvent<HTMLDivElement>) => {
      const current = value === null ? 0 : indexFor(value);
      let next = current;
      switch (event.key) {
        case Key.ArrowRight:
        case Key.ArrowDown:
          next = (current + 1) % ALL_MOODS.length;
          break;
        case Key.ArrowLeft:
        case Key.ArrowUp:
          next = (current - 1 + ALL_MOODS.length) % ALL_MOODS.length;
          break;
        case Key.Home:
          next = 0;
          break;
        case Key.End:
          next = ALL_MOODS.length - 1;
          break;
        default:
          return;
      }
      event.preventDefault();
      const nextMood = ALL_MOODS[next];
      if (nextMood) {
        select(nextMood, true);
      }
    },
    [value, select]
  );

  const tabIndexFor = (mood: Mood): 0 | -1 => {
    if (value === null && indexFor(mood) === 0) return 0;
    return value === mood ? 0 : -1;
  };

  return { setRef, select, onKeyDown, tabIndexFor };
}
