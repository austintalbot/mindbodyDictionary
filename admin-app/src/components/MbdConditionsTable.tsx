import React from 'react';
import { useTheme } from '../theme/useTheme';
import { MbdCondition } from '../types';

interface MbdConditionsTableProps {
  mbdConditions: MbdCondition[];
  onEdit: (mbdCondition: MbdCondition) => void;
  onDelete: (id: string, name: string) => void;
}

const MbdConditionsTable: React.FC<MbdConditionsTableProps> = ({ mbdConditions, onEdit, onDelete }) => {
  const { colors } = useTheme();
  return (
    <div style={{ backgroundColor: colors.background, borderRadius: '8px', border: `1px solid ${colors.border}`, overflow: 'hidden', marginBottom: '20px' }}>
      <div style={{ overflowX: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
          <thead>
            <tr style={{ backgroundColor: colors.backgroundSecondary, borderBottom: `2px solid ${colors.border}` }}>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Actions</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Name</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Physical Connections</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Tags</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Sub. Only</th>
            </tr>
          </thead>
          <tbody>
            {mbdConditions.map((mbdCondition, index) => (
              <tr
                key={mbdCondition.id}
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
                    onClick={() => onEdit(mbdCondition)}
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
                    onClick={() => onDelete(mbdCondition.id!, mbdCondition.name!)}
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
                <td style={{ padding: '16px', fontWeight: '500', color: colors.foreground }}>{mbdCondition.name}</td>
                <td style={{ padding: '16px', color: colors.mutedText, fontSize: '13px' }}>
                  {(mbdCondition.physicalConnections || []).length > 0
                    ? (mbdCondition.physicalConnections || []).join(', ')
                    : <span style={{ color: colors.placeholder }}>—</span>
                  }
                </td>
                <td style={{ padding: '16px', color: colors.mutedText, fontSize: '13px' }}>
                  {(mbdCondition.searchTags || []).length > 0
                    ? (mbdCondition.searchTags || []).map((tag: string, i: number) => (
                        <span key={i} style={{
                          display: 'inline-block',
                          backgroundColor: colors.primaryLight,
                          color: colors.primary,
                          padding: '2px 8px',
                          borderRadius: '12px',
                          marginRight: '4px',
                          marginBottom: '4px',
                          fontSize: '12px'
                        }}>
                          {tag}
                        </span>
                      ))
                    : <span style={{ color: '#adb5bd' }}>—</span>
                  }
                </td>
                <td style={{ padding: '16px', color: colors.mutedText, fontSize: '13px' }}>
                  {mbdCondition.subscriptionOnly ? (
                    <span style={{ color: colors.success, fontWeight: '600' }}>Yes</span>
                  ) : (
                    <span style={{ color: colors.mutedText }}>No</span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {mbdConditions.length === 0 && (
        <div style={{ padding: '40px 20px', textAlign: 'center', color: colors.placeholder }}>
          No conditions found
        </div>
      )}
    </div>
  );
};

export default MbdConditionsTable;
