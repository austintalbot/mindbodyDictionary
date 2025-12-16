// admin-app/src/components/ErrorModal.tsx
import React from 'react';
import { useTheme } from '../theme/useTheme';

interface ErrorModalProps {
  message: string;
  onClose: () => void;
}

const ErrorModal: React.FC<ErrorModalProps> = ({ message, onClose }) => {
  const { colors } = useTheme();

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0, 0, 0, 0.5)',
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      zIndex: 1000,
    }}>
      <div style={{
        backgroundColor: colors.background,
        padding: '30px',
        borderRadius: '8px',
        maxWidth: '500px',
        width: '90%',
        boxShadow: '0 4px 15px rgba(0, 0, 0, 0.2)',
        border: `1px solid ${colors.border}`,
        display: 'flex',
        flexDirection: 'column',
        gap: '20px',
      }}>
        <h4 style={{
          color: colors.danger,
          fontSize: '20px',
          fontWeight: '600',
          margin: 0,
        }}>Error</h4>
        <p style={{
          color: colors.foreground,
          fontSize: '15px',
          lineHeight: '1.5',
          margin: 0,
        }}>{message}</p>
        <button
          onClick={onClose}
          style={{
            alignSelf: 'flex-end',
            padding: '10px 20px',
            backgroundColor: colors.primary,
            color: '#fff',
            border: 'none',
            borderRadius: '6px',
            cursor: 'pointer',
            fontSize: '14px',
            fontWeight: '600',
            transition: 'all 0.2s',
          }}
          onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.primaryHover}
          onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.primary}
        >
          Close
        </button>
      </div>
    </div>
  );
};

export default ErrorModal;
