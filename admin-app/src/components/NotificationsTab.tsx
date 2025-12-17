// admin-app/src/components/NotificationsTab.tsx
import React, { useState, useEffect } from 'react';
import { sendPushNotification, fetchMbdConditions } from '../services/apiService';
import { MbdCondition } from '../types';

interface MbdConditionOption {
    id?: string;
    name?: string;
}

const NotificationsTab: React.FC = () => {
  const [notificationTitle, setNotificationTitle] = useState('');
  const [notificationBody, setNotificationBody] = useState('');
  const [notificationTag, setNotificationTag] = useState('0'); // '0' for Everyone, '1' for Subscribers Only
  const [notificationMbdCondition, setNotificationMbdCondition] = useState('0');
  const [mbdConditionOptions, setMbdConditionOptions] = useState<MbdConditionOption[]>([]);
  const [loadingMbdConditions, setLoadingMbdConditions] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [alertMessage, setAlertMessage] = useState<string | null>(null);

  useEffect(() => {
    loadMbdConditionOptions();
  }, []);

  const loadMbdConditionOptions = async () => {
    setLoadingMbdConditions(true);
    try {
        const response = await fetchMbdConditions();
        if (response && Array.isArray(response)) {
            const sorted = response
          .map((mbdCondition: MbdCondition) => ({ id: mbdCondition.id, name: mbdCondition.name }))
          .sort((a, b) => (a.name || '').localeCompare(b.name || ''));
            setMbdConditionOptions(sorted);
        } else {
            throw new Error('API response data for MbdConditions is not an array or is missing.');
        }
    } catch (err: any) {
        setError(err.message || "Failed to load conditions for notifications");
    } finally {
        setLoadingMbdConditions(false);
    }
  };

  const sendNotification = async () => {
    if (notificationTitle.trim() === '') {
      alert("Must Input a Title");
      return;
    }
    if (notificationBody.trim() === '') {
      alert("Must Input a Body");
      return;
    }

    setAlertMessage(null);
    setError(null);

    const notificationPayload = {
      Title: notificationTitle,
      Body: notificationBody,
      SubscribersOnly: notificationTag === '1' ? 'true' : 'false',
      AilmentId: notificationMbdCondition !== '0' ? notificationMbdCondition : '', // Send empty string if no condition selected
    };

    try {
      await sendPushNotification(notificationPayload);
      setAlertMessage("Notification Sent");
      setNotificationTitle('');
      setNotificationBody('');
      setNotificationTag('0');
      setNotificationMbdCondition('0');
    } catch (err: any) {
      setError(err.message || 'Failed to send notification');
    }
  };

  return (
    <div className="tab-pane fade show active" id="nav-notifications" role="tabpanel" aria-labelledby="nav-notifications-tab">
      <div className="card">
        <div className="card-body">
          <h5 className="card-title">Push Notifications</h5>
          <div id="sendNotificationDiv">
            <div className="form-group">
              <label htmlFor="notificationTag">Who To Notify:</label>
              <select
                className="form-control"
                id="notificationTag"
                value={notificationTag}
                onChange={(e) => setNotificationTag(e.target.value)}
              >
                <option value="0">Everyone</option>
                <option value="1">Subscribers Only</option>
              </select>
            </div>
            <div className="form-group">
              <label htmlFor="notificationTitle">Title:</label>
              <div className="input-group">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Notification Title"
                  id="notificationTitle"
                  maxLength={65}
                  value={notificationTitle}
                  onChange={(e) => setNotificationTitle(e.target.value)}
                />
                <div className="input-group-append">
                  <span id="titleLength" className="input-group-text">
                    {notificationTitle.length}/65
                  </span>
                </div>
              </div>
            </div>
            <div className="form-group">
              <label htmlFor="notificationBody">Body:</label>
              <div className="input-group">
                <textarea
                  className="form-control maxedLength"
                  rows={3}
                  id="notificationBody"
                  maxLength={150}
                  value={notificationBody}
                  onChange={(e) => setNotificationBody(e.target.value)}
                ></textarea>
                <div className="input-group-append">
                  <span id="bodyLength" className="input-group-text">
                    {notificationBody.length}/150
                  </span>
                </div>
              </div>
            </div>
            <div className="form-group">
              <label htmlFor="notificationMbdCondition">Associate with Condition:</label>
              <select
                className="form-control"
                id="notificationMbdCondition"
                value={notificationMbdCondition}
                onChange={(e) => setNotificationMbdCondition(e.target.value)}
                disabled={loadingMbdConditions}
              >
                <option value="0">No Condition... (Select Condition If Applicable)</option>
                {mbdConditionOptions.map((mbdCondition) => (
                  <option key={mbdCondition.id} value={mbdCondition.id!}>
                    {mbdCondition.name}
                  </option>
                ))}
              </select>
              {loadingMbdConditions && <span>Loading conditions...</span>}
              {error && <div className="text-danger mt-1">Error loading conditions.</div>}
            </div>
            <button className="btn btn-primary" onClick={sendNotification}>
              Send Notification
            </button>
          </div>
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

export default NotificationsTab;