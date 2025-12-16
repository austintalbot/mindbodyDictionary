import React from 'react';
import { useTheme } from '../theme/useTheme';

const ThemeToggle: React.FC = () => {
  const { theme, colors, toggleTheme } = useTheme();

  return (
    <button
      onClick={toggleTheme}
      style={{
        padding: '8px 16px',
        backgroundColor: colors.neutral,
        border: `1px solid ${colors.borderLight}`,
        borderRadius: '6px',
        cursor: 'pointer',
        fontSize: '14px',
        fontWeight: '600',
        color: colors.foreground,
        transition: 'all 0.2s',
        display: 'flex',
        alignItems: 'center',
        gap: '8px',
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.backgroundColor = colors.neutralHover;
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.backgroundColor = colors.neutral;
      }}
    >
      {theme === 'light' ? '🌙 Dark' : '☀️ Light'}
    </button>
  );
};

export default ThemeToggle;
