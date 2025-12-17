import React from 'react';
import { useTheme } from '../theme/useTheme';

interface StyledButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'warning';
  size?: 'sm' | 'md' | 'lg';
}

const StyledButton = React.forwardRef<HTMLButtonElement, StyledButtonProps>(
  ({ variant = 'primary', size = 'md', children, ...props }, ref) => {
    const { colors } = useTheme();

    const getButtonStyles = (variant: string, size: string) => {
      const baseStyles = {
        border: 'none',
        borderRadius: '6px',
        cursor: 'pointer',
        fontWeight: '600',
        transition: 'all 0.2s',
        fontFamily: 'inherit',
        outline: 'none',
      };

      const sizeStyles: Record<string, React.CSSProperties> = {
        sm: { padding: '6px 12px', fontSize: '12px' },
        md: { padding: '10px 20px', fontSize: '14px' },
        lg: { padding: '12px 24px', fontSize: '16px' },
      };

      const variantStyles: Record<string, { bg: string; color: string; hover: string }> = {
        primary: { bg: colors.primary, color: '#fff', hover: colors.primaryHover },
        secondary: { bg: colors.neutral, color: colors.foreground, hover: colors.neutralHover },
        danger: { bg: colors.dangerLight, color: colors.danger, hover: colors.danger },
        warning: { bg: colors.warning, color: '#fff', hover: colors.warningHover },
      };

      const variantConfig = variantStyles[variant];

      return {
        base: {
          ...baseStyles,
          ...sizeStyles[size],
          backgroundColor: variantConfig.bg,
          color: variantConfig.color,
        },
        hover: variantConfig.hover,
      };
    };

    const styles = getButtonStyles(variant, size);
    const baseColor = (styles.base as Record<string, string>).backgroundColor;

    return (
      <button
        ref={ref}
        {...props}
        style={styles.base as React.CSSProperties}
        onMouseEnter={(e) => {
          e.currentTarget.style.backgroundColor = styles.hover;
        }}
        onMouseLeave={(e) => {
          e.currentTarget.style.backgroundColor = baseColor;
        }}
      >
        {children}
      </button>
    );
  }
);

StyledButton.displayName = 'StyledButton';

export default StyledButton;
