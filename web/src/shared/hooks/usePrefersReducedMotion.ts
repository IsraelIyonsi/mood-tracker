import { useEffect, useState } from 'react';

const MEDIA_QUERY = '(prefers-reduced-motion: reduce)';

export function usePrefersReducedMotion(): boolean {
  const [prefersReduced, setPrefersReduced] = useState(() => {
    if (typeof window === 'undefined') return false;
    return window.matchMedia(MEDIA_QUERY).matches;
  });

  useEffect(() => {
    const query = window.matchMedia(MEDIA_QUERY);
    const handler = (event: MediaQueryListEvent) => setPrefersReduced(event.matches);
    query.addEventListener('change', handler);
    return () => query.removeEventListener('change', handler);
  }, []);

  return prefersReduced;
}
