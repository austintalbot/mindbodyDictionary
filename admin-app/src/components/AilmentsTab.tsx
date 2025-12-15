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
                <Tabs defaultValue="basicInfo" className="w-full">
                  <TabsList className="grid w-full grid-cols-5">
                    <TabsTrigger value="basicInfo">Basic Info</TabsTrigger>
                    <TabsTrigger value="affirmations">Affirmations</TabsTrigger>
                    <TabsTrigger value="physicalConnections">Physical Connections</TabsTrigger>
                    <TabsTrigger value="tags">Tags</TabsTrigger>
                    <TabsTrigger value="recommendations">Recommendations</TabsTrigger>
                  </TabsList>

                  <TabsContent value="basicInfo">
                    <div className="p-4">
                      <h5 className="text-lg font-semibold mb-3">Basic Info</h5>
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                          <div id="ailmentInfo" className="mb-3">
                            <div className="mb-3">
                              <label htmlFor="id" className="block text-sm font-medium text-gray-700 dark:text-gray-300">Id:</label>
                              <input
                                type="text"
                                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                                id="id"
                                value={selectedAilmentForModal.id || ''}
                                disabled
                              />
                            </div>
                            <div className="mb-3">
                              <label htmlFor="name" className="block text-sm font-medium text-gray-700 dark:text-gray-300">Name:</label>
                              <div className="flex">
                                <input
                                  type="text"
                                  className="mt-1 block w-full rounded-l-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                                  id="name"
                                  value={selectedAilmentForModal.name || ''}
                                  onChange={handleAilmentChange}
                                  disabled={!!selectedAilmentForModal.id}
                                  placeholder={selectedAilmentForModal.id ? '' : 'Input Ailment Name'}
                                />
                                <button className="inline-flex items-center px-4 py-2 border border-l-0 border-gray-300 rounded-r-md bg-yellow-500 text-white shadow-sm hover:bg-yellow-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-yellow-500 dark:bg-yellow-600 dark:hover:bg-yellow-700" onClick={duplicateAilment} type="button">
                                  Duplicate
                                </button>
                              </div>
                            </div>
                          </div>
                        </div>
                        <div id="ailmentImages" className="text-center">
                          <i className="block text-sm font-medium text-gray-700 dark:text-gray-300">Negative</i>
                          <img id="negativeImage" className="max-w-[120px] mx-auto my-2" src={getImageUrl('negative')} alt="Negative Ailment" />
                          <i className="block text-sm font-medium text-gray-700 dark:text-gray-300">Positive</i>
                          <img id="positiveImage" className="max-w-[120px] mx-auto my-2" src={getImageUrl('positive')} alt="Positive Ailment" />
                        </div>
                      </div>
                      <div id="ailmentEdit" className="mt-4">
                        <div className="flex items-center mb-3">
                          <input
                            type="checkbox"
                            className="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300 rounded dark:bg-gray-700 dark:border-gray-600"
                            id="subscriptionOnly"
                            checked={selectedAilmentForModal.subscriptionOnly}
                            onChange={handleAilmentChange}
                          />
                          <label htmlFor="subscriptionOnly" className="ml-2 block text-sm text-gray-900 dark:text-gray-100">Subscription Only</label>
                        </div>
                        <div className="mb-3">
                          <label htmlFor="summaryNegative" className="block text-sm font-medium text-gray-700 dark:text-gray-300">Summary Negative:</label>
                          <textarea
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                            rows={5}
                            id="summaryNegative"
                            value={selectedAilmentForModal.summaryNegative || ''}
                            onChange={handleAilmentChange}
                          ></textarea>
                        </div>
                        <div className="mb-3">
                          <label htmlFor="summaryPositive" className="block text-sm font-medium text-gray-700 dark:text-gray-300">Summary Positive:</label>
                          <textarea
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                            rows={5}
                            id="summaryPositive"
                            value={selectedAilmentForModal.summaryPositive || ''}
                            onChange={handleAilmentChange}
                          ></textarea>
                        </div>
                      </div>
                    </div>
                  </TabsContent>

                  <TabsContent value="affirmations">
                    <div className="p-4">
                      <h5 className="text-lg font-semibold mb-3">Affirmations</h5>
                      <div className="flex justify-between items-center mb-3">
                        <i className="block text-sm font-medium text-gray-700 dark:text-gray-300">Statements of Affirmation</i>
                        <button type="button" className="inline-flex items-center px-3 py-1.5 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" onClick={() => addToArray('affirmations')}>
                          Add
                        </button>
                      </div>
                      <div className="space-y-2">
                        {(selectedAilmentForModal.affirmations || []).map((affirmation, index) => (
                          <textarea
                            key={index}
                            rows={2}
                            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white"
                            value={affirmation}
                            onChange={(e) => handleArrayChange(index, e.target.value, 'affirmations')}
                          ></textarea>
                        ))}
                      </div>
                    </div>
                  </TabsContent>

                  <TabsContent value="physicalConnections">
                    <div className="p-4">
                      <h5 className="text-lg font-semibold mb-3">Physical Connections</h5>
                      <div className="flex justify-between items-center mb-3">
                        <i className="block text-sm font-medium text-gray-700 dark:text-gray-300">Locations on the body that can be referenced by this Ailment i.e Adrenal Problems linked to Kidney</i>
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
                <button className="w-full inline-flex items-center justify-center px-4 py-2 border border-transparent text-base font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 mt-4" onClick={saveAilment}>
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
