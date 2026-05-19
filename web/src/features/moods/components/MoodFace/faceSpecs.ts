import type { Mood } from '../../types/mood';

export type EyebrowSpec = { path: string };
export type EyeSpec = { shape: 'circle' | 'rect'; cx?: number; cy?: number; r?: number; x?: number; y?: number; width?: number; height?: number };
export type MouthSpec =
  | { type: 'arc'; path: string }
  | { type: 'line'; x1: number; y1: number; x2: number; y2: number }
  | { type: 'ellipse'; cx: number; cy: number; rx: number; ry: number }
  | { type: 'rect'; x: number; y: number; width: number; height: number };

export type FaceSpec = {
  leftBrow: EyebrowSpec;
  rightBrow: EyebrowSpec;
  leftEye: EyeSpec;
  rightEye: EyeSpec;
  mouth: MouthSpec;
};

export const FACE_SPECS: Record<Mood, FaceSpec> = {
  happy: {
    leftBrow: { path: 'M 28,36 L 42,32' },
    rightBrow: { path: 'M 58,32 L 72,36' },
    leftEye: { shape: 'circle', cx: 36, cy: 48, r: 3.5 },
    rightEye: { shape: 'circle', cx: 64, cy: 48, r: 3.5 },
    mouth: { type: 'arc', path: 'M 32,62 Q 50,80 68,62' },
  },
  excited: {
    leftBrow: { path: 'M 26,30 L 42,28' },
    rightBrow: { path: 'M 58,28 L 74,30' },
    leftEye: { shape: 'rect', x: 32, y: 42, width: 8, height: 10 },
    rightEye: { shape: 'rect', x: 60, y: 42, width: 8, height: 10 },
    mouth: { type: 'ellipse', cx: 50, cy: 68, rx: 14, ry: 9 },
  },
  neutral: {
    leftBrow: { path: 'M 28,34 L 42,34' },
    rightBrow: { path: 'M 58,34 L 72,34' },
    leftEye: { shape: 'circle', cx: 36, cy: 48, r: 3.5 },
    rightEye: { shape: 'circle', cx: 64, cy: 48, r: 3.5 },
    mouth: { type: 'line', x1: 34, y1: 68, x2: 66, y2: 68 },
  },
  anxious: {
    leftBrow: { path: 'M 28,38 L 44,34' },
    rightBrow: { path: 'M 56,34 L 72,38' },
    leftEye: { shape: 'rect', x: 33, y: 46, width: 6, height: 8 },
    rightEye: { shape: 'rect', x: 61, y: 46, width: 6, height: 8 },
    mouth: { type: 'rect', x: 46, y: 68, width: 8, height: 6 },
  },
  sad: {
    leftBrow: { path: 'M 28,40 L 42,36' },
    rightBrow: { path: 'M 58,36 L 72,40' },
    leftEye: { shape: 'rect', x: 32, y: 46, width: 8, height: 10 },
    rightEye: { shape: 'rect', x: 60, y: 46, width: 8, height: 10 },
    mouth: { type: 'arc', path: 'M 32,72 Q 50,58 68,72' },
  },
};
