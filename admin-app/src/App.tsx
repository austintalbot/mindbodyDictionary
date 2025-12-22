import { useState } from 'react';
import './App.css';
import { ThemeProvider } from './theme/ThemeProvider';
import { useTheme } from './theme/useTheme';
import MbdConditionsTab from './components/MbdConditionsTab';
import ImagesTab from './components/ImagesTab';
import ContactsTab from './components/ContactsTab';
import NotificationsTab from './components/NotificationsTab';
import DatabaseTab from './components/DatabaseTab';
import FaqsTab from './components/FaqsTab';
import MovementLinksTab from './components/MovementLinksTab';
import ThemeToggle from './components/ThemeToggle';

function AppContent() {
  const { colors } = useTheme();
  const [activeTab, setActiveTab] = useState('conditions');

  const renderContent = () => {
    switch (activeTab) {
      case 'conditions':
        return <MbdConditionsTab />;
      case 'images':
        return <ImagesTab />;
      case 'contacts':
        return <ContactsTab />;
      case 'notifications':
        return <NotificationsTab />;
      case 'faqs':
        return <FaqsTab />;
      case 'links':
        return <MovementLinksTab />;
      case 'database':
        return <DatabaseTab />;
      default:
        return <p style={{ color: colors.foreground }}>Tab content for {activeTab} not yet implemented</p>;
    }
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: colors.backgroundSecondary, color: colors.foreground }}>
      {/* Header */}
      <div style={{ backgroundColor: colors.background, borderBottom: `1px solid ${colors.border}`, padding: '30px 20px' }}>
        <div style={{ maxWidth: '1400px', margin: '0 auto', display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
            <img src="/icon.png" alt="MBD Logo" style={{ width: '48px', height: '48px', objectFit: 'contain' }} />
            <div>
              <h1 style={{ fontSize: '28px', fontWeight: '700', margin: '0 0 4px 0', color: colors.foreground }}>
                MBD Admin Portal
              </h1>
              <p style={{ fontSize: '14px', color: colors.mutedText, margin: 0 }}>
                Manage your Mind-Body Dictionary content
              </p>
            </div>
          </div>
          <ThemeToggle />
        </div>
      </div>

      {/* Navigation */}
      <div style={{ backgroundColor: colors.background, borderBottom: `1px solid ${colors.border}`, padding: '0 20px' }}>
        <div style={{ maxWidth: '1400px', margin: '0 auto', display: 'flex', gap: '2px' }}>
          {['conditions', 'images', 'contacts', 'notifications', 'faqs', 'links', 'database'].map((tab) => (
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
