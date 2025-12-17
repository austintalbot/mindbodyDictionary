import React, { useEffect, useState } from 'react';
import { fetchFaqs, upsertFaq, deleteFaq } from '../services/apiService';
import { Faq } from '../types';
import { useTheme } from '../theme/useTheme';
import FaqsTable from './FaqsTable';
import FaqModal from './FaqModal';
import ErrorModal from './ErrorModal';

const FaqsTab: React.FC = () => {
  const { colors } = useTheme();
  const [faqs, setFaqs] = useState<Faq[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [selectedFaq, setSelectedFaq] = useState<Faq | null>(null);
  const [showErrorModal, setShowErrorModal] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    loadFaqs();
  }, []);

  const loadFaqs = async () => {
    setLoading(true);
    try {
      const data = await fetchFaqs();
      if (Array.isArray(data)) {
        setFaqs(data.sort((a, b) => (a.order || 0) - (b.order || 0)));
      } else {
        setFaqs([]);
      }
    } catch (err: any) {
      setErrorMessage(err.message || 'Failed to load FAQs');
      setShowErrorModal(true);
    } finally {
      setLoading(false);
    }
  };

  const handleAddFaq = () => {
    setSelectedFaq({
      question: '',
      answer: '',
      order: 0,
    });
    setShowModal(true);
  };

  const handleEditFaq = (faq: Faq) => {
    setSelectedFaq(faq);
    setShowModal(true);
  };

  const handleDeleteFaq = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this FAQ?')) {
      try {
        await deleteFaq(id);
        loadFaqs();
      } catch (err: any) {
        setErrorMessage(err.message || 'Failed to delete FAQ');
        setShowErrorModal(true);
      }
    }
  };

  const handleSaveFaq = async () => {
    if (!selectedFaq) return;
    if (!selectedFaq.question.trim() || !selectedFaq.answer.trim()) {
        alert("Question and Answer are required.");
        return;
    }

    try {
      await upsertFaq(selectedFaq);
      setShowModal(false);
      loadFaqs();
    } catch (err: any) {
      setErrorMessage(err.message || 'Failed to save FAQ');
      setShowErrorModal(true);
    }
  };

  if (loading) return <div style={{ padding: '20px', color: colors.mutedText }}>Loading FAQs...</div>;

  return (
    <div style={{ padding: '20px' }}>
      <div style={{
        backgroundColor: colors.background,
        borderRadius: '8px',
        border: `1px solid ${colors.border}`,
        padding: '24px'
      }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
          <h5 style={{ fontSize: '18px', fontWeight: '600', margin: 0, color: colors.foreground }}>Freqently Asked Questions</h5>
          <button
            onClick={() => { loadFaqs(); }}
            style={{
              padding: '8px 16px',
              backgroundColor: colors.primary,
              color: '#fff',
              border: 'none',
              borderRadius: '6px',
              cursor: 'pointer',
              fontWeight: '600',
              fontSize: '14px',
              transition: 'all 0.2s'
            }}
            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.primaryHover}
            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.primary}
          >
            Refresh List
          </button>
        </div>

        <FaqsTable
          faqs={faqs}
          onEdit={handleEditFaq}
          onDelete={handleDeleteFaq}
        />

        <button
          onClick={handleAddFaq}
          style={{
            padding: '10px 20px',
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
          + Add New FAQ
        </button>
      </div>

      <FaqModal
        isOpen={showModal}
        faq={selectedFaq}
        onClose={() => setShowModal(false)}
        onSave={handleSaveFaq}
        onChange={setSelectedFaq}
      />

      {showErrorModal && (
        <ErrorModal
          message={errorMessage}
          onClose={() => setShowErrorModal(false)}
        />
      )}
    </div>
  );
};

export default FaqsTab;
