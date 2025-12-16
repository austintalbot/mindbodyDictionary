import React from 'react';

interface StyledButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'warning';
  size?: 'sm' | 'md' | 'lg';
}

const getButtonStyles = (variant: string = 'primary', size: string = 'md') => {
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
    primary: { bg: '#0066cc', color: '#fff', hover: '#0052a3' },
    secondary: { bg: '#f0f0f0', color: '#1a1a1a', hover: '#e0e0e0' },
    danger: { bg: '#ffebee', color: '#d32f2f', hover: '#d32f2f' },
    warning: { bg: '#ffc107', color: '#fff', hover: '#ffb300' },
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

const StyledButton: React.FC<StyledButtonProps> = ({
  variant = 'primary',
  size = 'md',
  children,
  ...props
}) => {
  const styles = getButtonStyles(variant, size);

  return (
    <button
      {...props}
      style={styles.base as React.CSSProperties}
      onMouseEnter={(e) => {
        e.currentTarget.style.backgroundColor = styles.hover;
      }}
      onMouseLeave={(e) => {
        const variantConfig = getButtonStyles(variant, size);
        e.currentTarget.style.backgroundColor = (variantConfig.base as any).backgroundColor;
      }}
    >
      {children}
    </button>
  );
};

export default StyledButton;
