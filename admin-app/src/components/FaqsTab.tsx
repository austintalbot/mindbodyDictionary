import React, { useEffect, useState } from 'react';
import { fetchFaqs, upsertFaq, deleteFaq, updateFaqsOrder } from '../services/apiService';
import { Faq } from '../types';
import { useTheme } from '../theme/useTheme';
import FaqsTable from './FaqsTable';
import FaqModal from './FaqModal';
import ErrorModal from './ErrorModal';

const FaqsTab: React.FC = () => {
  const { colors } = useTheme();
  const [faqs, setFaqs] = useState<Faq[]>([]);
  const [filteredFaqs, setFilteredFaqs] = useState<Faq[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [selectedFaq, setSelectedFaq] = useState<Faq | null>(null);
  const [showErrorModal, setShowErrorModal] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const [isUpdatingOrder, setIsUpdatingOrder] = useState(false);

    // Filtering state
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
      loadFaqs();
    }, []);

    useEffect(() => {
      applyFilters();
    }, [faqs, searchTerm]);

    const loadFaqs = async () => {
      setLoading(true);
      try {
        const data = await fetchFaqs();
        if (Array.isArray(data)) {
          // Ensure every FAQ has an order, defaulting to index if missing
          const normalizedData = data.map((faq, index) => ({
            ...faq,
            order: typeof faq.order === 'number' ? faq.order : index
          }));
          const sortedData = normalizedData.sort((a, b) => (a.order || 0) - (b.order || 0));
          setFaqs(sortedData);
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

    const applyFilters = () => {
      let result = [...faqs];

      if (searchTerm.trim()) {
        const lowerSearch = searchTerm.toLowerCase();
        result = result.filter(faq =>
          faq.question.toLowerCase().includes(lowerSearch) ||
          faq.answer.toLowerCase().includes(lowerSearch) ||
          (faq.shortAnswer && faq.shortAnswer.toLowerCase().includes(lowerSearch))
        );
      }

      setFilteredFaqs(result);
    };

    const handleAddFaq = () => {
      setSelectedFaq({
        question: '',
        shortAnswer: '',
        answer: '',
        order: faqs.length > 0 ? Math.max(...faqs.map(f => f.order || 0)) + 1 : 0,
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
      if (!selectedFaq.question.trim() || !selectedFaq.answer.trim() || !selectedFaq.shortAnswer?.trim()) {
          alert("Question, Short Answer, and Answer are required.");
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

    const handleReorder = async (reorderedFaqs: Faq[]) => {
      if (searchTerm.trim()) {
        alert("Cannot reorder while searching. Please clear the search first.");
        return;
      }

      setIsUpdatingOrder(true);

      // 1. Assign new sequential orders
      const updatedFaqs = reorderedFaqs.map((faq, index) => ({
        ...faq,
        order: index
      }));

      // 2. Update local state immediately for UI responsiveness
      setFaqs(updatedFaqs);

      // 3. Persist to database in bulk
      try {
        await updateFaqsOrder(updatedFaqs);
        console.log('Database FAQ order updated successfully.');
      } catch (err: any) {
        setErrorMessage(err.message || 'Failed to update FAQ order in database');
        setShowErrorModal(true);
        // Re-load from server to revert local state to what's actually in DB
        loadFaqs();
      } finally {
        setIsUpdatingOrder(false);
      }
    };

    if (loading && faqs.length === 0) return <div style={{ padding: '20px', color: colors.mutedText }}>Loading FAQs...</div>;

    return (
      <div style={{ padding: '20px' }}>
        <div style={{
          backgroundColor: colors.background,
          borderRadius: '8px',
          border: `1px solid ${colors.border}`,
          padding: '24px'
        }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
            <div>
              <h5 style={{ fontSize: '18px', fontWeight: '600', margin: 0, color: colors.foreground }}>Freqently Asked Questions</h5>
              {isUpdatingOrder && <span style={{ fontSize: '12px', color: colors.primary, fontWeight: '500' }}>Saving new order...</span>}
            </div>
            <div style={{ display: 'flex', gap: '12px' }}>
              <button
                onClick={() => { loadFaqs(); }}
                disabled={isUpdatingOrder}
                style={{
                  padding: '8px 16px',
                  backgroundColor: colors.backgroundSecondary,
                  color: colors.foreground,
                  border: `1px solid ${colors.border}`,
                  borderRadius: '6px',
                  cursor: isUpdatingOrder ? 'not-allowed' : 'pointer',
                  fontWeight: '600',
                  fontSize: '14px',
                  transition: 'all 0.2s',
                  opacity: isUpdatingOrder ? 0.6 : 1
                }}
                onMouseEnter={(e) => !isUpdatingOrder && (e.currentTarget.style.backgroundColor = colors.neutral)}
                onMouseLeave={(e) => !isUpdatingOrder && (e.currentTarget.style.backgroundColor = colors.backgroundSecondary)}
              >
                Refresh
              </button>
              <button
                onClick={handleAddFaq}
                disabled={isUpdatingOrder}
                style={{
                  padding: '8px 16px',
                  backgroundColor: colors.primary,
                  color: '#fff',
                  border: 'none',
                  borderRadius: '6px',
                  cursor: isUpdatingOrder ? 'not-allowed' : 'pointer',
                  fontSize: '14px',
                  fontWeight: '600',
                  transition: 'all 0.2s',
                  opacity: isUpdatingOrder ? 0.6 : 1
                }}
                onMouseEnter={(e) => !isUpdatingOrder && (e.currentTarget.style.backgroundColor = colors.primaryHover)}
                onMouseLeave={(e) => !isUpdatingOrder && (e.currentTarget.style.backgroundColor = colors.primary)}
              >
                + Add FAQ
              </button>
            </div>
          </div>

          {/* Filters */}
          <div style={{ display: 'flex', gap: '16px', marginBottom: '24px', flexWrap: 'wrap' }}>
            <div style={{ flex: 1, minWidth: '250px' }}>
              <input
                type="text"
                placeholder="Search FAQs (question, answer)..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                disabled={isUpdatingOrder}
                style={{
                  width: '100%',
                  padding: '10px 16px',
                  borderRadius: '8px',
                  border: `1px solid ${colors.border}`,
                  backgroundColor: colors.inputBackground,
                  color: colors.foreground,
                  fontSize: '14px',
                  opacity: isUpdatingOrder ? 0.6 : 1
                }}
              />
            </div>
          </div>

          <FaqsTable
            faqs={filteredFaqs}
            onEdit={handleEditFaq}
            onDelete={handleDeleteFaq}
            onReorder={handleReorder}
          />

          {searchTerm.trim() === '' && faqs.length > 1 && (
            <div style={{ fontSize: '12px', color: colors.mutedText, textAlign: 'center', marginTop: '10px' }}>
              💡 Drag rows to reorder them permanently in the database.
            </div>
          )}
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
