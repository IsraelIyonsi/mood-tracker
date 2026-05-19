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
    <main className="mx-auto max-w-[860px] px-lg py-md">
      <header className="flex justify-between items-end border-b-heavy border-ink pb-sm mb-xl font-mono text-[11px] uppercase tracking-wide">
        <span className="font-bold">MOOD.LOG / 2026-05-19 / TUE</span>
        <span className="font-bold">[ 7 / 7 ]</span>
      </header>

      <Headline hasSelection={selectedMood !== null} />

      <section className="mb-xl">
        <LogMoodForm mood={selectedMood} onMoodChange={setSelectedMood} />
      </section>

      <MoodTimeline entries={recent.data?.entries ?? []} isLoading={recent.isLoading} />
    </main>
  );
}
