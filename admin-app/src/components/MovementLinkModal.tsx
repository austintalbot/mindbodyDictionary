import React from 'react';
import { useTheme } from '../theme/useTheme';
import { MbdMovementLink } from '../types';

interface MovementLinkModalProps {
  isOpen: boolean;
  link: MbdMovementLink | null;
  onClose: () => void;
  onSave: () => void;
  onChange: (link: MbdMovementLink) => void;
}

const MovementLinkModal: React.FC<MovementLinkModalProps> = ({
  isOpen,
  link,
  onClose,
  onSave,
  onChange,
}) => {
  const { colors } = useTheme();

  if (!isOpen || !link) return null;

  return (
    <div style={{
      display: 'flex',
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: colors.shadow,
      zIndex: 1050,
      alignItems: 'center',
      justifyContent: 'center',
      padding: '20px'
    }}>
      <div style={{
        backgroundColor: colors.background,
        borderRadius: '8px',
        width: '100%',
        maxWidth: '600px',
        maxHeight: '90vh',
        display: 'flex',
        flexDirection: 'column',
        boxShadow: `0 10px 40px ${colors.shadowHeavy}`
      }}>
        {/* Modal Header */}
        <div style={{
          padding: '24px',
          borderBottom: `1px solid ${colors.border}`,
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center'
        }}>
          <h2 style={{ margin: 0, fontSize: '20px', fontWeight: '700', color: colors.foreground }}>
            {link.id ? 'Edit Movement Link' : 'Add New Movement Link'}
          </h2>
          <button
            onClick={onClose}
            style={{
              background: 'none',
              border: 'none',
              fontSize: '24px',
              color: colors.mutedText,
              cursor: 'pointer',
              padding: 0,
              width: '32px',
              height: '32px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              borderRadius: '4px',
              transition: 'all 0.2s'
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.backgroundColor = colors.neutral;
              e.currentTarget.style.color = colors.foreground;
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.backgroundColor = 'transparent';
              e.currentTarget.style.color = colors.mutedText;
            }}
          >
            ✕
          </button>
        </div>

        {/* Modal Body */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '32px', display: 'flex', flexDirection: 'column', gap: '20px' }}>

          {/* Title Field */}
          <div>
            <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.mutedText }} htmlFor="title">Title</label>
            <input
              type="text"
              id="title"
              value={link.title}
              onChange={(e) => onChange({ ...link, title: e.target.value })}
              style={{
                width: '100%',
                padding: '10px 12px',
                fontSize: '14px',
                border: `1px solid ${colors.inputBorder}`,
                borderRadius: '6px',
                transition: 'all 0.2s',
                backgroundColor: colors.inputBackground,
                color: colors.foreground
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

          {/* URL Field */}
          <div>
            <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.mutedText }} htmlFor="url">URL</label>
            <input
              type="text"
              id="url"
              value={link.url}
              onChange={(e) => onChange({ ...link, url: e.target.value })}
              style={{
                width: '100%',
                padding: '10px 12px',
                fontSize: '14px',
                border: `1px solid ${colors.inputBorder}`,
                borderRadius: '6px',
                transition: 'all 0.2s',
                backgroundColor: colors.inputBackground,
                color: colors.foreground
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

          {/* Order Field */}
          <div>
            <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.mutedText }} htmlFor="order">Display Order</label>
            <input
              type="number"
              id="order"
              value={link.order || 0}
              onChange={(e) => onChange({ ...link, order: parseInt(e.target.value, 10) || 0 })}
              style={{
                width: '100%',
                padding: '10px 12px',
                fontSize: '14px',
                border: `1px solid ${colors.inputBorder}`,
                borderRadius: '6px',
                transition: 'all 0.2s',
                backgroundColor: colors.inputBackground,
                color: colors.foreground
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
        </div>

        {/* Modal Footer */}
        <div style={{
          padding: '20px 24px',
          borderTop: `1px solid ${colors.border}`,
          display: 'flex',
          justifyContent: 'flex-end',
          gap: '12px'
        }}>
          <button
            onClick={onClose}
            style={{
              padding: '10px 24px',
              backgroundColor: colors.backgroundSecondary,
              color: colors.foreground,
              border: 'none',
              borderRadius: '6px',
              cursor: 'pointer',
              fontSize: '14px',
              fontWeight: '600',
              transition: 'all 0.2s'
            }}
            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.neutralHover}
            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.backgroundSecondary}
          >
            Cancel
          </button>
          <button
            onClick={onSave}
            style={{
              padding: '10px 24px',
              backgroundColor: colors.primary,
              color: colors.buttonText,
              border: 'none',
              borderRadius: '6px',
              cursor: 'pointer',
              fontSize: '14px',
              fontWeight: '600',
              transition: 'all 0.2s'
            }}
            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.primaryHover}
            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.primary}
          >
            Save Link
          </button>
        </div>
      </div>
    </div>
  );
};

export default MovementLinkModal;
