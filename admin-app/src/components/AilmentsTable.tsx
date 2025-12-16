import React from 'react';
import { useTheme } from '../theme/useTheme';
import { MbdCondition } from '../types';

interface AilmentsTableProps {
  ailments: MbdCondition[];
  onEdit: (ailment: MbdCondition) => void;
  onDelete: (id: string, name: string) => void;
}

const AilmentsTable: React.FC<AilmentsTableProps> = ({ ailments, onEdit, onDelete }) => {
  const { colors } = useTheme();
  return (
    <div style={{ backgroundColor: colors.background, borderRadius: '8px', border: `1px solid ${colors.border}`, overflow: 'hidden', marginBottom: '20px' }}>
      <div style={{ overflowX: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
          <thead>
            <tr style={{ backgroundColor: '#f8f9fa', borderBottom: '2px solid #e9ecef' }}>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: '#495057' }}>Actions</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: '#495057' }}>Name</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: '#495057' }}>Physical Connections</th>
              <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: '#495057' }}>Tags</th>
            </tr>
          </thead>
          <tbody>
            {ailments.map((ailment) => (
              <tr
                key={ailment.id}
                style={{ borderBottom: '1px solid #e9ecef', transition: 'background-color 0.15s' }}
                onMouseEnter={(e) => { e.currentTarget.style.backgroundColor = '#f8f9fa'; }}
                onMouseLeave={(e) => { e.currentTarget.style.backgroundColor = '#fff'; }}
              >
                <td style={{ padding: '16px', whiteSpace: 'nowrap' }}>
                  <button
                    onClick={() => onEdit(ailment)}
                    style={{
                      padding: '6px 12px',
                      marginRight: '8px',
                      fontSize: '12px',
                      fontWeight: '600',
                      color: '#0066cc',
                      backgroundColor: '#f0f7ff',
                      border: '1px solid #0066cc',
                      borderRadius: '4px',
                      cursor: 'pointer',
                      transition: 'all 0.2s',
                    }}
                    onMouseEnter={(e) => {
                      e.currentTarget.style.backgroundColor = '#0066cc';
                      e.currentTarget.style.color = '#fff';
                    }}
                    onMouseLeave={(e) => {
                      e.currentTarget.style.backgroundColor = '#f0f7ff';
                      e.currentTarget.style.color = '#0066cc';
                    }}
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => onDelete(ailment.id!, ailment.name!)}
                    style={{
                      padding: '6px 12px',
                      fontSize: '12px',
                      fontWeight: '600',
                      color: '#d32f2f',
                      backgroundColor: '#ffebee',
                      border: '1px solid #d32f2f',
                      borderRadius: '4px',
                      cursor: 'pointer',
                      transition: 'all 0.2s',
                    }}
                    onMouseEnter={(e) => {
                      e.currentTarget.style.backgroundColor = '#d32f2f';
                      e.currentTarget.style.color = '#fff';
                    }}
                    onMouseLeave={(e) => {
                      e.currentTarget.style.backgroundColor = '#ffebee';
                      e.currentTarget.style.color = '#d32f2f';
                    }}
                  >
                    Delete
                  </button>
                </td>
                <td style={{ padding: '16px', fontWeight: '500', color: '#1a1a1a' }}>{ailment.name}</td>
                <td style={{ padding: '16px', color: '#6c757d', fontSize: '13px' }}>
                  {(ailment.physicalConnections || []).length > 0
                    ? (ailment.physicalConnections || []).join(', ')
                    : <span style={{ color: '#adb5bd' }}>—</span>
                  }
                </td>
                <td style={{ padding: '16px', color: '#6c757d', fontSize: '13px' }}>
                  {(ailment.tags || []).length > 0
                    ? (ailment.tags || []).map((tag: string, i: number) => (
                        <span key={i} style={{
                          display: 'inline-block',
                          backgroundColor: '#e3f2fd',
                          color: '#1976d2',
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
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {ailments.length === 0 && (
        <div style={{ padding: '40px 20px', textAlign: 'center', color: '#adb5bd' }}>
          No ailments found
        </div>
      )}
    </div>
  );
};

export default AilmentsTable;
