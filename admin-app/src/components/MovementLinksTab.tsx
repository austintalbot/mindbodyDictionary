import React, { useEffect, useState } from 'react';
import { fetchMovementLinks, upsertMovementLink, deleteMovementLink, updateMovementLinksOrder } from '../services/apiService';
import { MbdMovementLink } from '../types';
import { useTheme } from '../theme/useTheme';
import MovementLinksTable from './MovementLinksTable';
import MovementLinkModal from './MovementLinkModal';
import ErrorModal from './ErrorModal';

const MovementLinksTab: React.FC = () => {
  const { colors } = useTheme();
  const [links, setLinks] = useState<MbdMovementLink[]>([]);
  const [filteredLinks, setFilteredLinks] = useState<MbdMovementLink[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [selectedLink, setSelectedLink] = useState<MbdMovementLink | null>(null);
  const [showErrorModal, setShowErrorModal] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [isUpdatingOrder, setIsUpdatingOrder] = useState(false);

  // Filtering state
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadLinks();
  }, []);

  useEffect(() => {
    applyFilters();
  }, [links, searchTerm]);

  const loadLinks = async () => {
    setLoading(true);
    try {
      const data = await fetchMovementLinks();
      if (Array.isArray(data)) {
        const normalizedData = data.map((link, index) => ({
          ...link,
          order: typeof link.order === 'number' ? link.order : index
        }));
        const sortedData = normalizedData.sort((a, b) => (a.order || 0) - (b.order || 0));
        setLinks(sortedData);
      } else {
        setLinks([]);
      }
    } catch (err: any) {
      setErrorMessage(err.message || 'Failed to load movement links');
      setShowErrorModal(true);
    } finally {
      setLoading(false);
    }
  };

  const applyFilters = () => {
    let result = [...links];

    if (searchTerm.trim()) {
      const lowerSearch = searchTerm.toLowerCase();
      result = result.filter(link =>
        link.title.toLowerCase().includes(lowerSearch) ||
        link.url.toLowerCase().includes(lowerSearch)
      );
    }

    setFilteredLinks(result);
  };

  const handleAddLink = () => {
    setSelectedLink({
      title: '',
      url: '',
      order: links.length > 0 ? Math.max(...links.map(l => l.order || 0)) + 1 : 0,
    });
    setShowModal(true);
  };

  const handleEditLink = (link: MbdMovementLink) => {
    setSelectedLink(link);
    setShowModal(true);
  };

  const handleDeleteLink = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this movement link?')) {
      try {
        await deleteMovementLink(id);
        loadLinks();
      } catch (err: any) {
        setErrorMessage(err.message || 'Failed to delete movement link');
        setShowErrorModal(true);
      }
    }
  };

  const handleSaveLink = async () => {
    if (!selectedLink) return;
    if (!selectedLink.title.trim() || !selectedLink.url.trim()) {
        alert("Title and URL are required.");
        return;
    }

    try {
      await upsertMovementLink(selectedLink);
      setShowModal(false);
      loadLinks();
    } catch (err: any) {
      setErrorMessage(err.message || 'Failed to save movement link');
      setShowErrorModal(true);
    }
  };

  const handleReorder = async (reorderedLinks: MbdMovementLink[]) => {
    if (searchTerm.trim()) {
      alert("Cannot reorder while searching. Please clear the search first.");
      return;
    }

    setIsUpdatingOrder(true);
    const updatedLinks = reorderedLinks.map((link, index) => ({
      ...link,
      order: index
    }));

    setLinks(updatedLinks);

    try {
      await updateMovementLinksOrder(updatedLinks);
    } catch (err: any) {
      setErrorMessage(err.message || 'Failed to update links order');
      setShowErrorModal(true);
      loadLinks();
    } finally {
      setIsUpdatingOrder(false);
    }
  };

  if (loading && links.length === 0) return <div style={{ padding: '20px', color: colors.mutedText }}>Loading movement links...</div>;

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
            <h5 style={{ fontSize: '18px', fontWeight: '600', margin: 0, color: colors.foreground }}>MindBody Movement Links</h5>
            {isUpdatingOrder && <span style={{ fontSize: '12px', color: colors.primary, fontWeight: '500' }}>Saving new order...</span>}
          </div>
          <div style={{ display: 'flex', gap: '12px' }}>
            <button
              onClick={() => { loadLinks(); }}
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
            >
              Refresh
            </button>
            <button
              onClick={handleAddLink}
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
            >
              + Add Link
            </button>
          </div>
        </div>

        <div style={{ display: 'flex', gap: '16px', marginBottom: '24px' }}>
          <div style={{ flex: 1 }}>
            <input
              type="text"
              placeholder="Search links..."
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

        <MovementLinksTable
          links={filteredLinks}
          onEdit={handleEditLink}
          onDelete={handleDeleteLink}
          onReorder={handleReorder}
        />

        {searchTerm.trim() === '' && links.length > 1 && (
          <div style={{ fontSize: '12px', color: colors.mutedText, textAlign: 'center', marginTop: '10px' }}>
            💡 Drag rows to reorder them permanently.
          </div>
        )}
      </div>
      <MovementLinkModal
        isOpen={showModal}
        link={selectedLink}
        onClose={() => setShowModal(false)}
        onSave={handleSaveLink}
        onChange={setSelectedLink}
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

export default MovementLinksTab;
