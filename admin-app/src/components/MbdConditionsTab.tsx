// admin-app/src/components/MbdConditionsTab.tsx
import React, { useEffect, useState } from 'react';
import { fetchMbdConditions, upsertMbdCondition, deleteMbdCondition, fetchMbdCondition, clearMbdConditionsCache } from '../services/apiService';
import { MbdCondition } from '../types';
import { getImageBaseUrl } from '../constants';
import { useTheme } from '../theme/useTheme';
import MbdConditionsSearch from './MbdConditionsSearch';
import MbdConditionsTable from './MbdConditionsTable';
import StyledButton from './StyledButton';
import MbdConditionModal from './MbdConditionModal';
import ErrorModal from './ErrorModal';

const MbdConditionsTab: React.FC = () => {
  const [mbdConditions, setMbdConditions] = useState<MbdCondition[]>([]);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [showDetailModal, setShowDetailModal] = useState<boolean>(false);
  const [selectedMbdConditionForModal, setSelectedMbdConditionForModal] = useState<MbdCondition | null>(null);
  const [showErrorModal, setShowErrorModal] = useState(false);
  const [modalErrorMessage, setModalErrorMessage] = useState('');


  useEffect(() => {
    loadMbdConditions();
  }, []);

  const loadMbdConditions = async () => {
    setLoading(true);
    try {
      const response = await fetchMbdConditions();
      if (response && Array.isArray(response)) {
        const sortedMbdConditions = response.sort((a: MbdCondition, b: MbdCondition) => a.name!.localeCompare(b.name!));
        setMbdConditions(sortedMbdConditions);
      } else {
        throw new Error('API response data is not an array or is missing.');
      }
    } catch (err) {
      setModalErrorMessage((err as Error).message || 'Failed to fetch conditions');
      setShowErrorModal(true);
    } finally {
      setLoading(false);
    }
  };

  const handleRefreshMbdConditions = () => {
    clearMbdConditionsCache();
    loadMbdConditions();
  };

  const selectMbdCondition = async (mbdCondition: MbdCondition) => {
    try {
      const data = await fetchMbdCondition(mbdCondition.id!, mbdCondition.name!);
      setSelectedMbdConditionForModal(data);
      setShowDetailModal(true);
    } catch (err) {
      setModalErrorMessage((err as Error).message || 'Failed to fetch condition details');
      setShowErrorModal(true);
    }
  };

  const handleCloseModal = () => {
    setShowDetailModal(false);
    setSelectedMbdConditionForModal(null);
  };

  const addMbdCondition = () => {
    setSelectedMbdConditionForModal({
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
  };

  const saveMbdCondition = async () => {
    if (!selectedMbdConditionForModal || !selectedMbdConditionForModal.name || selectedMbdConditionForModal.name.trim() === '') {
      alert('Must specify a Condition name');
      return;
    }

    try {
      const savedMbdCondition = await upsertMbdCondition(selectedMbdConditionForModal);
      setSelectedMbdConditionForModal(savedMbdCondition);
      loadMbdConditions(); // Reload table after save
      handleCloseModal();
    } catch (err) {
      setModalErrorMessage((err as Error).message || 'Failed to save condition');
      setShowErrorModal(true);
    }
  };

  const deleteMbdConditionConfirm = async (id: string, name: string) => {
    if (window.confirm(`Are you sure you want to delete ${name}?`)) {
      try {
        await deleteMbdCondition(id, name);
        loadMbdConditions(); // Reload table after deletion
        addMbdCondition(); // Reset form
      } catch (err) {
        setModalErrorMessage((err as Error).message || 'Failed to delete condition');
        setShowErrorModal(true);
      }
    }
  };


  const getImageUrl = (type: 'negative' | 'positive') => {
    if (!selectedMbdConditionForModal) return '';
    const baseUrl = getImageBaseUrl();

    let imageFileName = '';
    // Prioritize imageNegative/imagePositive from API response
    if (type === 'negative' && selectedMbdConditionForModal.imageNegative) {
      imageFileName = selectedMbdConditionForModal.imageNegative;
    } else if (type === 'positive' && selectedMbdConditionForModal.imagePositive) {
      imageFileName = selectedMbdConditionForModal.imagePositive;
    } else {
      // Fallback: Construct name from mbdCondition.name
      const conditionName = selectedMbdConditionForModal.name;
      if (!conditionName) return '';
      imageFileName = `${conditionName}${type === 'negative' ? '1' : '2'}`;
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

  const filteredMbdConditions = mbdConditions.filter((mbdCondition) => {
    const lowerCaseSearchTerm = searchTerm.toLowerCase();
    return (
      (mbdCondition.name?.toLowerCase().includes(lowerCaseSearchTerm)) ||
      (mbdCondition.physicalConnections?.some((conn) => conn.toLowerCase().includes(lowerCaseSearchTerm))) ||
      (mbdCondition.tags?.some((tag) => tag.toLowerCase().includes(lowerCaseSearchTerm)))
    );
  });

  const { colors } = useTheme();

  if (loading) return <div style={{ padding: '20px', color: colors.mutedText }}>Loading Conditions...</div>;


  return (
    <div>
      {/* Search Bar */}
      <MbdConditionsSearch searchTerm={searchTerm} onChange={(value) => handleSearchChange({ target: { value } } as React.ChangeEvent<HTMLInputElement>)} />

      {/* Table */}
      <MbdConditionsTable mbdConditions={filteredMbdConditions} onEdit={selectMbdCondition} onDelete={deleteMbdConditionConfirm} />

      {/* Add Button */}
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
        <StyledButton variant="secondary" size="md" onClick={handleRefreshMbdConditions}>
          Refresh Conditions
        </StyledButton>
        <StyledButton variant="primary" size="md" onClick={addMbdCondition}>
          + Add New Condition
        </StyledButton>
      </div>

      {/* Condition Detail Modal */}
      <MbdConditionModal
        isOpen={showDetailModal}
        mbdCondition={selectedMbdConditionForModal}
        onClose={handleCloseModal}
        onSave={saveMbdCondition}
        onChange={setSelectedMbdConditionForModal}
        getImageUrl={getImageUrl}
      />

      {showErrorModal && (
        <ErrorModal
          message={modalErrorMessage}
          onClose={() => setShowErrorModal(false)}
        />
      )}
    </div>
  );
};

export default MbdConditionsTab;