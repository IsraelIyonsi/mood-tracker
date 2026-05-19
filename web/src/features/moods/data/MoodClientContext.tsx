import { createContext, useContext, useMemo, type PropsWithChildren } from 'react';
import { apiFetch } from '@/shared/api/apiFetch';
import { MoodClient } from './MoodClient';

const MoodClientContext = createContext<MoodClient | null>(null);

export function MoodClientProvider({ children }: PropsWithChildren) {
  const client = useMemo(() => new MoodClient(apiFetch), []);
  return <MoodClientContext.Provider value={client}>{children}</MoodClientContext.Provider>;
}

export function useMoodClient(): MoodClient {
  const client = useContext(MoodClientContext);
  if (!client) {
    throw new Error('useMoodClient must be used within MoodClientProvider');
  }
  return client;
}
