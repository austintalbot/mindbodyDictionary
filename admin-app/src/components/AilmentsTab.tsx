// admin-app/src/components/AilmentsTab.tsx
import React, { useEffect, useState } from 'react';
import { fetchMbdConditionsTable, upsertAilment, deleteAilment, fetchMbdCondition } from '../services/apiService';
import { MbdCondition, Recommendation } from '../types';
import { getImageBaseUrl } from '../constants'; // Import directly from constants
import { Tabs, TabsContent, TabsList, TabsTrigger } from './components/ui/tabs';

// Interface for what Ailment data looks like, extends MbdCondition for additional properties if any
interface Ailment extends MbdCondition {}

const AilmentsTab: React.FC = () => {
  const [ailments, setAilments] = useState<Ailment[]>([]);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [alertMessage, setAlertMessage] = useState<string | null>(null);
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

  const selectAilment = async (id: string, name: string) => {
    setAlertMessage(null);
    setError(null);
    try {
      const data = await fetchMbdCondition(id, name);
      setSelectedAilmentForModal(data);
      setShowDetailModal(true);
    } catch (err) {
      setError((err as Error).message || 'Failed to fetch ailment details');
    }
  };

  const handleCloseModal = () => {
    setShowDetailModal(false);
    setSelectedAilmentForModal(null);
    setError(null); // Clear any errors
    setAlertMessage(null); // Clear any alert messages
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
    setAlertMessage(null);
    setError(null);
  };

  const duplicateAilment = () => {
    if (selectedAilmentForModal) {
      setSelectedAilmentForModal({
        ...selectedAilmentForModal,
        id: '', // Clear ID for duplication
        name: '', // Clear name to prompt new name
      });
    }
  };

  const saveAilment = async () => {
    if (!selectedAilmentForModal || !selectedAilmentForModal.name || selectedAilmentForModal.name.trim() === '') {
      alert('Must specify an Ailment name');
      return;
    }

    try {
      const savedAilment = await upsertAilment(selectedAilmentForModal);
      setSelectedAilmentForModal(savedAilment);
      setAlertMessage('Saved!');
      loadAilments(); // Reload table after save
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

  const handleAilmentChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    if (selectedAilmentForModal) {
      const { id, value, type, checked } = e.target as HTMLInputElement;
      setSelectedAilmentForModal((prev) => ({
        ...prev!,
        [id]: type === 'checkbox' ? checked : value,
      }));
    }
  };

  const handleArrayChange = (index: number, value: string, arrayType: 'affirmations' | 'physicalConnections' | 'tags') => {
    if (selectedAilmentForModal && selectedAilmentForModal[arrayType]) {
      const newArray = [...selectedAilmentForModal[arrayType]!];
      newArray[index] = value;
      setSelectedAilmentForModal((prev) => ({
        ...prev!,
        [arrayType]: newArray,
      }));
    }
  };

  const addToArray = (arrayType: 'affirmations' | 'physicalConnections' | 'tags') => {
    if (selectedAilmentForModal) {
      setSelectedAilmentForModal((prev) => ({
        ...prev!,
        [arrayType]: [...(prev![arrayType] || []), ''],
      }));
    }
  };

  const handleRecommendationChange = (index: number, field: keyof Recommendation, value: string) => {
    if (selectedAilmentForModal && selectedAilmentForModal.recommendations) {
      const newRecommendations = [...selectedAilmentForModal.recommendations];
      newRecommendations[index] = {
        ...newRecommendations[index],
        [field]: field === 'recommendationType' ? parseInt(value) : value,
      };
      setSelectedAilmentForModal((prev) => ({
        ...prev!,
        recommendations: newRecommendations,
      }));
    }
  };

  const addRecommendation = () => {
    if (selectedAilmentForModal) {
      setSelectedAilmentForModal((prev) => ({
        ...prev!,
        recommendations: [...(prev!.recommendations || []), { name: '', url: '', recommendationType: 0 }],
      }));
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

  if (loading) return <div style={{ padding: '20px', color: '#666' }}>Loading Ailments...</div>;
  if (error) return <div style={{ padding: '20px', color: '#d32f2f', backgroundColor: '#ffebee', borderRadius: '4px' }}>Error: {error}</div>;

  return (
    <div>
      {/* Search Bar */}
      <div style={{ marginBottom: '24px' }}>
        <input
          type="text"
          placeholder="Search by Name, Physical Connections, or Tags"
          value={searchTerm}
          onChange={handleSearchChange}
          style={{
            width: '100%',
            padding: '12px 16px',
            fontSize: '14px',
            border: '1px solid #d0d0d0',
            borderRadius: '6px',
            boxSizing: 'border-box',
            fontFamily: 'inherit',
            transition: 'border-color 0.2s',
            outline: 'none',
          }}
          onFocus={(e) => {
            e.currentTarget.style.borderColor = '#0066cc';
            e.currentTarget.style.boxShadow = '0 0 0 3px rgba(0, 102, 204, 0.1)';
          }}
          onBlur={(e) => {
            e.currentTarget.style.borderColor = '#d0d0d0';
            e.currentTarget.style.boxShadow = 'none';
          }}
        />
      </div>

      {/* Table Container */}
      <div style={{ backgroundColor: '#fff', borderRadius: '8px', border: '1px solid #e9ecef', overflow: 'hidden', marginBottom: '20px' }}>
        <div style={{ overflowX: 'auto' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
            <thead>
              <tr style={{ backgroundColor: '#f8f9fa', borderBottom: '2px solid #e9ecef' }}>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: '#495057' }}>Actions</th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: '#495057' }}>Name</th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: '#495057' }}>Physical Connections</th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: '#495057' }}>Tags</th>
              </tr>
            </thead>
            <tbody>
              {filteredAilments.map((ailment) => (
                <tr key={ailment.id} style={{ borderBottom: '1px solid #e9ecef', transition: 'background-color 0.15s' }}
                    onMouseEnter={(e) => { e.currentTarget.style.backgroundColor = '#f8f9fa'; }}
                    onMouseLeave={(e) => { e.currentTarget.style.backgroundColor = '#fff'; }}>
                  <td style={{ padding: '16px', whiteSpace: 'nowrap' }}>
                    <button
                      onClick={() => selectAilment(ailment.id!, ailment.name!)}
                      style={{
                        padding: '6px 12px',
                        marginRight: '8px',
                        fontSize: '12px',
                        fontWeight: '600',
                        color: '#0066cc',
                        backgroundColor: '#f0f7ff',
                        border: '1px solid #0066cc',
                        borderRadius: '4px',
                        cursor: 'pointer',
                        transition: 'all 0.2s',
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = '#0066cc';
                        e.currentTarget.style.color = '#fff';
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = '#f0f7ff';
                        e.currentTarget.style.color = '#0066cc';
                      }}
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => deleteAilmentConfirm(ailment.id!, ailment.name!)}
                      style={{
                        padding: '6px 12px',
                        fontSize: '12px',
                        fontWeight: '600',
                        color: '#d32f2f',
                        backgroundColor: '#ffebee',
                        border: '1px solid #d32f2f',
                        borderRadius: '4px',
                        cursor: 'pointer',
                        transition: 'all 0.2s',
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = '#d32f2f';
                        e.currentTarget.style.color = '#fff';
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = '#ffebee';
                        e.currentTarget.style.color = '#d32f2f';
                      }}
                    >
                      Delete
                    </button>
                  </td>
                  <td style={{ padding: '16px', fontWeight: '500', color: '#1a1a1a' }}>{ailment.name}</td>
                  <td style={{ padding: '16px', color: '#6c757d', fontSize: '13px' }}>
                    {(ailment.physicalConnections || []).length > 0
                      ? (ailment.physicalConnections || []).join(', ')
                      : <span style={{ color: '#adb5bd' }}>—</span>
                    }
                  </td>
                  <td style={{ padding: '16px', color: '#6c757d', fontSize: '13px' }}>
                    {(ailment.tags || []).length > 0
                      ? (ailment.tags || []).map((tag, i) => (
                          <span key={i} style={{
                            display: 'inline-block',
                            backgroundColor: '#e3f2fd',
                            color: '#1976d2',
                            padding: '2px 8px',
                            borderRadius: '12px',
                            marginRight: '4px',
                            marginBottom: '4px',
                            fontSize: '12px'
                          }}>
                            {tag}
                          </span>
                        ))
                      : <span style={{ color: '#adb5bd' }}>—</span>
                    }
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        {filteredAilments.length === 0 && (
          <div style={{ padding: '40px 20px', textAlign: 'center', color: '#adb5bd' }}>
            No ailments found
          </div>
        )}
      </div>

      {/* Add Button */}
      <button
        onClick={addAilment}
        style={{
          padding: '10px 20px',
          fontSize: '14px',
          fontWeight: '600',
          color: '#fff',
          backgroundColor: '#0066cc',
          border: 'none',
          borderRadius: '6px',
          cursor: 'pointer',
          transition: 'all 0.2s',
        }}
        onMouseEnter={(e) => {
          e.currentTarget.style.backgroundColor = '#0052a3';
        }}
        onMouseLeave={(e) => {
          e.currentTarget.style.backgroundColor = '#0066cc';
        }}
      >
        + Add New Ailment
      </button>

      {/* Ailment Detail Modal */}
      {selectedAilmentForModal && (
        <div style={{
          display: showDetailModal ? 'flex' : 'none',
          position: 'fixed',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          backgroundColor: 'rgba(0,0,0,0.5)',
          zIndex: 1050,
          alignItems: 'center',
          justifyContent: 'center',
          padding: '20px'
        }}>
          <div style={{
            backgroundColor: '#fff',
            borderRadius: '8px',
            width: '100%',
            maxWidth: '900px',
            maxHeight: '90vh',
            display: 'flex',
            flexDirection: 'column',
            boxShadow: '0 10px 40px rgba(0,0,0,0.2)'
          }}>
            {/* Modal Header */}
            <div style={{
              padding: '24px',
              borderBottom: '1px solid #e9ecef',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}>
              <h2 style={{ margin: 0, fontSize: '20px', fontWeight: '700', color: '#1a1a1a' }}>
                {selectedAilmentForModal.id ? `Edit: ${selectedAilmentForModal.name}` : 'Add New Ailment'}
              </h2>
              <button
                onClick={handleCloseModal}
                style={{
                  background: 'none',
                  border: 'none',
                  fontSize: '24px',
                  color: '#6c757d',
                  cursor: 'pointer',
                  padding: 0,
                  width: '32px',
                  height: '32px',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  borderRadius: '4px',
                  transition: 'all 0.2s'
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.backgroundColor = '#f0f0f0';
                  e.currentTarget.style.color = '#000';
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.backgroundColor = 'transparent';
                  e.currentTarget.style.color = '#6c757d';
                }}
              >
                ✕
              </button>
            </div>

            {/* Modal Body */}
            <div style={{ flex: 1, overflowY: 'auto', padding: '24px' }}>
              <Tabs defaultValue="basicInfo" className="w-full">
                <TabsList className="grid w-full grid-cols-5" style={{
                  marginBottom: '20px',
                  borderBottom: '2px solid #e9ecef',
                  display: 'grid',
                  gridTemplateColumns: 'repeat(5, 1fr)',
                  gap: '8px'
                }}>
                  <TabsTrigger value="basicInfo" style={{
                    padding: '12px 16px',
                    borderRadius: 0,
                    backgroundColor: 'transparent',
                    border: 'none'
                  }}>Basic Info</TabsTrigger>
                  <TabsTrigger value="affirmations" style={{
                    padding: '12px 16px',
                    borderRadius: 0,
                    backgroundColor: 'transparent',
                    border: 'none'
                  }}>Affirmations</TabsTrigger>
                  <TabsTrigger value="physicalConnections" style={{
                    padding: '12px 16px',
                    borderRadius: 0,
                    backgroundColor: 'transparent',
                    border: 'none'
                  }}>Physical Connections</TabsTrigger>
                  <TabsTrigger value="tags" style={{
                    padding: '12px 16px',
                    borderRadius: 0,
                    backgroundColor: 'transparent',
                    border: 'none'
                  }}>Tags</TabsTrigger>
                  <TabsTrigger value="recommendations" style={{
                    padding: '12px 16px',
                    borderRadius: 0,
                    backgroundColor: 'transparent',
                    border: 'none'
                  }}>Recommendations</TabsTrigger>
                </TabsList>

                <TabsContent value="basicInfo">
                  <div style={{ padding: '20px' }}>
                    <h5 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '20px', color: '#1a1a1a' }}>Basic Information</h5>
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px' }}>
                      <div>
                        <div style={{ marginBottom: '20px' }}>
                          <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: '#495057' }} htmlFor="id">ID</label>
                          <input
                            type="text"
                            style={{
                              width: '100%',
                              padding: '10px 12px',
                              fontSize: '14px',
                              border: '1px solid #d0d0d0',
                              borderRadius: '6px',
                              backgroundColor: '#f8f9fa',
                              color: '#6c757d',
                              cursor: 'not-allowed'
                            }}
                            id="id"
                            value={selectedAilmentForModal.id || ''}
                            disabled
                          />
                        </div>
                        <div style={{ marginBottom: '20px' }}>
                          <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: '#495057' }} htmlFor="name">Name</label>
                          <div style={{ display: 'flex', gap: '8px' }}>
                            <input
                              type="text"
                              style={{
                                flex: 1,
                                padding: '10px 12px',
                                fontSize: '14px',
                                border: '1px solid #d0d0d0',
                                borderRadius: '6px 0 0 6px',
                                backgroundColor: '#fff',
                                color: '#1a1a1a',
                                transition: 'all 0.2s',
                                outline: 'none'
                              }}
                              id="name"
                              value={selectedAilmentForModal.name || ''}
                              onChange={handleAilmentChange}
                              disabled={!!selectedAilmentForModal.id}
                              placeholder={selectedAilmentForModal.id ? '' : 'Enter ailment name'}
                              onFocus={(e) => {
                                e.currentTarget.style.borderColor = '#0066cc';
                                e.currentTarget.style.boxShadow = '0 0 0 3px rgba(0, 102, 204, 0.1)';
                              }}
                              onBlur={(e) => {
                                e.currentTarget.style.borderColor = '#d0d0d0';
                                e.currentTarget.style.boxShadow = 'none';
                              }}
                            />
                            <button
                              style={{
                                padding: '10px 16px',
                                fontSize: '13px',
                                fontWeight: '600',
                                color: '#fff',
                                backgroundColor: '#ffc107',
                                border: 'none',
                                borderRadius: '0 6px 6px 0',
                                cursor: 'pointer',
                                transition: 'background-color 0.2s'
                              }}
                              onClick={duplicateAilment}
                              type="button"
                              onMouseEnter={(e) => { e.currentTarget.style.backgroundColor = '#ffb300'; }}
                              onMouseLeave={(e) => { e.currentTarget.style.backgroundColor = '#ffc107'; }}
                            >
                              Duplicate
                            </button>
                          </div>
                        </div>
                        <div style={{ marginBottom: '20px' }}>
                          <label style={{ display: 'flex', alignItems: 'center', fontSize: '14px', color: '#495057', cursor: 'pointer' }}>
                            <input
                              type="checkbox"
                              style={{
                                width: '16px',
                                height: '16px',
                                marginRight: '8px',
                                cursor: 'pointer',
                                accentColor: '#0066cc'
                              }}
                              id="subscriptionOnly"
                              checked={selectedAilmentForModal.subscriptionOnly}
                              onChange={handleAilmentChange}
                            />
                            Subscription Only
                          </label>
                        </div>
                      </div>
                      <div style={{ textAlign: 'center' }}>
                        <h6 style={{ fontSize: '13px', fontWeight: '600', color: '#495057', marginBottom: '12px' }}>Preview Images</h6>
                        <div style={{ marginBottom: '16px' }}>
                          <p style={{ fontSize: '12px', color: '#6c757d', marginBottom: '8px', fontWeight: '500' }}>Negative</p>
                          <img style={{ maxWidth: '100px', height: 'auto', borderRadius: '6px', border: '1px solid #e9ecef' }} src={getImageUrl('negative')} alt="Negative" />
                        </div>
                        <div>
                          <p style={{ fontSize: '12px', color: '#6c757d', marginBottom: '8px', fontWeight: '500' }}>Positive</p>
                          <img style={{ maxWidth: '100px', height: 'auto', borderRadius: '6px', border: '1px solid #e9ecef' }} src={getImageUrl('positive')} alt="Positive" />
                        </div>
                      </div>
                    </div>
                    <div style={{ marginTop: '24px', paddingTop: '20px', borderTop: '1px solid #e9ecef' }}>
                      <h6 style={{ fontSize: '14px', fontWeight: '600', color: '#1a1a1a', marginBottom: '16px' }}>Descriptions</h6>
                      <div style={{ marginBottom: '20px' }}>
                        <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: '#495057' }} htmlFor="summaryNegative">Negative Summary</label>
                        <textarea
                          style={{
                            width: '100%',
                            padding: '10px 12px',
                            fontSize: '13px',
                            border: '1px solid #d0d0d0',
                            borderRadius: '6px',
                            backgroundColor: '#fff',
                            color: '#1a1a1a',
                            fontFamily: 'inherit',
                            transition: 'all 0.2s',
                            resize: 'vertical',
                            minHeight: '100px',
                            outline: 'none'
                          }}
                          rows={4}
                          id="summaryNegative"
                          value={selectedAilmentForModal.summaryNegative || ''}
                          onChange={handleAilmentChange}
                          onFocus={(e) => {
                            e.currentTarget.style.borderColor = '#0066cc';
                            e.currentTarget.style.boxShadow = '0 0 0 3px rgba(0, 102, 204, 0.1)';
                          }}
                          onBlur={(e) => {
                            e.currentTarget.style.borderColor = '#d0d0d0';
                            e.currentTarget.style.boxShadow = 'none';
                          }}
                        ></textarea>
                      </div>
                      <div style={{ marginBottom: '20px' }}>
                        <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: '#495057' }} htmlFor="summaryPositive">Positive Summary</label>
                        <textarea
                          style={{
                            width: '100%',
                            padding: '10px 12px',
                            fontSize: '13px',
                            border: '1px solid #d0d0d0',
                            borderRadius: '6px',
                            backgroundColor: '#fff',
                            color: '#1a1a1a',
                            fontFamily: 'inherit',
                            transition: 'all 0.2s',
                            resize: 'vertical',
                            minHeight: '100px',
                            outline: 'none'
                          }}
                          rows={4}
                          id="summaryPositive"
                          value={selectedAilmentForModal.summaryPositive || ''}
                          onChange={handleAilmentChange}
                          onFocus={(e) => {
                            e.currentTarget.style.borderColor = '#0066cc';
                            e.currentTarget.style.boxShadow = '0 0 0 3px rgba(0, 102, 204, 0.1)';
                          }}
                          onBlur={(e) => {
                            e.currentTarget.style.borderColor = '#d0d0d0';
                            e.currentTarget.style.boxShadow = 'none';
                          }}
                        ></textarea>
                      </div>
                    </div>
                  </div>
                </TabsContent>

                <TabsContent value="affirmations">
                  <div style={{ padding: '20px' }}>
                    <div style={{ marginBottom: '20px' }}>
                      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
                        <h5 style={{ margin: 0, fontSize: '16px', fontWeight: '600', color: '#1a1a1a' }}>Affirmations</h5>
                        <button
                          type="button"
                          style={{
                            padding: '8px 16px',
                            fontSize: '13px',
                            fontWeight: '600',
                            color: '#fff',
                            backgroundColor: '#0066cc',
                            border: 'none',
                            borderRadius: '6px',
                            cursor: 'pointer',
                            transition: 'background-color 0.2s'
                          }}
                          onClick={() => addToArray('affirmations')}
                          onMouseEnter={(e) => { e.currentTarget.style.backgroundColor = '#0052a3'; }}
                          onMouseLeave={(e) => { e.currentTarget.style.backgroundColor = '#0066cc'; }}
                        >
                          + Add
                        </button>
                      </div>
                      <p style={{ fontSize: '13px', color: '#6c757d', marginBottom: '16px' }}>Positive affirmation statements</p>
                      <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                        {(selectedAilmentForModal.affirmations || []).map((affirmation, index) => (
                          <textarea
                            key={index}
                            style={{
                              width: '100%',
                              padding: '10px 12px',
                              fontSize: '13px',
                              border: '1px solid #d0d0d0',
                              borderRadius: '6px',
                              backgroundColor: '#fff',
                              color: '#1a1a1a',
                              fontFamily: 'inherit',
                              transition: 'all 0.2s',
                              resize: 'vertical',
                              minHeight: '80px',
                              outline: 'none'
                            }}
                            rows={2}
                            value={affirmation}
                            onChange={(e) => handleArrayChange(index, e.target.value, 'affirmations')}
                            onFocus={(e) => {
                              e.currentTarget.style.borderColor = '#0066cc';
                              e.currentTarget.style.boxShadow = '0 0 0 3px rgba(0, 102, 204, 0.1)';
                            }}
                            onBlur={(e) => {
                              e.currentTarget.style.borderColor = '#d0d0d0';
                              e.currentTarget.style.boxShadow = 'none';
                            }}
                          ></textarea>
                        ))}
                      </div>
                    </div>                  </div>                </TabsContent>

                <TabsContent value="physicalConnections">
                  <div style={{ padding: '20px' }}>
                    <div style={{ marginBottom: '20px' }}>
                      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
                        <h5 style={{ margin: 0, fontSize: '16px', fontWeight: '600', color: '#1a1a1a' }}>Physical Connections</h5>
                        <button
                          type="button"
                          style={{
                            padding: '8px 16px',
                            fontSize: '13px',
                            fontWeight: '600',
                            color: '#fff',
                            backgroundColor: '#0066cc',
                            border: 'none',
                            borderRadius: '6px',
                            cursor: 'pointer',
                            transition: 'background-color 0.2s'
                          }}
                          onClick={() => addToArray('physicalConnections')}
                          onMouseEnter={(e) => { e.currentTarget.style.backgroundColor = '#0052a3'; }}
                          onMouseLeave={(e) => { e.currentTarget.style.backgroundColor = '#0066cc'; }}
                        >
                          + Add
                        </button>
                      </div>
                      <p style={{ fontSize: '13px', color: '#6c757d', marginBottom: '16px' }}>Body locations referenced by this ailment</p>
                      <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                        <button type="button" className="inline-flex items-center px-3 py-1.5 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" onClick={() => addToArray('physicalConnections')}>
                          Add
                        </button>
                      </div>
                      <div className="space-y-2">
                        {(selectedAilmentForModal.physicalConnections || []).map((connection, index) => (
                          <input
                            key={index}
                            type="text"
                            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                            value={connection}
                            onChange={(e) => handleArrayChange(index, e.target.value, 'physicalConnections')}
                          />
                        ))}
                      </div>
                    </div>
                  </div>
                </TabsContent>

                <TabsContent value="tags">
                  <div className="p-4">
                      <h5 className="text-lg font-semibold mb-3">Tags</h5>
                      <div className="flex justify-between items-center mb-3">
                        <i className="block text-sm font-medium text-gray-700 dark:text-gray-300">Single words used for matching Ailment to search parameters</i>
                        <button type="button" className="inline-flex items-center px-3 py-1.5 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" onClick={() => addToArray('tags')}>
                          Add
                        </button>
                      </div>
                      <div className="space-y-2">
                        {(selectedAilmentForModal.tags || []).map((tag, index) => (
                          <input
                            key={index}
                            type="text"
                            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                            value={tag}
                            onChange={(e) => handleArrayChange(index, e.target.value, 'tags')}
                          />
                        ))}
                      </div>
                    </div>
                </TabsContent>

                <TabsContent value="recommendations">
                  <div className="p-4">
                      <h5 className="text-lg font-semibold mb-3">Recommendations</h5>
                      <div className="flex justify-between items-center mb-3">
                        <button type="button" className="inline-flex items-center px-3 py-1.5 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" onClick={addRecommendation}>
                          Add
                        </button>
                      </div>
                      <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                          <thead className="bg-gray-50 dark:bg-gray-800">
                            <tr>
                              <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-400">Name</th>
                              <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-400">URL</th>
                              <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-400">Type</th>
                            </tr>
                          </thead>
                          <tbody className="bg-white divide-y divide-gray-200 dark:bg-gray-900 dark:divide-gray-700">
                            {(selectedAilmentForModal.recommendations || []).map((rec, index) => (
                              <tr key={index}>
                                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100">
                                  <input
                                    type="text"
                                    className="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                                    value={rec.name || ''}
                                    onChange={(e) => handleRecommendationChange(index, 'name', e.target.value)}
                                  />
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                                  <input
                                    type="text"
                                    className="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                                    value={rec.url || ''}
                                    onChange={(e) => handleRecommendationChange(index, 'url', e.target.value)}
                                  />
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                                  <select
                                    className="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                                    value={rec.recommendationType}
                                    onChange={(e) => handleRecommendationChange(index, 'recommendationType', e.target.value)}
                                  >
                                    <option value={0}>Product</option>
                                    <option value={1}>Practitioner</option>
                                    <option value={2}>Book</option>
                                    <option value={3}>Food</option>
                                  </select>
                                </td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                    </div>
                  </TabsContent>
                </Tabs>

                {alertMessage && (
                  <div className="p-4 mb-4 text-sm text-green-800 rounded-lg bg-green-50 dark:bg-gray-800 dark:text-green-400" role="alert">
                    {alertMessage}
                  </div>
                )}
                {error && (
                  <div className="p-4 mb-4 text-sm text-red-800 rounded-lg bg-red-50 dark:bg-gray-800 dark:text-red-400" role="alert">
                    Error: {error}
                  </div>
                )}
                <button style={{
                  width: '100%',
                  padding: '12px 16px',
                  marginTop: '16px',
                  backgroundColor: '#0066cc',
                  color: '#fff',
                  border: 'none',
                  borderRadius: '6px',
                  fontSize: '14px',
                  fontWeight: '600',
                  cursor: 'pointer',
                  transition: 'background-color 0.2s'
                }} onClick={saveAilment}
                onMouseEnter={(e) => { e.currentTarget.style.backgroundColor = '#0052a3'; }}
                onMouseLeave={(e) => { e.currentTarget.style.backgroundColor = '#0066cc'; }}>
                  Save Changes
                </button>
              </div>
            </div>


            {/* Modal Footer */}
            <div style={{
              padding: '16px 24px',
              borderTop: '1px solid #e9ecef',
              display: 'flex',
              gap: '12px',
              justifyContent: 'flex-end'
            }}>
              <button
                type="button"
                onClick={handleCloseModal}
                style={{
                  padding: '10px 20px',
                  backgroundColor: '#f0f0f0',
                  color: '#1a1a1a',
                  border: '1px solid #d0d0d0',
                  borderRadius: '6px',
                  fontSize: '14px',
                  fontWeight: '600',
                  cursor: 'pointer',
                  transition: 'all 0.2s'
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.backgroundColor = '#e0e0e0';
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.backgroundColor = '#f0f0f0';
                }}
              >
                Close
              </button>
            </div>
          </div>
      )}
    </div>
  );
};

export default AilmentsTab;
