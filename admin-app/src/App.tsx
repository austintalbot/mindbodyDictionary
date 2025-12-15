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
    <div style={{ minHeight: '100vh', padding: '20px', backgroundColor: '#f5f5f5' }}>
      <h1>MBD Admin Portal</h1>
      <p>Active tab: {activeTab}</p>
      
      <div style={{ marginTop: '20px', display: 'flex', gap: '10px' }}>
        {['ailments', 'images', 'contacts', 'notifications', 'database'].map((tab) => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            style={{
              padding: '10px 15px',
              backgroundColor: activeTab === tab ? '#007bff' : '#f0f0f0',
              color: activeTab === tab ? 'white' : 'black',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer',
            }}
          >
            {tab}
          </button>
        ))}
      </div>

      <div style={{ marginTop: '20px', padding: '20px', backgroundColor: '#fff', borderRadius: '4px', border: '1px solid #ddd' }}>
        {renderContent()}
      </div>
    </div>
  );
}

function App() {
  return <AppContent />;
}

export default App;