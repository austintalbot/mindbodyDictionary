import { useState } from 'react';
import './App.css';
import { ThemeProvider } from './theme/ThemeProvider';
import { useTheme } from './theme/useTheme';
import AilmentsTab from './components/AilmentsTab';
import ThemeToggle from './components/ThemeToggle';

function AppContent() {
  const { colors } = useTheme();
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
    <div style={{ minHeight: '100vh', backgroundColor: colors.backgroundSecondary }}>
      {/* Header */}
      <div style={{ backgroundColor: colors.background, borderBottom: `1px solid ${colors.border}`, padding: '30px 20px' }}>
        <div style={{ maxWidth: '1400px', margin: '0 auto', display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <div>
            <h1 style={{ fontSize: '28px', fontWeight: '700', margin: '0 0 8px 0', color: colors.foreground }}>
              MBD Admin Portal
            </h1>
            <p style={{ fontSize: '14px', color: colors.mutedText, margin: 0 }}>
              Manage your Mind-Body Dictionary content
            </p>
          </div>
          <ThemeToggle />
        </div>
      </div>

      {/* Navigation */}
      <div style={{ backgroundColor: colors.background, borderBottom: `1px solid ${colors.border}`, padding: '0 20px' }}>
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
                borderBottom: activeTab === tab ? `3px solid ${colors.primary}` : '3px solid transparent',
                color: activeTab === tab ? colors.primary : colors.mutedText,
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
  return (
    <ThemeProvider>
      <AppContent />
    </ThemeProvider>
  );
}

export default App;
