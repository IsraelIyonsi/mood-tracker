import { forwardRef, type TextareaHTMLAttributes } from 'react';
import { cn } from '../utils/cn';

export type TextareaProps = TextareaHTMLAttributes<HTMLTextAreaElement> & {
  invalid?: boolean;
};

export const Textarea = forwardRef<HTMLTextAreaElement, TextareaProps>(function Textarea(
  { className, invalid, ...rest },
  ref
) {
  return (
    <textarea
      ref={ref}
      aria-invalid={invalid || undefined}
      className={cn(
        'block w-full min-h-[96px] bg-card text-ink',
        'border-thick border-ink p-md',
        'font-display text-base leading-relaxed',
        'resize-y placeholder:text-ink-muted',
        'transition-all duration-fast ease-snap',
        'focus:outline-none focus:shadow-slab',
        invalid && 'border-mood-excited focus:shadow-[4px_4px_0_0_var(--mood-excited-500)]',
        className
      )}
      {...rest}
    />
  );
});
