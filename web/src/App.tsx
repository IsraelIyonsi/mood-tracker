import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ErrorBoundary } from '@/shared/errors/ErrorBoundary';
import { MoodClientProvider } from '@/features/moods/data/MoodClientContext';
import { MoodTrackerPage } from '@/features/moods/pages/MoodTrackerPage';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

export function App() {
  return (
    <ErrorBoundary>
      <QueryClientProvider client={queryClient}>
        <MoodClientProvider>
          <MoodTrackerPage />
        </MoodClientProvider>
      </QueryClientProvider>
    </ErrorBoundary>
  );
}
