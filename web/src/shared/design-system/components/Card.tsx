import type { HTMLAttributes, PropsWithChildren } from 'react';
import { cn } from '../utils/cn';

type CardProps = HTMLAttributes<HTMLElement> & {
  as?: 'article' | 'div' | 'section';
};

export function Card({ as: Tag = 'article', className, children, ...rest }: CardProps) {
  return (
    <Tag
      className={cn('bg-card border-thick border-ink p-md flex flex-col gap-sm', className)}
      {...rest}
    >
      {children}
    </Tag>
  );
}

function CardHeader({ children, className }: PropsWithChildren<{ className?: string }>) {
  return (
    <div
      className={cn(
        'flex justify-between items-baseline font-mono text-[10px] uppercase tracking-wide font-bold opacity-85',
        className
      )}
    >
      {children}
    </div>
  );
}

function CardBody({ children, className }: PropsWithChildren<{ className?: string }>) {
  return <div className={cn('flex flex-col gap-sm', className)}>{children}</div>;
}

function CardFooter({ children, className }: PropsWithChildren<{ className?: string }>) {
  return <div className={cn('font-mono text-[10px] opacity-75', className)}>{children}</div>;
}

Card.Header = CardHeader;
Card.Body = CardBody;
Card.Footer = CardFooter;
