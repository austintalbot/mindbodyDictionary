import { useState } from 'react';
import './App.css';

function AppContent() {
  const [activeTab, setActiveTab] = useState('ailments');

  return (
    <div style={{ padding: '20px', fontFamily: 'sans-serif' }}>
      <h1>MBD Admin Portal</h1>
      <p>Manage your Mind-Body Dictionary content</p>
      
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

      <div style={{ marginTop: '20px', padding: '20px', border: '1px solid #ccc', borderRadius: '4px' }}>
        <p>Selected: {activeTab}</p>
      </div>
    </div>
  );
}

function App() {
  return <AppContent />;
}

export default App;