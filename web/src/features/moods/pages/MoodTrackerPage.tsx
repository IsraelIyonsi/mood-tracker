import { useState } from 'react';
import { Headline } from '../components/Headline/Headline';
import { LogMoodForm } from '../components/LogMoodForm/LogMoodForm';
import { MoodTimeline } from '../components/MoodTimeline/MoodTimeline';
import { useRecentMoods } from '../data/useMoodRepository';
import type { Mood } from '../types/mood';

export function MoodTrackerPage() {
  const recent = useRecentMoods();
  const [selectedMood, setSelectedMood] = useState<Mood | null>(null);

  return (
    <main className="mx-auto max-w-[1320px] px-lg py-md">
      <header className="flex justify-between items-end border-b-heavy border-ink pb-sm mb-xl font-mono text-[11px] uppercase tracking-wide">
        <span className="font-bold">MOOD.LOG / 2026-05-19 / TUE</span>
        <span className="font-bold">[ 7 / 7 ]</span>
      </header>

      <Headline hasSelection={selectedMood !== null} />

      <div className="grid grid-cols-1 lg:grid-cols-[7fr_1fr_5fr] gap-0 items-start">
        <div className="lg:col-start-1">
          <LogMoodForm mood={selectedMood} onMoodChange={setSelectedMood} />
        </div>
        <div className="hidden lg:block" aria-hidden="true" />
        <aside className="lg:col-start-3 lg:mt-xl">
          <MoodTimeline entries={recent.data?.entries ?? []} isLoading={recent.isLoading} />
        </aside>
      </div>
    </main>
  );
}
