import { cn } from '@/shared/design-system/utils/cn';
import type { Mood } from '../../types/mood';
import { FACE_SPECS, type FaceSpec, type EyeSpec, type MouthSpec } from './faceSpecs';

type MoodFaceProps = {
  mood: Mood;
  size?: number;
  className?: string;
};

export function MoodFace({ mood, size = 64, className }: MoodFaceProps) {
  const spec = FACE_SPECS[mood];

  return (
    <svg
      viewBox="0 0 100 100"
      width={size}
      height={size}
      className={cn('block', className)}
      aria-hidden="true"
    >
      <circle cx={50} cy={50} r={44} fill="none" stroke="currentColor" strokeWidth={3} />
      <Eyebrow spec={spec.leftBrow} />
      <Eyebrow spec={spec.rightBrow} />
      <Eye spec={spec.leftEye} />
      <Eye spec={spec.rightEye} />
      <Mouth spec={spec.mouth} />
    </svg>
  );
}

function Eyebrow({ spec }: { spec: FaceSpec['leftBrow'] }) {
  return (
    <path
      d={spec.path}
      fill="none"
      stroke="currentColor"
      strokeWidth={3.5}
      strokeLinecap="square"
    />
  );
}

function Eye({ spec }: { spec: EyeSpec }) {
  if (spec.shape === 'circle') {
    return <circle cx={spec.cx} cy={spec.cy} r={spec.r} fill="currentColor" />;
  }
  return <rect x={spec.x} y={spec.y} width={spec.width} height={spec.height} fill="currentColor" />;
}

function Mouth({ spec }: { spec: MouthSpec }) {
  switch (spec.type) {
    case 'arc':
      return (
        <path
          d={spec.path}
          fill="none"
          stroke="currentColor"
          strokeWidth={3.5}
          strokeLinecap="square"
        />
      );
    case 'line':
      return (
        <line
          x1={spec.x1}
          y1={spec.y1}
          x2={spec.x2}
          y2={spec.y2}
          stroke="currentColor"
          strokeWidth={3.5}
          strokeLinecap="square"
        />
      );
    case 'ellipse':
      return (
        <ellipse
          cx={spec.cx}
          cy={spec.cy}
          rx={spec.rx}
          ry={spec.ry}
          fill="none"
          stroke="currentColor"
          strokeWidth={3.5}
        />
      );
    case 'rect':
      return (
        <rect
          x={spec.x}
          y={spec.y}
          width={spec.width}
          height={spec.height}
          fill="none"
          stroke="currentColor"
          strokeWidth={3.5}
        />
      );
  }
}
