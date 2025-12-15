import React, { useState } from 'react';
import './App.css';
import AilmentsTab from './components/AilmentsTab';
import ImagesTab from './components/ImagesTab';
import ContactsTab from './components/ContactsTab';
import NotificationsTab from './components/NotificationsTab';
import DatabaseTab from './components/DatabaseTab';

function App() {
  const [activeTab, setActiveTab] = useState('ailments'); // 'ailments', 'images', 'contacts', 'notifications', 'database'

  const renderContent = () => {
    switch (activeTab) {
      case 'ailments':
        return <AilmentsTab />;
      case 'images':
        return <ImagesTab />;
      case 'contacts':
        return <ContactsTab />;
      case 'notifications':
        return <NotificationsTab />;
      case 'database':
        return <DatabaseTab />;
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