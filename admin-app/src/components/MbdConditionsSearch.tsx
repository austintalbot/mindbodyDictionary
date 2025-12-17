import React from 'react';
import { useTheme } from '../theme/useTheme';

interface MbdConditionsSearchProps {
  searchTerm: string;
  onChange: (value: string) => void;
}

const MbdConditionsSearch: React.FC<MbdConditionsSearchProps> = ({ searchTerm, onChange }) => {
  const { colors } = useTheme();
  return (
    <div style={{ marginBottom: '24px' }}>
      <input
        type="text"
        placeholder="Search by Name, Physical Connections, or Tags"
        value={searchTerm}
        onChange={(e) => onChange(e.target.value)}
        style={{
          width: '100%',
          padding: '12px 16px',
          fontSize: '14px',
          border: `1px solid ${colors.inputBorder}`,
          backgroundColor: colors.inputBackground,
          color: colors.foreground,
          borderRadius: '6px',
          boxSizing: 'border-box',
          fontFamily: 'inherit',
          transition: 'border-color 0.2s, box-shadow 0.2s',
          outline: 'none',
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

export default MbdConditionsSearch;
