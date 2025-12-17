import React from 'react';
import { useTheme } from '../theme/useTheme';
import { Faq } from '../types';

interface FaqsTableProps {
  faqs: Faq[];
  onEdit: (faq: Faq) => void;
  onDelete: (id: string) => void;
}

const FaqsTable: React.FC<FaqsTableProps> = ({ faqs, onEdit, onDelete }) => {
  const { colors } = useTheme();

  return (
    <div style={{ backgroundColor: colors.background, borderRadius: '8px', border: `1px solid ${colors.border}`, overflow: 'hidden', marginBottom: '20px' }}>
      <div style={{ overflowX: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
          <thead>
            <tr style={{ backgroundColor: colors.backgroundSecondary, borderBottom: `2px solid ${colors.border}` }}>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Actions</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Question</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Short Answer</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Answer</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Order</th>
            </tr>
          </thead>
          <tbody>
            {faqs.map((faq, index) => (
              <tr
                key={faq.id || index}
                style={{
                  backgroundColor: index % 2 === 0 ? colors.background : colors.backgroundSecondary,
                  borderBottom: `1px solid ${colors.border}`,
                  transition: 'background-color 0.15s',
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.backgroundColor = colors.border;
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.backgroundColor = index % 2 === 0 ? colors.background : colors.backgroundSecondary;
                }}
              >
                <td style={{ padding: '16px', whiteSpace: 'nowrap' }}>
                  <button
                    onClick={() => onEdit(faq)}
                    style={{
                      padding: '6px 12px',
                      marginRight: '8px',
                      fontSize: '12px',
                      fontWeight: '600',
                      color: colors.primary,
                      backgroundColor: colors.primaryLight,
                      border: `1px solid ${colors.primary}`,
                      borderRadius: '4px',
                      cursor: 'pointer',
                      transition: 'all 0.2s',
                    }}
                    onMouseEnter={(e) => {
                      e.currentTarget.style.backgroundColor = colors.primary;
                      e.currentTarget.style.color = '#fff';
                    }}
                    onMouseLeave={(e) => {
                      e.currentTarget.style.backgroundColor = colors.primaryLight;
                      e.currentTarget.style.color = colors.primary;
                    }}
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => onDelete(faq.id!)}
                    style={{
                      padding: '6px 12px',
                      fontSize: '12px',
                      fontWeight: '600',
                      color: colors.danger,
                      backgroundColor: colors.dangerLight,
                      border: `1px solid ${colors.danger}`,
                      borderRadius: '4px',
                      cursor: 'pointer',
                      transition: 'all 0.2s',
                    }}
                    onMouseEnter={(e) => {
                      e.currentTarget.style.backgroundColor = colors.danger;
                      e.currentTarget.style.color = '#fff';
                    }}
                    onMouseLeave={(e) => {
                      e.currentTarget.style.backgroundColor = colors.dangerLight;
                      e.currentTarget.style.color = colors.danger;
                    }}
                  >
                    Delete
                  </button>
                </td>
                <td style={{ padding: '16px', fontWeight: '500', color: colors.foreground }}>{faq.question}</td>
                <td style={{ padding: '16px', color: colors.mutedText, maxWidth: '200px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                  {faq.shortAnswer}
                </td>
                <td style={{ padding: '16px', color: colors.mutedText, maxWidth: '400px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                  {faq.answer}
                </td>
                <td style={{ padding: '16px', color: colors.mutedText }}>{faq.order}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {faqs.length === 0 && (
        <div style={{ padding: '40px 20px', textAlign: 'center', color: colors.placeholder }}>
          No FAQs found
        </div>
      )}
    </div>
  );
};

export default FaqsTable;
