import { cn } from '@/shared/design-system/utils/cn';
import { UI } from '@/shared/constants/ui';

type HeadlineProps = {
  hasSelection: boolean;
};

export function Headline({ hasSelection }: HeadlineProps) {
  return (
    <section className="mb-xl">
      <p className="font-mono text-[11px] uppercase tracking-wide text-ink-muted mb-sm">
        // the question
      </p>
      <h1 className="font-display font-bold text-[clamp(56px,11vw,144px)] leading-[0.92] tracking-tighter lowercase m-0">
        {UI.Labels.HowAreYou}
        <br />
        <span
          className={cn(
            'inline-block relative',
            'transition-colors duration-base ease-snap',
            hasSelection ? 'text-accent' : 'text-ink'
          )}
        >
          {UI.Labels.Really}
          <span
            aria-hidden="true"
            className={cn(
              'absolute left-0 -bottom-2 h-2',
              'bg-accent',
              'transition-[right] duration-slow ease-snap',
              hasSelection ? 'right-0' : 'right-full'
            )}
          />
        </span>
      </h1>
    </section>
  );
}
