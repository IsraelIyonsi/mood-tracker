import { Component, type ErrorInfo, type PropsWithChildren, type ReactNode } from 'react';

type Props = PropsWithChildren<{ fallback?: ReactNode }>;

type State = { hasError: boolean; error: Error | null };

export class ErrorBoundary extends Component<Props, State> {
  override state: State = { hasError: false, error: null };

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  override componentDidCatch(error: Error, info: ErrorInfo): void {
    console.error('Unhandled render error', error, info);
  }

  override render() {
    if (this.state.hasError) {
      return (
        this.props.fallback ?? (
          <div className="border-thick border-ink bg-card p-lg" role="alert">
            <h2 className="font-display text-2xl uppercase tracking-wide mb-sm">
              Something broke.
            </h2>
            <p className="text-ink-soft">{this.state.error?.message}</p>
          </div>
        )
      );
    }
    return this.props.children;
  }
}
