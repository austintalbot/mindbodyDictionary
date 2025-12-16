import React from 'react';

interface StyledInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
}

const StyledInput: React.FC<StyledInputProps> = ({ label, ...props }) => {
  return (
    <div style={{ marginBottom: '20px' }}>
      {label && (
        <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: '#495057' }}>
          {label}
        </label>
      )}
      <input
        {...props}
        style={{
          width: '100%',
          padding: '10px 12px',
          fontSize: '14px',
          border: '1px solid #d0d0d0',
          borderRadius: '6px',
          backgroundColor: '#fff',
          color: '#1a1a1a',
          fontFamily: 'inherit',
          transition: 'all 0.2s',
          outline: 'none',
          ...(props.style as React.CSSProperties),
        }}
        onFocus={(e) => {
          e.currentTarget.style.borderColor = '#0066cc';
          e.currentTarget.style.boxShadow = '0 0 0 3px rgba(0, 102, 204, 0.1)';
        }}
        onBlur={(e) => {
          e.currentTarget.style.borderColor = '#d0d0d0';
          e.currentTarget.style.boxShadow = 'none';
        }}
      />
    </div>
  );
};

export default StyledInput;
