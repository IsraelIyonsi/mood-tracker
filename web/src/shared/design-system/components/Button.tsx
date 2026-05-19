import { forwardRef, type ButtonHTMLAttributes } from 'react';
import { cva, type VariantProps } from 'class-variance-authority';
import { cn } from '../utils/cn';

const buttonVariants = cva(
  [
    'inline-flex items-center justify-center gap-sm',
    'font-display font-bold uppercase tracking-wide',
    'border-thick border-ink',
    'transition-all duration-fast ease-snap',
    'focus-visible:outline-none',
    'disabled:cursor-not-allowed disabled:opacity-60',
  ],
  {
    variants: {
      variant: {
        slab: [
          'bg-ink text-cream',
          'enabled:hover:-translate-x-[3px] enabled:hover:-translate-y-[3px] enabled:hover:shadow-slab-lg',
          'enabled:active:translate-x-0 enabled:active:translate-y-0 enabled:active:shadow-none',
        ],
        mood: [
          'bg-accent text-cream',
          'enabled:hover:-translate-x-[3px] enabled:hover:-translate-y-[3px] enabled:hover:shadow-slab-lg',
          'enabled:active:translate-x-0 enabled:active:translate-y-0 enabled:active:shadow-none',
        ],
      },
      size: {
        md: 'px-md py-sm text-sm',
        lg: 'px-lg py-md text-base',
      },
    },
    defaultVariants: {
      variant: 'slab',
      size: 'lg',
    },
  }
);

export type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & VariantProps<typeof buttonVariants>;

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(function Button(
  { className, variant, size, type = 'button', children, ...rest },
  ref
) {
  return (
    <button ref={ref} type={type} className={cn(buttonVariants({ variant, size }), className)} {...rest}>
      {children}
    </button>
  );
});
