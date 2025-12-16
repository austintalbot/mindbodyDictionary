import { useState } from 'react';
import './App.css';
import AilmentsTab from './components/AilmentsTab';

function AppContent() {
  const [activeTab, setActiveTab] = useState('ailments');

  const renderContent = () => {
    switch (activeTab) {
      case 'ailments':
        return <AilmentsTab />;
      default:
        return <p>Tab content for {activeTab} not yet implemented</p>;
    }
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f8f9fa' }}>
      {/* Header */}
      <div style={{ backgroundColor: '#fff', borderBottom: '1px solid #e9ecef', padding: '30px 20px' }}>
        <div style={{ maxWidth: '1400px', margin: '0 auto' }}>
          <h1 style={{ fontSize: '28px', fontWeight: '700', margin: '0 0 8px 0', color: '#1a1a1a' }}>
            MBD Admin Portal
          </h1>
          <p style={{ fontSize: '14px', color: '#6c757d', margin: 0 }}>
            Manage your Mind-Body Dictionary content
          </p>
        </div>
      </div>

      {/* Navigation */}
      <div style={{ backgroundColor: '#fff', borderBottom: '1px solid #e9ecef', padding: '0 20px' }}>
        <div style={{ maxWidth: '1400px', margin: '0 auto', display: 'flex', gap: '2px' }}>
          {['ailments', 'images', 'contacts', 'notifications', 'database'].map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              style={{
                padding: '16px 20px',
                fontSize: '14px',
                fontWeight: activeTab === tab ? '600' : '500',
                textTransform: 'capitalize',
                background: 'none',
                border: 'none',
                borderBottom: activeTab === tab ? '3px solid #0066cc' : '3px solid transparent',
                color: activeTab === tab ? '#0066cc' : '#6c757d',
                cursor: 'pointer',
                transition: 'all 0.2s ease',
              }}
            >
              {tab}
            </button>
          ))}
        </div>
      </div>

      {/* Content */}
      <div style={{ padding: '30px 20px' }}>
        <div style={{ maxWidth: '1400px', margin: '0 auto' }}>
          {renderContent()}
        </div>
      </div>
    </div>
  );
}

function App() {
  return <AppContent />;
}

export default App;
