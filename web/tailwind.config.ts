import type { Config } from 'tailwindcss';

const cssVar = (name: string): string => `var(--${name})`;

const config: Config = {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  darkMode: 'media',
  theme: {
    extend: {
      colors: {
        cream: cssVar('bg-cream'),
        card: cssVar('bg-card'),
        ink: {
          DEFAULT: cssVar('ink'),
          soft: cssVar('ink-soft'),
          muted: cssVar('ink-muted'),
        },
        mood: {
          happy: {
            DEFAULT: cssVar('mood-happy-500'),
            text: cssVar('mood-happy-700'),
          },
          excited: {
            DEFAULT: cssVar('mood-excited-500'),
            text: cssVar('mood-excited-700'),
          },
          neutral: {
            DEFAULT: cssVar('mood-neutral-500'),
            text: cssVar('mood-neutral-700'),
          },
          anxious: {
            DEFAULT: cssVar('mood-anxious-500'),
            text: cssVar('mood-anxious-700'),
          },
          sad: {
            DEFAULT: cssVar('mood-sad-500'),
            text: cssVar('mood-sad-700'),
          },
        },
        accent: cssVar('accent-mood'),
      },
      fontFamily: {
        display: ['"Space Grotesk"', 'Helvetica Neue', 'Arial', 'sans-serif'],
        mono: ['"JetBrains Mono"', 'ui-monospace', 'SFMono-Regular', 'Menlo', 'monospace'],
      },
      spacing: {
        xs: '4px',
        sm: '8px',
        md: '16px',
        lg: '32px',
        xl: '64px',
        '2xl': '96px',
      },
      borderRadius: {
        none: '0',
        DEFAULT: '0',
      },
      borderWidth: {
        thick: '2px',
        heavy: '3px',
      },
      transitionDuration: {
        fast: '100ms',
        base: '250ms',
        slow: '500ms',
      },
      transitionTimingFunction: {
        snap: 'cubic-bezier(0.65, 0, 0.35, 1)',
      },
      boxShadow: {
        slab: '4px 4px 0 0 var(--ink)',
        'slab-lg': '6px 6px 0 0 var(--ink)',
        'slab-xl': '8px 8px 0 0 var(--ink)',
      },
    },
  },
  plugins: [],
};

export default config;
