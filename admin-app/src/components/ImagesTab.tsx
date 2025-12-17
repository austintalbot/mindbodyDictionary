// admin-app/src/components/ImagesTab.tsx
import React, { useEffect, useState } from 'react';
import { fetchImagesTable, deleteImage, uploadImage, fetchMbdConditions, clearImagesCache } from '../services/apiService';
import { MbdCondition } from '../types';
import { getImageBaseUrl } from '../constants'; // Import directly from constants
import { useTheme } from '../theme/useTheme';
import ErrorModal from './ErrorModal'; // Import ErrorModal
import ImageActionModal from './ImageActionModal'; // Import ImageActionModal

interface Image {
  name: string;
  uri: string;
  ailment: string;
}

// AilmentOption now uses MbdCondition for name and id
interface AilmentOption {
    id?: string;
    name?: string;
}

const ImagesTab: React.FC = () => {
  const { colors } = useTheme();
  const [images, setImages] = useState<Image[]>([]);
  const [ailmentOptions, setAilmentOptions] = useState<AilmentOption[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedImage, setSelectedImage] = useState<Image | null>(null);
  const [showAddImageDiv, setShowAddImageDiv] = useState(false);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [showErrorModal, setShowErrorModal] = useState(false); // New state for error modal visibility
  const [modalErrorMessage, setModalErrorMessage] = useState(''); // New state for error modal message
  const [showImageActionModal, setShowImageActionModal] = useState(false); // New state for image action modal visibility
  const [selectedImageForAction, setSelectedImageForAction] = useState<Image | null>(null); // New state for image passed to action modal

  // Form states for adding image
  const [imageAilment, setImageAilment] = useState('0');
  const [imageType, setImageType] = useState('0'); // 1 for Negative, 2 for Positive
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [fileLabel, setFileLabel] = useState('Choose file');

  useEffect(() => {
    loadImages();
    loadAilmentOptions();
  }, []);

  const loadImages = async () => {
    setLoading(true);
    // setError(null); // No longer needed directly for rendering
    try {
      const response = await fetchImagesTable();
      // Handle the new response structure which is wrapped in a "data" property
      // and ensure we fallback to an empty array if data is missing.
      const imageData = (response as any).data || response;

      if (Array.isArray(imageData)) {
          setImages(imageData);
      } else {
          setImages([]); // Fallback to empty array if not an array
          console.warn('Unexpected response format for images:', response);
      }
    } catch (err: any) {
      setModalErrorMessage(err.message || 'Failed to fetch images'); // Set error message for modal
      setShowErrorModal(true); // Show error modal
    } finally {
      setLoading(false);
    }
  };

  const loadAilmentOptions = async () => {
    try {
        const response = await fetchMbdConditions(); // Changed to fetchMbdConditions
        if (response && Array.isArray(response)) {
            setAilmentOptions(response.map((ailment: MbdCondition) => ({ id: ailment.id, name: ailment.name })));
        } else {
            throw new Error('API response data for MbdConditions is not an array or is missing.');
        }
    } catch (err: any) {
        console.error("Failed to load ailment options:", err);
        setModalErrorMessage(err.message || "Failed to load ailment options"); // Set error message for modal
        setShowErrorModal(true); // Show error modal
    }
  };

  const selectImage = (image: Image) => {
    setSelectedImageForAction(image);
    setShowImageActionModal(true);
    setShowAddImageDiv(false); // Hide add image form when selecting an image
  };

  const deleteImageConfirm = async (imageName: string) => {
    if (window.confirm(`Are you sure you want to delete ${imageName}?`)) {
      try {
        await deleteImage(imageName);
        loadImages(); // Reload table after deletion
        // No longer need setSelectedImage(null) as inline image display is removed
      } catch (err: any) {
        setModalErrorMessage(err.message || 'Failed to delete image');
        setShowErrorModal(true);
      }
    }
  };



  const addImage = () => {
    setSelectedImage(null); // Clear selected image when adding new
    setShowAddImageDiv(true);
    // Reset form fields
    setImageAilment('0');
    setImageType('0');
    setImageFile(null);
    setFileLabel('Choose file');
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const file = e.target.files[0];
      if (file.size > 1000000) { // 1MB limit for example
        alert("File too large, please keep it below 1MB (1000KB)");
        setImageFile(null);
        setFileLabel('Please Select a new file');
        e.target.value = ''; // Clear input
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
    if (imageAilment === '0') {
      alert("Must Select an Ailment");
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
        // Need to get ailment name from id if imageAilment is an id, or use imageAilment directly if it's name
        const selectedAilment = ailmentOptions.find(opt => opt.id === imageAilment);
        const ailmentNameToUse = selectedAilment ? selectedAilment.name : imageAilment; // Fallback to id if name not found

        await uploadImage(ailmentNameToUse!, imageType, imageFile);
        alert("Image uploaded successfully!");
        setShowAddImageDiv(false);
        loadImages(); // Reload images table
      } catch (err: any) {
        setModalErrorMessage(err.message || 'Failed to upload image'); // Set error message for modal
        setShowErrorModal(true); // Show error modal
      }
  };

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(event.target.value);
  };

  const filteredImages = images.filter((image) => {
    const lowerCaseSearchTerm = searchTerm.toLowerCase();
    return (
      image.name.toLowerCase().includes(lowerCaseSearchTerm) ||
      image.ailment.toLowerCase().includes(lowerCaseSearchTerm)
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
            Refresh Images
          </button>
        </div>

        {/* Search input field */}
        <div style={{ marginBottom: '20px' }}>
            <input
                type="text"
                placeholder="Search images by name or ailment..."
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
                <th style={{ padding: '12px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Name</th>
                <th style={{ padding: '12px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Default Ailment</th>
                <th style={{ padding: '12px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Delete</th>
              </tr>
            </thead>
            <tbody>
              {filteredImages.map((image, index) => ( // Use filteredImages here
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
                  <td style={{ padding: '12px', color: colors.foreground }}>{image.name}</td>
                  <td style={{ padding: '12px', color: colors.foreground }}>{image.ailment}</td>
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

        <button
          onClick={addImage}
          style={{
            padding: '10px 20px',
            backgroundColor: colors.primary,
            color: '#fff',
            border: 'none',
            borderRadius: '6px',
            cursor: 'pointer',
            fontSize: '14px',
            fontWeight: '600',
            marginBottom: '20px',
            transition: 'all 0.2s'
          }}
          onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.primaryHover}
          onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.primary}
        >
          + Add New Image
        </button>



        {showAddImageDiv && (
          <div style={{
            maxWidth: '500px',
            margin: '0 auto',
            padding: '24px',
            backgroundColor: colors.backgroundSecondary,
            borderRadius: '8px',
            border: `1px solid ${colors.border}`
          }}>
            <h6 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '20px', color: colors.foreground }}>Upload New Image</h6>

            <div style={{ marginBottom: '16px' }}>
              <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Select Ailment</label>
              <select
                value={imageAilment}
                onChange={(e) => setImageAilment(e.target.value)}
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
                <option value="0">Select Ailment...</option>
                {ailmentOptions.map(opt => <option key={opt.id} value={opt.id!}>{opt.name}</option>)}
              </select>
            </div>

            <div style={{ marginBottom: '16px' }}>
              <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Image Type</label>
              <select
                value={imageType}
                onChange={(e) => setImageType(e.target.value)}
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
                <option value="1">Negative</option>
                <option value="2">Positive</option>
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

            <button
              onClick={submitImage}
              style={{
                width: '100%',
                padding: '12px',
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
        )}
      </div>
      {showImageActionModal && selectedImageForAction && (
        <ImageActionModal
          isOpen={showImageActionModal}
          onClose={() => {
            setShowImageActionModal(false);
            setSelectedImageForAction(null); // Clear selected image
          }}
          image={selectedImageForAction}
          onImageDeleted={() => {
            setShowImageActionModal(false);
            setSelectedImageForAction(null); // Clear selected image
            loadImages(); // Reload table after deletion
          }}
          onImageUploaded={() => {
            setShowImageActionModal(false);
            setSelectedImageForAction(null); // Clear selected image
            loadImages(); // Reload table after upload
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
