import { createContext } from 'react';
import { lightTheme, Theme } from './colors';

type ThemeType = 'light' | 'dark';

export interface ThemeContextType {
  theme: ThemeType;
  colors: Theme;
  toggleTheme: () => void;
}

const defaultValue: ThemeContextType = {
  theme: 'light',
  colors: lightTheme,
  toggleTheme: () => {},
};

export const ThemeContext = createContext<ThemeContextType>(defaultValue);

