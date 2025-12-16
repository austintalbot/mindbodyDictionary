// Theme color palette for light and dark modes
export const lightTheme = {
  // Background colors
  background: '#ffffff',
  backgroundSecondary: '#f8f9fa',

  // Text colors
  foreground: '#1a1a1a',
  mutedText: '#6c757d',
  lightText: '#495057',
  placeholder: '#adb5bd',

  // Border colors
  border: '#e9ecef',
  borderLight: '#d0d0d0',

  // Primary brand colors
  primary: '#0066cc',
  primaryLight: '#e3f2fd',
  primaryHover: '#0052a3',
  primaryDark: '#003d99',

  // Success/Positive colors
  success: '#4caf50',
  successLight: '#e8f5e9',
  successHover: '#388e3c',

  // Danger/Negative colors
  danger: '#d32f2f',
  dangerLight: '#ffebee',
  dangerHover: '#b71c1c',

  // Warning colors
  warning: '#ffc107',
  warningLight: '#fff8e1',
  warningHover: '#ffb300',

  // Neutral colors
  neutral: '#f0f0f0',
  neutralHover: '#e0e0e0',

  // Input/Form colors
  inputBackground: '#ffffff',
  inputBorder: '#d0d0d0',
  inputFocus: '#0066cc',
  inputFocusRing: 'rgba(0, 102, 204, 0.1)',

  // Shadow
  shadow: 'rgba(0, 0, 0, 0.1)',
  shadowHeavy: 'rgba(0, 0, 0, 0.2)',
};

export const darkTheme = {
  // Background colors
  background: '#1a1a1a',
  backgroundSecondary: '#2d2d2d',

  // Text colors
  foreground: '#e8e8e8',
  mutedText: '#9ca3af',
  lightText: '#b3bac2',
  placeholder: '#6b7280',

  // Border colors
  border: '#3d3d3d',
  borderLight: '#4a4a4a',

  // Primary brand colors
  primary: '#4a9eff',
  primaryLight: '#1e3a5f',
  primaryHover: '#66b3ff',
  primaryDark: '#2e63a0',

  // Success/Positive colors
  success: '#66bb6a',
  successLight: '#1b5e20',
  successHover: '#7cc576',

  // Danger/Negative colors
  danger: '#ff6b6b',
  dangerLight: '#5e1a1a',
  dangerHover: '#ff8787',

  // Warning colors
  warning: '#ffb74d',
  warningLight: '#4d3500',
  warningHover: '#ffc876',

  // Neutral colors
  neutral: '#3a3a3a',
  neutralHover: '#4a4a4a',

  // Input/Form colors
  inputBackground: '#2d2d2d',
  inputBorder: '#4a4a4a',
  inputFocus: '#4a9eff',
  inputFocusRing: 'rgba(74, 158, 255, 0.15)',

  // Shadow
  shadow: 'rgba(0, 0, 0, 0.3)',
  shadowHeavy: 'rgba(0, 0, 0, 0.5)',
};

export type Theme = typeof lightTheme;
