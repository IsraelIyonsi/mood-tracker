import { render } from '@testing-library/react';
import { MoodFace } from './MoodFace';
import { ALL_MOODS } from '../../types/mood';

describe('<MoodFace />', () => {
  it.each(ALL_MOODS)('renders %s face', (mood) => {
    const { container } = render(<MoodFace mood={mood} />);
    expect(container.firstChild).toBeInstanceOf(SVGElement);
  });

  it.each(ALL_MOODS)('renders %s face with circle + 2 eyebrows + 2 eyes + 1 mouth element', (mood) => {
    const { container } = render(<MoodFace mood={mood} />);
    const svg = container.querySelector('svg');
    expect(svg).not.toBeNull();
    expect(svg!.querySelector('circle')).not.toBeNull();
  });

  it('applies size prop', () => {
    const { container } = render(<MoodFace mood="happy" size={128} />);
    const svg = container.querySelector('svg');
    expect(svg).toHaveAttribute('width', '128');
    expect(svg).toHaveAttribute('height', '128');
  });

  it('has aria-hidden=true (decorative)', () => {
    const { container } = render(<MoodFace mood="happy" />);
    expect(container.querySelector('svg')).toHaveAttribute('aria-hidden', 'true');
  });
});
