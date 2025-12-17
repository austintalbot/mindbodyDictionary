import React, { useEffect, useState } from 'react';
import { lightTheme, darkTheme } from '../theme/colors';
import { ThemeContext } from './ThemeContext';

type ThemeType = 'light' | 'dark';

export const ThemeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [theme, setTheme] = useState<ThemeType>('light');

  useEffect(() => {
    // Check localStorage or system preference
    const stored = localStorage.getItem('theme') as ThemeType | null;
    const preferred = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    const initialTheme = stored || preferred;

    setTheme(initialTheme);
    applyTheme(initialTheme);
  }, []);

  const applyTheme = (newTheme: ThemeType) => {
    const root = document.documentElement;
    if (newTheme === 'dark') {
      root.classList.add('dark');
      root.style.backgroundColor = darkTheme.background;
      root.style.color = darkTheme.foreground;
    } else {
      root.classList.remove('dark');
      root.style.backgroundColor = lightTheme.background;
      root.style.color = lightTheme.foreground;
    }
  };

  const toggleTheme = () => {
    const newTheme = theme === 'light' ? 'dark' : 'light';
    setTheme(newTheme);
    localStorage.setItem('theme', newTheme);
    applyTheme(newTheme);
  };

  const colors = theme === 'light' ? lightTheme : darkTheme;

  return (
    <ThemeContext.Provider value={{ theme, colors, toggleTheme }}>
      {children}
    </ThemeContext.Provider>
  );
};
