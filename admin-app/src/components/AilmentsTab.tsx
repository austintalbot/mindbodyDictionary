// admin-app/src/components/AilmentsTab.tsx
import React, { useEffect, useState } from 'react';
import { fetchMbdConditionsTable, upsertAilment, deleteAilment, fetchMbdCondition } from '../services/apiService';
import { MbdCondition, Recommendation } from '../types';
import { getImageBaseUrl } from '../constants'; // Import directly from constants

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
  const [activeModalTab, setActiveModalTab] = useState<string>('basicInfo');

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
    } catch (err: any) {
      setError(err.message || 'Failed to fetch ailments');
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
    } catch (err: any) {
      setError(err.message || 'Failed to fetch ailment details');
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
    } catch (err: any) {
      setError(err.message || 'Failed to save ailment');
    }
  };

  const deleteAilmentConfirm = async (id: string, name: string) => {
    if (window.confirm(`Are you sure you want to delete ${name}?`)) {
      try {
        await deleteAilment(id, name);
        loadAilments(); // Reload table after deletion
        addAilment(); // Reset form
      } catch (err: any) {
        setError(err.message || 'Failed to delete ailment');
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

  if (loading) return <div>Loading Ailments...</div>;
  if (error) return <div className="alert alert-danger">Error: {error}</div>;

  return (
    <div className="tab-pane fade show active" id="nav-ailments" role="tabpanel" aria-labelledby="nav-ailments-tab">
      <div className="card">
        <div className="card-body">
          <h5 className="card-title">Ailments</h5>
          <div className="mb-3">
            <input
              type="text"
              className="form-control"
              placeholder="Search by Name, Physical Connections, or Tags"
              value={searchTerm}
              onChange={handleSearchChange}
            />
          </div>
          <div className="table-responsive">
            <table className="table table-hover" style={{ width: '100%' }}>
              <thead>
                <tr>
                  <th>View</th>
                  <th>Delete</th>
                  <th>Name</th>
                  <th>Physical Connections</th>
                  <th>Tags</th>
                </tr>
              </thead>
              <tbody>
                {filteredAilments.map((ailment) => (
                  <tr key={ailment.id}>
                    <td>
                      <button
                        className="btn btn-outline-info"
                        onClick={() => selectAilment(ailment.id!, ailment.name!)}
                      >
                        <i className="fas fa-edit"></i> Edit
                      </button>
                    </td>
                    <td>
                      <button
                        className="btn btn-outline-dark"
                        onClick={() => deleteAilmentConfirm(ailment.id!, ailment.name!)}
                      >
                        <i className="fas fa-trash"></i>
                      </button>
                    </td>
                    <td>{ailment.name}</td>
                    <td>{(ailment.physicalConnections || []).join(', ')}</td>
                    <td>{(ailment.tags || []).join(', ')}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <button type="button" className="btn btn-sm btn-outline-primary" onClick={addAilment}>
            Add
          </button>
        </div>
      </div>

      {/* Ailment Detail Modal */}
      {selectedAilmentForModal && (
        <div className={`modal fade ${showDetailModal ? 'show d-block' : ''}`} tabIndex={-1} role="dialog" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-xl" role="document">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  {selectedAilmentForModal.id ? `Edit Ailment: ${selectedAilmentForModal.name}` : 'Add New Ailment'}
                </h5>
                <button type="button" className="close" aria-label="Close" onClick={handleCloseModal}>
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>
              <div className="modal-body">
                <ul className="nav nav-tabs" id="myTab" role="tablist">
                  <li className="nav-item">
                    <a
                      className={`nav-link ${activeModalTab === 'basicInfo' ? 'active' : ''}`}
                      onClick={() => setActiveModalTab('basicInfo')}
                      role="tab"
                    >
                      Basic Info
                    </a>
                  </li>
                  <li className="nav-item">
                    <a
                      className={`nav-link ${activeModalTab === 'affirmations' ? 'active' : ''}`}
                      onClick={() => setActiveModalTab('affirmations')}
                      role="tab"
                    >
                      Affirmations
                    </a>
                  </li>
                  <li className="nav-item">
                    <a
                      className={`nav-link ${activeModalTab === 'physicalConnections' ? 'active' : ''}`}
                      onClick={() => setActiveModalTab('physicalConnections')}
                      role="tab"
                    >
                      Physical Connections
                    </a>
                  </li>
                  <li className="nav-item">
                    <a
                      className={`nav-link ${activeModalTab === 'tags' ? 'active' : ''}`}
                      onClick={() => setActiveModalTab('tags')}
                      role="tab"
                    >
                      Tags
                    </a>
                  </li>
                  <li className="nav-item">
                    <a
                      className={`nav-link ${activeModalTab === 'recommendations' ? 'active' : ''}`}
                      onClick={() => setActiveModalTab('recommendations')}
                      role="tab"
                    >
                      Recommendations
                    </a>
                  </li>
                </ul>

                <div className="tab-content mt-3">
                  {/* Basic Info Tab Pane */}
                  {activeModalTab === 'basicInfo' && (
                    <div className="tab-pane fade show active" role="tabpanel">
                      <h5 className="card-title">Basic Info</h5>
                      <div className="row">
                        <div className="col-sm-6">
                          <div id="ailmentInfo" className="mb-3">
                            <div className="form-group">
                              <label htmlFor="id">Id:</label>
                              <input
                                type="text"
                                className="form-control"
                                id="id"
                                value={selectedAilmentForModal.id || ''}
                                disabled
                              />
                            </div>
                            <div className="form-group">
                              <label htmlFor="name">Name:</label>
                              <div className="input-group">
                                <input
                                  type="text"
                                  className="form-control"
                                  id="name"
                                  value={selectedAilmentForModal.name || ''}
                                  onChange={handleAilmentChange}
                                  disabled={!!selectedAilmentForModal.id}
                                  placeholder={selectedAilmentForModal.id ? '' : 'Input Ailment Name'}
                                />
                                <div className="input-group-append">
                                  <button className="btn btn-outline-warning" onClick={duplicateAilment} type="button">
                                    Duplicate Ailment
                                  </button>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                        <div id="ailmentImages" className="col-sm-6 text-center">
                          <i>Negative</i>
                          <img id="negativeImage" style={{ maxWidth: '120px' }} src={getImageUrl('negative')} alt="Negative Ailment" />
                          <i>Positive</i>
                          <img id="positiveImage" style={{ maxWidth: '120px' }} src={getImageUrl('positive')} alt="Positive Ailment" />
                        </div>
                      </div>
                      <div id="ailmentEdit">
                        <div className="form-check">
                          <input
                            type="checkbox"
                            className="form-check-input"
                            id="subscriptionOnly"
                            checked={selectedAilmentForModal.subscriptionOnly}
                            onChange={handleAilmentChange}
                          />
                          <label htmlFor="subscriptionOnly">Subscription Only</label>
                        </div>
                        <div className="form-group">
                          <label htmlFor="summaryNegative">Summary Negative:</label>
                          <textarea
                            className="form-control"
                            rows={5}
                            id="summaryNegative"
                            value={selectedAilmentForModal.summaryNegative || ''}
                            onChange={handleAilmentChange}
                          ></textarea>
                        </div>
                        <div className="form-group">
                          <label htmlFor="summaryPositive">Summary Positive:</label>
                          <textarea
                            className="form-control"
                            rows={5}
                            id="summaryPositive"
                            value={selectedAilmentForModal.summaryPositive || ''}
                            onChange={handleAilmentChange}
                          ></textarea>
                        </div>
                      </div>
                    </div>
                  )}

                  {/* Affirmations Tab Pane */}
                  {activeModalTab === 'affirmations' && (
                    <div className="tab-pane fade show active" role="tabpanel">
                      <div className="row">
                        <h5 className="col">Affirmations</h5>
                        <div className="col">
                          <button type="button" className="btn btn-sm btn-outline-primary" onClick={() => addToArray('affirmations')}>
                            Add
                          </button>
                        </div>
                      </div>
                      <i style={{ display: 'block' }}>Statements of Affirmation</i>
                      <hr />
                      <table style={{ width: '100%' }}>
                        <tbody>
                          {(selectedAilmentForModal.affirmations || []).map((affirmation, index) => (
                            <tr key={index}>
                              <td>
                                <textarea
                                  rows={2}
                                  className="sol-sm-12 form-control"
                                  value={affirmation}
                                  onChange={(e) => handleArrayChange(index, e.target.value, 'affirmations')}
                                ></textarea>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}

                  {/* Physical Connections Tab Pane */}
                  {activeModalTab === 'physicalConnections' && (
                    <div className="tab-pane fade show active" role="tabpanel">
                      <div className="row">
                        <h5 className="col">Physical Connections</h5>
                        <div className="col">
                          <button type="button" className="btn btn-sm btn-outline-primary" onClick={() => addToArray('physicalConnections')}>
                            Add
                          </button>
                        </div>
                      </div>
                      <i style={{ display: 'block' }}>Locations on the body that can be referenced by this Ailment i.e Adrenal Problems linked to Kidney</i>
                      <hr />
                      <table style={{ width: '100%' }}>
                        <tbody>
                          {(selectedAilmentForModal.physicalConnections || []).map((connection, index) => (
                            <tr key={index}>
                              <td>
                                <input
                                  type="text"
                                  className="sol-sm-12 form-control"
                                  value={connection}
                                  onChange={(e) => handleArrayChange(index, e.target.value, 'physicalConnections')}
                                />
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}

                  {/* Tags Tab Pane */}
                  {activeModalTab === 'tags' && (
                    <div className="tab-pane fade show active" role="tabpanel">
                      <div className="row">
                        <h5 className="col">Tags</h5>
                        <div className="col">
                          <button type="button" className="btn btn-sm btn-outline-primary" onClick={() => addToArray('tags')}>
                            Add
                          </button>
                        </div>
                      </div>
                      <i style={{ display: 'block' }}>Single words used for matching Ailment to search parameters</i>
                      <hr />
                      <table style={{ width: '100%' }}>
                        <tbody>
                          {(selectedAilmentForModal.tags || []).map((tag, index) => (
                            <tr key={index}>
                              <td>
                                <input
                                  type="text"
                                  className="sol-sm-12 form-control"
                                  value={tag}
                                  onChange={(e) => handleArrayChange(index, e.target.value, 'tags')}
                                />
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}

                  {/* Recommendations Tab Pane */}
                  {activeModalTab === 'recommendations' && (
                    <div className="tab-pane fade show active" role="tabpanel">
                      <div className="row">
                        <h5 className="col">Recommendations</h5>
                        <div className="col">
                          <button type="button" className="btn btn-sm btn-outline-primary" onClick={addRecommendation}>
                            Add
                          </button>
                        </div>
                      </div>
                      <table style={{ width: '100%' }}>
                        <thead>
                          <tr>
                            <th>Name</th>
                            <th>URL</th>
                            <th>Type</th>
                          </tr>
                        </thead>
                        <tbody>
                          {(selectedAilmentForModal.recommendations || []).map((rec, index) => (
                            <tr key={index}>
                              <td>
                                <input
                                  type="text"
                                  className="sol-sm-12 form-control rName"
                                  value={rec.name || ''}
                                  onChange={(e) => handleRecommendationChange(index, 'name', e.target.value)}
                                />
                              </td>
                              <td>
                                <input
                                  type="text"
                                  className="sol-sm-12 form-control rUrl"
                                  value={rec.url || ''}
                                  onChange={(e) => handleRecommendationChange(index, 'url', e.target.value)}
                                />
                              </td>
                              <td>
                                <select
                                  className="form-control rType"
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
                  )}
                </div>

                {alertMessage && (
                  <div className="alert alert-success mt-3" role="alert">
                    {alertMessage}
                  </div>
                )}
                {error && (
                  <div className="alert alert-danger mt-3" role="alert">
                    Error: {error}
                  </div>
                )}
                <button className="btn btn-primary btn-lg btn-block btn-success mt-3" onClick={saveAilment}>
                  Save
                </button>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={handleCloseModal}>Close</button>
                {/* Additional buttons can go here if needed */}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AilmentsTab;
