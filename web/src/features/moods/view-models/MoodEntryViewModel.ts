import type { MoodEntryView } from '../types/dto';
import type { Mood } from '../types/mood';
import { MOOD_HUE_500, MOOD_HUE_700, labelFor } from '../components/MoodPicker/moodHues';

const RELATIVE_FORMAT = new Intl.RelativeTimeFormat('en', { numeric: 'auto' });
const ABSOLUTE_FORMAT = new Intl.DateTimeFormat('en', {
  weekday: 'short',
  day: 'numeric',
  month: 'short',
});

export class MoodEntryViewModel {
  readonly #entry: MoodEntryView;
  readonly #now: Date;

  constructor(entry: MoodEntryView, now: Date = new Date()) {
    this.#entry = entry;
    this.#now = now;
  }

  get id(): string {
    return this.#entry.id;
  }

  get mood(): Mood {
    return this.#entry.mood;
  }

  get note(): string | null {
    return this.#entry.note;
  }

  get loggedAt(): Date {
    return new Date(this.#entry.loggedAt);
  }

  get accent(): string {
    return MOOD_HUE_500[this.#entry.mood];
  }

  get textColor(): string {
    return MOOD_HUE_700[this.#entry.mood];
  }

  get moodLabel(): string {
    return labelFor(this.#entry.mood).toUpperCase();
  }

  get relativeDate(): string {
    const ms = this.loggedAt.getTime() - this.#now.getTime();
    const hours = Math.round(ms / (1000 * 60 * 60));
    if (Math.abs(hours) < 24) return RELATIVE_FORMAT.format(hours, 'hour');
    const days = Math.round(hours / 24);
    return RELATIVE_FORMAT.format(days, 'day');
  }

  get shortDate(): string {
    return ABSOLUTE_FORMAT.format(this.loggedAt).toUpperCase();
  }

  get ariaLabel(): string {
    const base = `${this.moodLabel} mood logged on ${this.shortDate}`;
    return this.#entry.note ? `${base}. Note: ${this.#entry.note}` : base;
  }
}
