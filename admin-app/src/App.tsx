import React, { useState } from 'react';
import './App.css'; // Keep the existing App.css for now
import {
  ADMIN_API_URL,
  IMAGE_BASE_URL,
  AILMENTS_TABLE_CODE,
  AILMENT_CODE,
  DELETE_AILMENT_CODE,
  UPSERT_AILMENT_CODE,
  IMAGES_TABLE_CODE,
  DELETE_IMAGE_CODE,
  CONTACTS_TABLE_CODE,
  DELETE_CONTACT_CODE,
  SEND_PUSH_NOTIFICATION_CODE,
  CREATE_BACKUP_CODE,
  RESTORE_DATABASE_CODE
} from './constants';

function App() {
  const [activeTab, setActiveTab] = useState('ailments'); // 'ailments', 'images', 'contacts', 'notifications', 'database'

  const renderContent = () => {
    switch (activeTab) {
      case 'ailments':
        return <div>Ailments Content (Coming Soon)</div>;
      case 'images':
        return <div>Images Content (Coming Soon)</div>;
      case 'contacts':
        return <div>Contacts Content (Coming Soon)</div>;
      case 'notifications':
        return <div>Notifications Content (Coming Soon)</div>;
      case 'database':
        return <div>Database Content (Coming Soon)</div>;
      default:
        return <div>Select a tab</div>;
    }
  };

  return (
    <div className="container">
      <div><h1 className="text-center">MBD Admin Portal</h1></div>
      <nav className="mt-3 mb-3">
        <div className="nav nav-tabs nav-fill" id="nav-tab" role="tablist">
          <a
            className={`nav-item nav-link ${activeTab === 'ailments' ? 'active' : ''}`}
            onClick={() => setActiveTab('ailments')}
            href="#"
          >
            Ailments
          </a>
          <a
            className={`nav-item nav-link ${activeTab === 'images' ? 'active' : ''}`}
            onClick={() => setActiveTab('images')}
            href="#"
          >
            Images
          </a>
          <a
            className={`nav-item nav-link ${activeTab === 'contacts' ? 'active' : ''}`}
            onClick={() => setActiveTab('contacts')}
            href="#"
          >
            Contacts
          </a>
          <a
            className={`nav-item nav-link ${activeTab === 'notifications' ? 'active' : ''}`}
            onClick={() => setActiveTab('notifications')}
            href="#"
          >
            Notifications
          </a>
          <a
            className={`nav-item nav-link ${activeTab === 'database' ? 'active' : ''}`}
            onClick={() => setActiveTab('database')}
            href="#"
          >
            Database
          </a>
        </div>
      </nav>
      <div className="tab-content" id="nav-tabContent">
        {renderContent()}
      </div>
    </div>
  );
}

export default App;