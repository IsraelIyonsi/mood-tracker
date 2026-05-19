import type { Variants } from 'framer-motion';

export const Duration = {
  fast: 0.1,
  base: 0.25,
  slow: 0.5,
  bloom: 0.72,
} as const;

export const Ease = {
  snap: [0.65, 0, 0.35, 1],
  swell: [0.16, 0.84, 0.32, 1.2],
} as const;

export const MotionVariants = {
  pageEnter: {
    initial: { opacity: 0, y: 8 },
    animate: { opacity: 1, y: 0, transition: { duration: Duration.slow, ease: Ease.snap } },
  } satisfies Variants,

  moodSelectInvert: {
    rest: { scale: 1 },
    selected: {
      scale: 1.04,
      transition: { duration: Duration.base, ease: Ease.snap },
    },
  } satisfies Variants,

  submitFlash: {
    idle: { scale: 1 },
    success: {
      scale: [1, 1.05, 1],
      transition: { duration: Duration.slow, ease: Ease.swell },
    },
  } satisfies Variants,

  timelineEnter: {
    animate: { transition: { staggerChildren: 0.06, delayChildren: 0.08 } },
  } satisfies Variants,

  timelineItem: {
    initial: { opacity: 0, y: 12 },
    animate: { opacity: 1, y: 0, transition: { duration: Duration.base, ease: Ease.snap } },
  } satisfies Variants,
} as const;

export const ReducedMotionVariants = {
  pageEnter: {
    initial: { opacity: 0 },
    animate: { opacity: 1, transition: { duration: 0.2 } },
  } satisfies Variants,

  moodSelectInvert: {
    rest: { opacity: 0.6 },
    selected: { opacity: 1 },
  } satisfies Variants,

  submitFlash: {
    idle: { opacity: 1 },
    success: { opacity: [1, 0.7, 1], transition: { duration: 0.3 } },
  } satisfies Variants,

  timelineEnter: {
    animate: { transition: { staggerChildren: 0 } },
  } satisfies Variants,

  timelineItem: {
    initial: { opacity: 0 },
    animate: { opacity: 1, transition: { duration: 0.15 } },
  } satisfies Variants,
} as const;
