import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { CacheTime } from '@/shared/constants/cache';
import { QueryKeys, MoodLimits } from '../constants';
import { useMoodClient } from './MoodClientContext';

export function useRecentMoods(take: number = MoodLimits.RecentDefault) {
  const client = useMoodClient();
  return useQuery({
    queryKey: QueryKeys.moods.recent(take),
    queryFn: () => client.getRecent(take),
    staleTime: CacheTime.RecentMoodsStale,
  });
}

export function useLogMood() {
  const client = useMoodClient();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: client.log,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QueryKeys.moods.all });
    },
  });
}
