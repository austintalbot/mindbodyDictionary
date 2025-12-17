// admin-app/src/components/DatabaseTab.tsx
import React, { useState } from 'react';
import { createBackupUrl, restoreDatabase } from '../services/apiService';

const DatabaseTab: React.FC = () => {
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
    <div className="tab-pane fade show active" id="nav-database" role="tabpanel" aria-labelledby="nav-database-tab">
      <div className="card">
        <div className="card-body">
          <h5 className="card-title">Database</h5>
          <div className="row">
            <div className="form-control mb-3">
              <a href={createBackupUrl()} download="mbd_backup.json">
                <button className="btn btn-primary" id="getDatabaseBackupButton">
                  Download Database
                </button>
              </a>
              {/* The original iframe was used for download, an <a> tag with download attribute is more React-friendly */}
            </div>
          </div>
          <div className="input-group mt-3 mb-3">
            <div className="custom-file">
              <input type="file" className="custom-file-input" id="databaseFile" onChange={handleFileChange} />
              <label className="custom-file-label" htmlFor="databaseFile">{fileLabel}</label>
            </div>
          </div>
          <button className="btn btn-danger" onClick={submitBackup}>
            Restore Database
          </button>
          {alertMessage && (
            <div className="alert alert-success mt-3" role="alert">
              {alertMessage}
            </div>
          )}
          {error && (
            <div className="alert alert-danger mt-3" role="alert">
              Error: {error}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default DatabaseTab;
