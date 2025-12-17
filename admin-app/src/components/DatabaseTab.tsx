// admin-app/src/components/DatabaseTab.tsx
import React, { useState } from 'react';
import { createBackupUrl, restoreDatabase } from '../services/apiService';
import { COSMOS_DB_CONTAINER_NAME } from '../constants';
import { useTheme } from '../theme/useTheme';

const DatabaseTab: React.FC = () => {
  const { colors } = useTheme();
  const [databaseFile, setDatabaseFile] = useState<File | null>(null);
  const [fileLabel, setFileLabel] = useState('Choose file');
  const [alertMessage, setAlertMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const file = e.target.files[0];
      setDatabaseFile(file);
      setFileLabel(file.name);
    } else {
      setDatabaseFile(null);
      setFileLabel('Choose file');
    }
  };

  const submitBackup = async () => {
    if (window.confirm("Are you sure you want to restore the database from backup?\nThis will overwrite data from the backup file!")) {
      if (!databaseFile) {
        alert("Must select a file");
        return;
      }

      setAlertMessage(null);
      setError(null);

      try {
        await restoreDatabase(databaseFile);
        setAlertMessage('Database Successfully restored!');
        setDatabaseFile(null);
        setFileLabel('Choose file');
      } catch (err: any) {
        setError(err.message || 'Failed to restore database!');
      }
    }
  };

  return (
    <div style={{ padding: '20px' }}>
      <div style={{
        backgroundColor: colors.background,
        borderRadius: '8px',
        border: `1px solid ${colors.border}`,
        padding: '24px'
      }}>
        <h5 style={{ fontSize: '18px', fontWeight: '600', margin: '0 0 24px 0', color: colors.foreground }}>Database Management</h5>

        <div style={{ maxWidth: '600px' }}>
          {/* Download Backup Section */}
          <div style={{ marginBottom: '32px', paddingBottom: '24px', borderBottom: `1px solid ${colors.border}` }}>
            <h6 style={{ fontSize: '15px', fontWeight: '600', marginBottom: '12px', color: colors.foreground }}>Backup Database</h6>
            <p style={{ fontSize: '13px', color: colors.mutedText, marginBottom: '16px' }}>
              Download a complete JSON backup of the {COSMOS_DB_CONTAINER_NAME} container.
            </p>
            <a href={createBackupUrl(COSMOS_DB_CONTAINER_NAME)} download={`${COSMOS_DB_CONTAINER_NAME}_backup.json`} style={{ textDecoration: 'none' }}>
              <button
                style={{
                  padding: '10px 20px',
                  backgroundColor: colors.primary,
                  color: '#fff',
                  border: 'none',
                  borderRadius: '6px',
                  cursor: 'pointer',
                  fontSize: '14px',
                  fontWeight: '600',
                  transition: 'all 0.2s',
                  display: 'flex',
                  alignItems: 'center',
                  gap: '8px'
                }}
                onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.primaryHover}
                onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.primary}
              >
                <span>⬇️</span> Download Database Backup
              </button>
            </a>
          </div>

          {/* Restore Database Section */}
          <div>
            <h6 style={{ fontSize: '15px', fontWeight: '600', marginBottom: '12px', color: colors.foreground }}>Restore Database</h6>
            <p style={{ fontSize: '13px', color: colors.mutedText, marginBottom: '16px' }}>
              Restore the database from a JSON backup file. <strong style={{ color: colors.danger }}>Warning: This will overwrite existing data.</strong>
            </p>

            <div style={{ marginBottom: '16px' }}>
              <div style={{ position: 'relative', overflow: 'hidden', display: 'inline-block', width: '100%' }}>
                <input
                  type="file"
                  id="databaseFile"
                  onChange={handleFileChange}
                  style={{
                    position: 'absolute',
                    left: 0,
                    top: 0,
                    opacity: 0,
                    width: '100%',
                    height: '100%',
                    cursor: 'pointer'
                  }}
                />
                <div style={{
                  padding: '10px 12px',
                  fontSize: '14px',
                  border: `1px solid ${colors.inputBorder}`,
                  borderRadius: '6px',
                  backgroundColor: colors.inputBackground,
                  color: colors.mutedText,
                  whiteSpace: 'nowrap',
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'space-between'
                }}>
                  <span>{fileLabel}</span>
                  <span style={{ fontSize: '12px', padding: '2px 8px', backgroundColor: colors.neutral, borderRadius: '4px', color: colors.lightText }}>Browse</span>
                </div>
              </div>
            </div>

            <button
              onClick={submitBackup}
              disabled={!databaseFile}
              style={{
                padding: '10px 20px',
                backgroundColor: colors.danger,
                color: '#fff',
                border: 'none',
                borderRadius: '6px',
                cursor: !databaseFile ? 'not-allowed' : 'pointer',
                fontSize: '14px',
                fontWeight: '600',
                transition: 'all 0.2s',
                opacity: !databaseFile ? 0.6 : 1
              }}
              onMouseEnter={(e) => { if (databaseFile) e.currentTarget.style.backgroundColor = colors.dangerHover; }}
              onMouseLeave={(e) => { if (databaseFile) e.currentTarget.style.backgroundColor = colors.danger; }}
            >
              Restore Database
            </button>
          </div>
        </div>

        {alertMessage && (
          <div style={{ marginTop: '20px', padding: '12px', backgroundColor: colors.successLight, color: colors.success, borderRadius: '6px' }}>
            {alertMessage}
          </div>
        )}
        {error && (
          <div style={{ marginTop: '20px', padding: '12px', backgroundColor: colors.dangerLight, color: colors.danger, borderRadius: '6px' }}>
            Error: {error}
          </div>
        )}
      </div>
    </div>
  );
};

export default DatabaseTab;
