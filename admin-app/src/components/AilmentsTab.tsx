// admin-app/src/components/AilmentsTab.tsx
import React, { useEffect, useState } from 'react';
import { fetchMbdConditionsTable, upsertAilment, deleteAilment, fetchMbdCondition } from '../services/apiService';
import { MbdCondition } from '../types';
import { getImageBaseUrl } from '../constants';
import { useTheme } from '../theme/useTheme';
import AilmentsSearch from './AilmentsSearch';
import AilmentsTable from './AilmentsTable';
import StyledButton from './StyledButton';
import AilmentModal from './AilmentModal';

// Interface for what Ailment data looks like, extends MbdCondition for additional properties if any
interface Ailment extends MbdCondition {}

const AilmentsTab: React.FC = () => {
  const [ailments, setAilments] = useState<Ailment[]>([]);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showDetailModal, setShowDetailModal] = useState<boolean>(false);
  const [selectedAilmentForModal, setSelectedAilmentForModal] = useState<Ailment | null>(null);


  useEffect(() => {
    loadAilments();
  }, []);

  const loadAilments = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetchMbdConditionsTable(); // Changed to fetchMbdConditionsTable
      if (response && Array.isArray(response)) {
        const sortedAilments = response.sort((a, b) => a.name!.localeCompare(b.name!));
        setAilments(sortedAilments);
      } else {
        throw new Error('API response data is not an array or is missing.');
      }
    } catch (err) {
      setError((err as Error).message || 'Failed to fetch ailments');
    } finally {
      setLoading(false);
    }
  };

  const selectAilment = async (ailment: Ailment) => {
    setError(null);
    try {
      const data = await fetchMbdCondition(ailment.id!, ailment.name!);
      setSelectedAilmentForModal(data);
      setShowDetailModal(true);
    } catch (err) {
      setError((err as Error).message || 'Failed to fetch ailment details');
    }
  };

  const handleCloseModal = () => {
    setShowDetailModal(false);
    setSelectedAilmentForModal(null);
    setError(null);
  };

  const addAilment = () => {
    setSelectedAilmentForModal({
      id: '',
      name: '',
      subscriptionOnly: false,
      summaryNegative: '',
      summaryPositive: '',
      affirmations: [],
      physicalConnections: [],
      tags: [],
      recommendations: [],
    });
    setShowDetailModal(true);
    setError(null);
  };

  const saveAilment = async () => {
    if (!selectedAilmentForModal || !selectedAilmentForModal.name || selectedAilmentForModal.name.trim() === '') {
      alert('Must specify an Ailment name');
      return;
    }

    try {
      const savedAilment = await upsertAilment(selectedAilmentForModal);
      setSelectedAilmentForModal(savedAilment);
      loadAilments(); // Reload table after save
      handleCloseModal();
    } catch (err) {
      setError((err as Error).message || 'Failed to save ailment');
    }
  };

  const deleteAilmentConfirm = async (id: string, name: string) => {
    if (window.confirm(`Are you sure you want to delete ${name}?`)) {
      try {
        await deleteAilment(id, name);
        loadAilments(); // Reload table after deletion
        addAilment(); // Reset form
      } catch (err) {
        setError((err as Error).message || 'Failed to delete ailment');
      }
    }
  };


  const getImageUrl = (type: 'negative' | 'positive') => {
    if (!selectedAilmentForModal) return '';
    const baseUrl = getImageBaseUrl();

    let imageFileName = '';
    // Prioritize imageNegative/imagePositive from API response
    if (type === 'negative' && selectedAilmentForModal.imageNegative) {
      imageFileName = selectedAilmentForModal.imageNegative;
    } else if (type === 'positive' && selectedAilmentForModal.imagePositive) {
      imageFileName = selectedAilmentForModal.imagePositive;
    } else {
      // Fallback: Construct name from ailment.name
      const ailmentName = selectedAilmentForModal.name;
      if (!ailmentName) return '';
      imageFileName = `${ailmentName}${type === 'negative' ? '1' : '2'}`;
    }

    // Ensure .png extension is present, but avoid double extensions
    const finalImageName = (imageFileName && !/\.(png|jpg|jpeg)$/i.test(imageFileName))
      ? `${imageFileName}.png`
      : imageFileName;

    // This constructs the final URL
    return `${baseUrl}/${finalImageName}`;
  };


  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(event.target.value);
  };

  const filteredAilments = ailments.filter((ailment) => {
    const lowerCaseSearchTerm = searchTerm.toLowerCase();
    return (
      (ailment.name?.toLowerCase().includes(lowerCaseSearchTerm)) ||
      (ailment.physicalConnections?.some((conn) => conn.toLowerCase().includes(lowerCaseSearchTerm))) ||
      (ailment.tags?.some((tag) => tag.toLowerCase().includes(lowerCaseSearchTerm)))
    );
  });

  const { colors } = useTheme();

  if (loading) return <div style={{ padding: '20px', color: colors.mutedText }}>Loading Ailments...</div>;
  if (error) return <div style={{ padding: '20px', color: colors.danger, backgroundColor: colors.dangerLight, borderRadius: '4px' }}>Error: {error}</div>;

  return (
    <div>
      {/* Search Bar */}
      <AilmentsSearch searchTerm={searchTerm} onChange={(value) => handleSearchChange({ target: { value } } as React.ChangeEvent<HTMLInputElement>)} />

      {/* Table */}
      <AilmentsTable ailments={filteredAilments} onEdit={selectAilment} onDelete={deleteAilmentConfirm} />

      {/* Add Button */}
      <StyledButton variant="primary" size="md" onClick={addAilment}>
        + Add New Ailment
      </StyledButton>

      {/* Ailment Detail Modal */}

      {/* Ailment Detail Modal */}
      <AilmentModal
        isOpen={showDetailModal}
        ailment={selectedAilmentForModal}
        onClose={handleCloseModal}
        onSave={saveAilment}
        onChange={setSelectedAilmentForModal}
        getImageUrl={getImageUrl}
      />
    </div>
  );
};

export default AilmentsTab;
