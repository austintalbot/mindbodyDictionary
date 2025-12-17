// admin-app/src/components/NotificationsTab.tsx
import React, { useState, useEffect } from 'react';
import { sendPushNotification, fetchMbdConditions } from '../services/apiService';
import { MbdCondition } from '../types';
import { useTheme } from '../theme/useTheme';

interface MbdConditionOption {
    id?: string;
    name?: string;
}

const NotificationsTab: React.FC = () => {
  const { colors } = useTheme();
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
    <div style={{ padding: '20px' }}>
      <div style={{
        backgroundColor: colors.background,
        borderRadius: '8px',
        border: `1px solid ${colors.border}`,
        padding: '24px'
      }}>
        <h5 style={{ fontSize: '18px', fontWeight: '600', margin: '0 0 20px 0', color: colors.foreground }}>Push Notifications</h5>

        <div style={{ maxWidth: '600px' }}>
          <div style={{ marginBottom: '16px' }}>
            <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Who To Notify:</label>
            <select
              value={notificationTag}
              onChange={(e) => setNotificationTag(e.target.value)}
              style={{
                width: '100%',
                padding: '10px 12px',
                fontSize: '14px',
                border: `1px solid ${colors.inputBorder}`,
                borderRadius: '6px',
                backgroundColor: colors.inputBackground,
                color: colors.foreground,
                outline: 'none',
                cursor: 'pointer'
              }}
            >
              <option value="0">Everyone</option>
              <option value="1">Subscribers Only</option>
            </select>
          </div>

          <div style={{ marginBottom: '16px' }}>
            <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Title:</label>
            <div style={{ position: 'relative' }}>
              <input
                type="text"
                placeholder="Notification Title"
                maxLength={65}
                value={notificationTitle}
                onChange={(e) => setNotificationTitle(e.target.value)}
                style={{
                  width: '100%',
                  padding: '10px 12px',
                  fontSize: '14px',
                  border: `1px solid ${colors.inputBorder}`,
                  borderRadius: '6px',
                  backgroundColor: colors.inputBackground,
                  color: colors.foreground,
                  outline: 'none'
                }}
              />
              <span style={{ position: 'absolute', right: '10px', top: '10px', fontSize: '12px', color: colors.mutedText }}>
                {notificationTitle.length}/65
              </span>
            </div>
          </div>

          <div style={{ marginBottom: '16px' }}>
            <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Body:</label>
            <div style={{ position: 'relative' }}>
              <textarea
                rows={3}
                maxLength={150}
                value={notificationBody}
                onChange={(e) => setNotificationBody(e.target.value)}
                style={{
                  width: '100%',
                  padding: '10px 12px',
                  fontSize: '14px',
                  border: `1px solid ${colors.inputBorder}`,
                  borderRadius: '6px',
                  backgroundColor: colors.inputBackground,
                  color: colors.foreground,
                  outline: 'none',
                  resize: 'vertical',
                  fontFamily: 'inherit'
                }}
              ></textarea>
              <span style={{ position: 'absolute', right: '10px', bottom: '10px', fontSize: '12px', color: colors.mutedText }}>
                {notificationBody.length}/150
              </span>
            </div>
          </div>

          <div style={{ marginBottom: '24px' }}>
            <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Associate with Condition:</label>
            <select
              value={notificationMbdCondition}
              onChange={(e) => setNotificationMbdCondition(e.target.value)}
              disabled={loadingMbdConditions}
              style={{
                width: '100%',
                padding: '10px 12px',
                fontSize: '14px',
                border: `1px solid ${colors.inputBorder}`,
                borderRadius: '6px',
                backgroundColor: colors.inputBackground,
                color: colors.foreground,
                outline: 'none',
                cursor: loadingMbdConditions ? 'not-allowed' : 'pointer',
                opacity: loadingMbdConditions ? 0.7 : 1
              }}
            >
              <option value="0">No Condition... (Select Condition If Applicable)</option>
              {mbdConditionOptions.map((mbdCondition) => (
                <option key={mbdCondition.id} value={mbdCondition.id!}>
                  {mbdCondition.name}
                </option>
              ))}
            </select>
            {loadingMbdConditions && <span style={{ fontSize: '12px', color: colors.mutedText, marginTop: '4px', display: 'block' }}>Loading conditions...</span>}
            {error && <div style={{ fontSize: '12px', color: colors.danger, marginTop: '4px' }}>Error loading conditions.</div>}
          </div>

          <button
            onClick={sendNotification}
            style={{
              padding: '10px 24px',
              backgroundColor: colors.primary,
              color: '#fff',
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
            Send Notification
          </button>
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

export default NotificationsTab;
