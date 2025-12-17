import React from 'react';
import { useTheme } from '../theme/useTheme';

interface StyledTextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
}

const StyledTextarea: React.FC<StyledTextareaProps> = ({ label, ...props }) => {
  const { colors } = useTheme();
  return (
    <div style={{ marginBottom: '20px' }}>
      {label && (
        <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>
          {label}
        </label>
      )}
      <textarea
        {...props}
        style={{
          width: '100%',
          padding: '10px 12px',
          fontSize: '13px',
          border: `1px solid ${colors.inputBorder}`,
          borderRadius: '6px',
          backgroundColor: colors.inputBackground,
          color: colors.foreground,
          fontFamily: 'inherit',
          transition: 'all 0.2s',
          resize: 'vertical',
          minHeight: '100px',
          outline: 'none',
          ...(props.style as React.CSSProperties),
        }}
        onFocus={(e) => {
          e.currentTarget.style.borderColor = colors.inputFocus;
          e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
        }}
        onBlur={(e) => {
          e.currentTarget.style.borderColor = colors.inputBorder;
          e.currentTarget.style.boxShadow = 'none';
        }}
      />
    </div>
  );
};

export default StyledTextarea;
