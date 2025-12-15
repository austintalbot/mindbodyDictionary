// admin-app/src/components/AilmentsTab.tsx
import React, { useEffect, useState } from 'react';
import { fetchMbdConditionsTable, upsertAilment, deleteAilment, fetchMbdCondition } from '../services/apiService';
import { MbdCondition, Recommendation } from '../types';
import { getImageBaseUrl } from '../constants'; // Import directly from constants

// Interface for what Ailment data looks like, extends MbdCondition for additional properties if any
interface Ailment extends MbdCondition {}

const AilmentsTab: React.FC = () => {
  const [ailments, setAilments] = useState<Ailment[]>([]);
  const [currentAilment, setCurrentAilment] = useState<Ailment | null>(null);
  const [showAilmentDiv, setShowAilmentDiv] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [alertMessage, setAlertMessage] = useState<string | null>(null);

  useEffect(() => {
    loadAilments();
  }, []);

  const loadAilments = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetchMbdConditionsTable(); // Changed to fetchMbdConditionsTable
      if (response && Array.isArray(response)) { // Check if response.data exists and is an array
        setAilments(response);
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
    setShowAilmentDiv(true);
    setAlertMessage(null);
    try {
      const data = await fetchMbdCondition(id, name); // Changed to fetchMbdCondition
      setCurrentAilment(data);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch ailment details');
    }
  };

  const addAilment = () => {
    setCurrentAilment({
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
    setShowAilmentDiv(true);
    setAlertMessage(null);
    setError(null);
  };

  const duplicateAilment = () => {
    if (currentAilment) {
      setCurrentAilment({
        ...currentAilment,
        id: '', // Clear ID for duplication
        name: '', // Clear name to prompt new name
      });
    }
  };

  const saveAilment = async () => {
    if (!currentAilment || !currentAilment.name || currentAilment.name.trim() === '') {
      alert('Must specify an Ailment name');
      return;
    }

    try {
      const savedAilment = await upsertAilment(currentAilment);
      setCurrentAilment(savedAilment);
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
    if (currentAilment) {
      const { id, value, type, checked } = e.target as HTMLInputElement;
      setCurrentAilment((prev) => ({
        ...prev!,
        [id]: type === 'checkbox' ? checked : value,
      }));
    }
  };

  const handleArrayChange = (index: number, value: string, arrayType: 'affirmations' | 'physicalConnections' | 'tags') => {
    if (currentAilment && currentAilment[arrayType]) {
      const newArray = [...currentAilment[arrayType]!];
      newArray[index] = value;
      setCurrentAilment((prev) => ({
        ...prev!,
        [arrayType]: newArray,
      }));
    }
  };

  const addToArray = (arrayType: 'affirmations' | 'physicalConnections' | 'tags') => {
    if (currentAilment) {
      setCurrentAilment((prev) => ({
        ...prev!,
        [arrayType]: [...(prev![arrayType] || []), ''],
      }));
    }
  };

  const handleRecommendationChange = (index: number, field: keyof Recommendation, value: string) => {
    if (currentAilment && currentAilment.recommendations) {
      const newRecommendations = [...currentAilment.recommendations];
      newRecommendations[index] = {
        ...newRecommendations[index],
        [field]: field === 'recommendationType' ? parseInt(value) : value,
      };
      setCurrentAilment((prev) => ({
        ...prev!,
        recommendations: newRecommendations,
      }));
    }
  };

  const addRecommendation = () => {
    if (currentAilment) {
      setCurrentAilment((prev) => ({
        ...prev!,
        recommendations: [...(prev!.recommendations || []), { name: '', url: '', recommendationType: 0 }],
      }));
    }
  };


  const getImageUrl = (type: 'negative' | 'positive') => {
    if (!currentAilment) return '';
    const baseUrl = getImageBaseUrl();
    const ailmentName = currentAilment.name;
    if (!ailmentName) return '';
    return `${baseUrl}/${ailmentName}${type === 'negative' ? '1' : '2'}.png`;
  };


  if (loading) return <div>Loading Ailments...</div>;
  if (error) return <div className="alert alert-danger">Error: {error}</div>;

  return (
    <div className="tab-pane fade show active" id="nav-ailments" role="tabpanel" aria-labelledby="nav-ailments-tab">
      <div className="card">
        <div className="card-body">
          <h5 className="card-title">Ailments</h5>
          <table className="display" style={{ width: '100%' }}>
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
              {ailments.map((ailment) => (
                <tr key={ailment.id}>
                  <td>
                    <button
                      className="btn btn-outline-info"
                      onClick={() => selectAilment(ailment.id!, ailment.name!)}
                    >
                      <i className="fas fa-file-alt"></i>
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
          <button type="button" className="btn btn-sm btn-outline-primary" onClick={addAilment}>
            Add
          </button>
        </div>
      </div>

      {showAilmentDiv && currentAilment && (
        <div className="card mt-3" id="ailmentDiv">
          <div className="card-body">
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
                      value={currentAilment.id || ''}
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
                        value={currentAilment.name || ''}
                        onChange={handleAilmentChange}
                        disabled={!!currentAilment.id} // Disable name if ID exists (editing existing)
                        placeholder={currentAilment.id ? '' : 'Input Ailment Name'}
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
                  checked={currentAilment.subscriptionOnly}
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
                  value={currentAilment.summaryNegative || ''}
                  onChange={handleAilmentChange}
                ></textarea>
              </div>
              <div className="form-group">
                <label htmlFor="summaryPositive">Summary Positive:</label>
                <textarea
                  className="form-control"
                  rows={5}
                  id="summaryPositive"
                  value={currentAilment.summaryPositive || ''}
                  onChange={handleAilmentChange}
                ></textarea>
              </div>

              <div className="row mt-3">
                <div className="col-sm">
                  <div>
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
                        {(currentAilment.affirmations || []).map((affirmation, index) => (
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
                </div>
                <div className="col-sm">
                  <div>
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
                        {(currentAilment.physicalConnections || []).map((connection, index) => (
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
                </div>
                <div className="col-sm">
                  <div>
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
                        {(currentAilment.tags || []).map((tag, index) => (
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
                </div>
              </div>

              <div className="col-sm-12 mt-3">
                <div>
                  <div className="row">
                    <h5 className="">Recommendations</h5>
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
                      {(currentAilment.recommendations || []).map((rec, index) => (
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
          </div>
        </div>
      )}
    </div>
  );
};

export default AilmentsTab;
