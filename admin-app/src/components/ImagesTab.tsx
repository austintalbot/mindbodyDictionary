// admin-app/src/components/ImagesTab.tsx
import React, { useEffect, useState } from 'react';
import { fetchImagesTable, deleteImage, uploadImage, fetchMbdConditions, clearImagesCache } from '../services/apiService';
import { MbdCondition, Image, ImageType } from '../types';
import { useTheme } from '../theme/useTheme';
import ErrorModal from './ErrorModal';
import ImageActionModal from './ImageActionModal';

// MbdConditionOption now uses MbdCondition for name and id
interface MbdConditionOption {
    id?: string;
    name?: string;
}

const ImagesTab: React.FC = () => {
  const { colors } = useTheme();
  const [images, setImages] = useState<Image[]>([]);
  const [mbdConditionOptions, setMbdConditionOptions] = useState<MbdConditionOption[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAddImageDiv, setShowAddImageDiv] = useState(false);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [showErrorModal, setShowErrorModal] = useState(false);
  const [modalErrorMessage, setModalErrorMessage] = useState('');
  const [showImageActionModal, setShowImageActionModal] = useState(false);
  const [selectedImageForAction, setSelectedImageForAction] = useState<Image | null>(null);

  // Form states for adding image
  const [imageMbdCondition, setImageMbdCondition] = useState('0');
  const [imageType, setImageType] = useState<ImageType | '0'>('0');
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [fileLabel, setFileLabel] = useState('Choose file');

  useEffect(() => {
    loadImages();
    loadMbdConditionOptions();
  }, []);

  const loadImages = async () => {
    setLoading(true);
    try {
      const response = await fetchImagesTable();
      const imageData = (response as any).data || response;

      if (Array.isArray(imageData)) {
          setImages(imageData);
      } else {
          setImages([]);
          console.warn('Unexpected response format for images:', response);
      }
    } catch (err: any) {
      setModalErrorMessage(err.message || 'Failed to fetch images');
      setShowErrorModal(true);
    } finally {
      setLoading(false);
    }
  };

  const loadMbdConditionOptions = async () => {
    try {
        const response = await fetchMbdConditions();
        if (response && Array.isArray(response)) {
            setMbdConditionOptions(response.map((mbdCondition: MbdCondition) => ({ id: mbdCondition.id, name: mbdCondition.name })));
        } else {
            throw new Error('API response data for MbdConditions is not an array or is missing.');
        }
    } catch (err: any) {
        console.error("Failed to load condition options:", err);
        setModalErrorMessage(err.message || "Failed to load condition options");
        setShowErrorModal(true);
    }
  };

  const selectImage = (image: Image) => {
    setSelectedImageForAction(image);
    setShowImageActionModal(true);
    setShowAddImageDiv(false);
  };

  const deleteImageConfirm = async (imageName: string) => {
    if (window.confirm(`Are you sure you want to delete ${imageName}?`)) {
      try {
        await deleteImage(imageName);
        loadImages();
      } catch (err: any) {
        setModalErrorMessage(err.message || 'Failed to delete image');
        setShowErrorModal(true);
      }
    }
  };

  const addImage = () => {
    setShowAddImageDiv(true);
    setImageMbdCondition('0');
    setImageType('0');
    setImageFile(null);
    setFileLabel('Choose file');
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const file = e.target.files[0];
      if (file.size > 1000000) {
        alert("File too large, please keep it below 1MB (1000KB)");
        setImageFile(null);
        setFileLabel('Please Select a new file');
        e.target.value = '';
      } else {
        setImageFile(file);
        setFileLabel(file.name);
      }
    } else {
      setImageFile(null);
      setFileLabel('Choose file');
    }
  };

  const submitImage = async () => {
    if (imageMbdCondition === '0') {
      alert("Must Select a Condition");
      return;
    }
    if (imageType === '0') {
      alert("Must Select Image Type");
      return;
    }
    if (!imageFile) {
      alert("Must select a file");
      return;
    }

    try {
        const selectedMbdCondition = mbdConditionOptions.find(opt => opt.id === imageMbdCondition);
        const conditionNameToUse = selectedMbdCondition ? selectedMbdCondition.name : imageMbdCondition;

        await uploadImage(conditionNameToUse!, imageType as ImageType, imageFile);
        alert("Image uploaded successfully!");
        setShowAddImageDiv(false);
        loadImages();
      } catch (err: any) {
        setModalErrorMessage(err.message || 'Failed to upload image');
        setShowErrorModal(true);
      }
  };

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(event.target.value);
  };

  const filteredImages = images.filter((image) => {
    const lowerCaseSearchTerm = searchTerm.toLowerCase();
    return (
      image.name.toLowerCase().includes(lowerCaseSearchTerm) ||
      (image.mbdCondition && image.mbdCondition.toLowerCase().includes(lowerCaseSearchTerm))
    );
  });


  if (loading) return <div style={{ padding: '20px', color: colors.mutedText }}>Loading Images...</div>;


  return (
    <div style={{ padding: '20px' }}>
      <div style={{
        backgroundColor: colors.background,
        borderRadius: '8px',
        border: `1px solid ${colors.border}`,
        padding: '24px'
      }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
          <h5 style={{ fontSize: '18px', fontWeight: '600', margin: 0, color: colors.foreground }}>Images</h5>
          <div style={{ display: 'flex', gap: '12px' }}>
            <button
              onClick={() => { clearImagesCache(); loadImages(); }}
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
              Refresh
            </button>
            <button
              onClick={addImage}
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
              + Add
            </button>
          </div>
        </div>

        {/* Search input field */}
        <div style={{ marginBottom: '20px' }}>
            <input
                type="text"
                placeholder="Search images by name or condition..."
                value={searchTerm}
                onChange={handleSearchChange}
                style={{
                    width: '100%',
                    padding: '10px 12px',
                    fontSize: '14px',
                    border: `1px solid ${colors.inputBorder}`,
                    borderRadius: '6px',
                    backgroundColor: colors.inputBackground,
                    color: colors.foreground,
                    outline: 'none',
                }}
            />
        </div>
        <div style={{ marginBottom: '15px', fontSize: '14px', color: colors.mutedText }}>
            Total Images: {filteredImages.length}
        </div>

        <div style={{ overflowX: 'auto', marginBottom: '20px' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
            <thead>
              <tr style={{ backgroundColor: colors.backgroundSecondary, borderBottom: `2px solid ${colors.border}` }}>
                <th style={{ padding: '12px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>View</th>
                <th style={{ padding: '12px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Thumbnail</th>
                <th style={{ padding: '12px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Name</th>
                <th style={{ padding: '12px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Default Condition</th>
                <th style={{ padding: '12px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Delete</th>
              </tr>
            </thead>
            <tbody>
              {filteredImages.map((image, index) => (
                <tr
                  key={image.name}
                  style={{
                    backgroundColor: index % 2 === 0 ? colors.background : colors.backgroundSecondary,
                    borderBottom: `1px solid ${colors.border}`,
                    transition: 'background-color 0.15s',
                  }}
                  onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.border}
                  onMouseLeave={(e) => e.currentTarget.style.backgroundColor = index % 2 === 0 ? colors.background : colors.backgroundSecondary}
                >
                  <td style={{ padding: '12px' }}>
                    <button
                      onClick={() => selectImage(image)}
                      style={{
                        padding: '6px 10px',
                        backgroundColor: colors.primaryLight,
                        color: colors.primary,
                        border: `1px solid ${colors.primary}`,
                        borderRadius: '4px',
                        cursor: 'pointer',
                        fontSize: '12px',
                        fontWeight: '600',
                        transition: 'all 0.2s'
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = colors.primary;
                        e.currentTarget.style.color = '#fff';
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = colors.primaryLight;
                        e.currentTarget.style.color = colors.primary;
                      }}
                    >
                      View
                    </button>
                  </td>
                  <td style={{ padding: '12px' }}>
                    {image.uri ? (
                        <img
                            src={image.uri}
                            alt={image.name}
                            style={{
                                width: '50px',
                                height: '50px',
                                objectFit: 'contain',
                                borderRadius: '4px',
                                border: `1px solid ${colors.border}`,
                                backgroundColor: 'whitesmoke'
                            }}
                        />
                    ) : (
                        <div style={{
                            width: '50px',
                            height: '50px',
                            borderRadius: '4px',
                            border: `1px solid ${colors.border}`,
                            backgroundColor: 'whitesmoke',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            fontSize: '10px',
                            color: colors.mutedText
                        }}>No Img</div>
                    )}
                  </td>
                  <td style={{ padding: '12px', color: colors.foreground }}>{image.name}</td>
                  <td style={{ padding: '12px', color: colors.foreground }}>{image.mbdCondition}</td>
                  <td style={{ padding: '12px' }}>
                    <button
                      onClick={() => deleteImageConfirm(image.name)}
                      style={{
                        padding: '6px 10px',
                        backgroundColor: colors.dangerLight,
                        color: colors.danger,
                        border: `1px solid ${colors.danger}`,
                        borderRadius: '4px',
                        cursor: 'pointer',
                        fontSize: '12px',
                        fontWeight: '600',
                        transition: 'all 0.2s'
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = colors.danger;
                        e.currentTarget.style.color = '#fff';
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = colors.dangerLight;
                        e.currentTarget.style.color = colors.danger;
                      }}
                    >
                      Delete
                    </button>
                  </td>

                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {showAddImageDiv && (
          <div style={{
            display: 'flex',
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: colors.shadow,
            zIndex: 1050,
            alignItems: 'center',
            justifyContent: 'center',
            padding: '20px'
          }}>
            <div style={{
              backgroundColor: colors.background,
              borderRadius: '8px',
              width: '100%',
              maxWidth: '500px',
              display: 'flex',
              flexDirection: 'column',
              boxShadow: `0 10px 40px ${colors.shadowHeavy}`
            }}>
              {/* Modal Header */}
              <div style={{
                padding: '24px',
                borderBottom: `1px solid ${colors.border}`,
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
              }}>
                <h2 style={{ margin: 0, fontSize: '20px', fontWeight: '700', color: colors.foreground }}>Upload New Image</h2>
                <button
                  onClick={() => setShowAddImageDiv(false)}
                  style={{
                    background: 'none',
                    border: 'none',
                    fontSize: '24px',
                    color: colors.mutedText,
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
                    e.currentTarget.style.backgroundColor = colors.neutral;
                    e.currentTarget.style.color = colors.foreground;
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.backgroundColor = 'transparent';
                    e.currentTarget.style.color = colors.mutedText;
                  }}
                >
                  ✕
                </button>
              </div>

              {/* Modal Body */}
              <div style={{ padding: '24px', overflowY: 'auto', maxHeight: '70vh' }}>
                <div style={{ marginBottom: '16px' }}>
                  <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Select Condition</label>
                  <select
                    value={imageMbdCondition}
                    onChange={(e) => setImageMbdCondition(e.target.value)}
                    style={{
                      width: '100%',
                      padding: '10px 12px',
                      fontSize: '14px',
                      border: `1px solid ${colors.inputBorder}`,
                      borderRadius: '6px',
                      backgroundColor: colors.inputBackground,
                      color: colors.foreground,
                      outline: 'none',
                      cursor: 'pointer'
                    }}
                  >
                    <option value="0">Select Condition...</option>
                    {mbdConditionOptions.map(opt => <option key={opt.id} value={opt.id!}>{opt.name}</option>)}
                  </select>
                </div>

                <div style={{ marginBottom: '16px' }}>
                  <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Image Type</label>
                  <select
                    value={imageType}
                    onChange={(e) => setImageType(e.target.value as ImageType)}
                    style={{
                      width: '100%',
                      padding: '10px 12px',
                      fontSize: '14px',
                      border: `1px solid ${colors.inputBorder}`,
                      borderRadius: '6px',
                      backgroundColor: colors.inputBackground,
                      color: colors.foreground,
                      outline: 'none',
                      cursor: 'pointer'
                    }}
                  >
                    <option value="0">Select Image Type...</option>
                    <option value={ImageType.Negative}>Negative</option>
                    <option value={ImageType.Positive}>Positive</option>
                  </select>
                </div>

                <div style={{ marginBottom: '24px' }}>
                  <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Image File</label>
                  <div style={{ position: 'relative', overflow: 'hidden', display: 'inline-block', width: '100%' }}>
                    <input
                      type="file"
                      id="imageFile"
                      onChange={handleFileChange}
                      style={{
                        position: 'absolute',
                        left: 0,
                        top: 0,
                        opacity: 0,
                        width: '100%',
                        height: '100%',
                        cursor: 'pointer'
                      }}
                    />
                    <div style={{
                      padding: '10px 12px',
                      fontSize: '14px',
                      border: `1px solid ${colors.inputBorder}`,
                      borderRadius: '6px',
                      backgroundColor: colors.inputBackground,
                      color: colors.mutedText,
                      whiteSpace: 'nowrap',
                      overflow: 'hidden',
                      textOverflow: 'ellipsis'
                    }}>
                      {fileLabel}
                    </div>
                  </div>
                </div>
              </div>

              {/* Modal Footer */}
              <div style={{
                padding: '20px 24px',
                borderTop: `1px solid ${colors.border}`,
                display: 'flex',
                justifyContent: 'flex-end',
                gap: '12px'
              }}>
                <button
                  onClick={() => setShowAddImageDiv(false)}
                  style={{
                    padding: '10px 24px',
                    backgroundColor: colors.neutral,
                    color: colors.foreground,
                    border: 'none',
                    borderRadius: '6px',
                    cursor: 'pointer',
                    fontSize: '14px',
                    fontWeight: '600',
                    transition: 'all 0.2s'
                  }}
                  onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.border}
                  onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.neutral}
                >
                  Cancel
                </button>
                <button
                  onClick={submitImage}
                  style={{
                    padding: '10px 24px',
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
                  Upload Image
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
      {showImageActionModal && selectedImageForAction && (
        <ImageActionModal
          isOpen={showImageActionModal}
          onClose={() => {
            setShowImageActionModal(false);
            setSelectedImageForAction(null);
          }}
          image={selectedImageForAction}
          mbdConditionOptions={mbdConditionOptions}
          onImageDeleted={() => {
            setShowImageActionModal(false);
            setSelectedImageForAction(null);
            loadImages();
          }}
          onImageUploaded={() => {
            setShowImageActionModal(false);
            setSelectedImageForAction(null);
            loadImages();
          }}
        />
      )}
      {showErrorModal && (
        <ErrorModal
          message={modalErrorMessage}
          onClose={() => setShowErrorModal(false)}
        />
      )}
    </div>
  );
};

export default ImagesTab;
