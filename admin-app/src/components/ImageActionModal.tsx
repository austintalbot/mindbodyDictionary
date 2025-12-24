// admin-app/src/components/ImageActionModal.tsx
import React, { useState, useEffect } from 'react';
import { useTheme } from '../theme/useTheme';
import { Image, ImageType } from '../types'; // Assuming Image interface is in types.ts now
import { deleteImage, uploadImage, fetchMbdCondition, upsertMbdCondition } from '../services/apiService'; // Re-import delete/upload
import { getImageBaseUrl } from '../constants';

interface ImageActionModalProps {
  isOpen: boolean;
  onClose: () => void;
  image: Image | null;
  mbdConditionOptions?: { id?: string; name?: string }[];
  onImageDeleted: () => void; // Callback to refresh images after deletion
  onImageUploaded: () => void; // Callback to refresh images after upload
  initialImageType?: ImageType; // Optional context
}

const ImageActionModal: React.FC<ImageActionModalProps> = ({
  isOpen,
  onClose,
  image,
  mbdConditionOptions = [],
  onImageDeleted,
  onImageUploaded,
  initialImageType
}) => {
  const { colors } = useTheme();
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [fileLabel, setFileLabel] = useState('Choose new image file');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [previewImageUrl, setPreviewImageUrl] = useState<string | null>(null); // New state for image preview

  // State for changing association
  const [selectedConditionId, setSelectedConditionId] = useState<string>('');
  const [selectedImageType, setSelectedImageType] = useState<ImageType | undefined>(undefined);

  useEffect(() => {
    if (!isOpen) {
      // Reset form when modal closes
      setImageFile(null);
      setFileLabel('Choose new image file');
      setError(null);
      setLoading(false);
      setSelectedConditionId('');
      setSelectedImageType(undefined);
      if (previewImageUrl) {
        URL.revokeObjectURL(previewImageUrl); // Clean up the old preview URL
        setPreviewImageUrl(null);
      }
    } else if (image) {
       // Try to pre-fill the association form based on current image name/condition
       const parsed = parseImageNameForUpload(image.name);
       
       // Use explicit initial type if provided, otherwise parsed
       const typeToUse = initialImageType || parsed.imageType;
       setSelectedImageType(typeToUse);

       // Pre-select condition if possible
       if (image.mbdCondition) {
           const found = mbdConditionOptions.find(opt => opt.name === image.mbdCondition);
           if (found && found.id) {
               setSelectedConditionId(found.id);
           }
       }
    }
  }, [isOpen, previewImageUrl, image, mbdConditionOptions, initialImageType]);

  // Clean up object URL on component unmount
  useEffect(() => {
    return () => {
      if (previewImageUrl) {
        URL.revokeObjectURL(previewImageUrl);
      }
    };
  }, [previewImageUrl]);

  if (!isOpen || !image) return null;

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const file = e.target.files[0];
      if (file.size > 1000000) { // 1MB limit for example
        alert("File too large, please keep it below 1MB (1000KB)");
        setImageFile(null);
        setFileLabel('Please Select a new file');
        e.target.value = ''; // Clear input
        if (previewImageUrl) {
          URL.revokeObjectURL(previewImageUrl);
          setPreviewImageUrl(null);
        }
      } else {
        setImageFile(file);
        setFileLabel(file.name);
        if (previewImageUrl) { // Revoke previous URL if any
          URL.revokeObjectURL(previewImageUrl);
        }
        setPreviewImageUrl(URL.createObjectURL(file)); // Create new preview URL
      }
    } else {
      setImageFile(null);
      setFileLabel('Choose new image file');
      if (previewImageUrl) {
        URL.revokeObjectURL(previewImageUrl);
        setPreviewImageUrl(null);
      }
    }
  };

  const parseImageNameForUpload = (imageName: string) => {
    // Remove extension
    let nameWithoutExtension = imageName.split('.').slice(0, -1).join('.');

    // Check for timestamp suffix (e.g. _1234567890) and remove it if present
    const timestampRegex = /_\d+$/;
    if (timestampRegex.test(nameWithoutExtension)) {
        nameWithoutExtension = nameWithoutExtension.replace(timestampRegex, '');
    }

    let mbdConditionName = '';
    let imageType: ImageType | undefined = undefined;

    if (nameWithoutExtension.endsWith('Negative')) {
      mbdConditionName = nameWithoutExtension.replace('Negative', '');
      imageType = ImageType.Negative;
    } else if (nameWithoutExtension.endsWith('Positive')) {
      mbdConditionName = nameWithoutExtension.replace('Positive', '');
      imageType = ImageType.Positive;
    } else {
      // Fallback
      mbdConditionName = nameWithoutExtension;
      imageType = ImageType.Positive; // Default
    }
    return { mbdConditionName, imageType };
  };


  const handleImageUpload = async () => {
    if (!imageFile) {
      alert("Please select a file to upload.");
      return;
    }

    setLoading(true);
    setError(null);

    try {
      // Parse details from filename
      const parsed = parseImageNameForUpload(image.name);
      let { mbdConditionName, imageType } = parsed;
      
      // Override with explicit context if available
      if (initialImageType) {
          imageType = initialImageType;
      }

      // If the image object has an explicit condition association, use that name
      if (image.mbdCondition) {
          mbdConditionName = image.mbdCondition;
      }

      // Ensure we have a valid type
      if (!imageType) {
          // Fallback if parsing failed and no context provided
          imageType = ImageType.Positive; 
      }

      // Upload the new image using the constant naming convention
      // This will overwrite the existing blob in Azure Storage.
      // Since filenames are constant (<ConditionName><Type>.png), we don't need to update the MbdCondition record.
      // The backend 'UpsertMbdImage' updates the global 'LastUpdatedTime', which triggers the mobile app
      // to force-refresh all images.
      await uploadImage(mbdConditionName, imageType, imageFile);

      alert("Image replaced successfully!");
      onImageUploaded(); // Refresh image list
      onClose(); // Close modal
    } catch (err: any) {
      setError(err.message || 'Failed to replace image.');
    } finally {
      setLoading(false);
    }
  };

  const handleMoveImage = async () => {
      if (!selectedConditionId || !selectedImageType) {
          alert("Please select both a condition and an image type.");
          return;
      }

      setLoading(true);
      setError(null);

      try {
          const selectedCondition = mbdConditionOptions.find(opt => opt.id === selectedConditionId);
          if (!selectedCondition || !selectedCondition.name) {
              throw new Error("Invalid condition selected.");
          }

          // 1. Download the current image as a Blob
          const imageUrl = getImageBaseUrl() + '/' + image.name;
          const response = await fetch(imageUrl);
          if (!response.ok) {
              throw new Error(`Failed to fetch original image: ${response.statusText}`);
          }
          const blob = await response.blob();

          // 2. Create a File object from the Blob
          const file = new File([blob], image.name, { type: blob.type });

          // 3. Upload the image with the NEW name (derived from new condition and type)
          // The uploadImage function enforces the naming convention: <conditionName><Type>.png
          await uploadImage(selectedCondition.name, selectedImageType, file);

          // 4. Update the NEW condition record to point to the new standard filename
          try {
              const condition = await fetchMbdCondition(selectedCondition.id!, selectedCondition.name);
              if (condition) {
                  const suffix = selectedImageType === ImageType.Negative ? 'Negative' : 'Positive';
                  const newFileName = `${selectedCondition.name}${suffix}.png`;
                  
                  if (selectedImageType === ImageType.Negative) {
                      condition.imageNegative = newFileName;
                  } else {
                      condition.imagePositive = newFileName;
                  }
                  await upsertMbdCondition(condition);
              }
          } catch (e) {
              console.warn("Failed to update condition after move", e);
          }

          // 5. Delete the OLD image
          await deleteImage(image.name);

          alert("Image re-associated successfully!");
          onImageUploaded(); // Refresh
          onClose();

      } catch (err: any) {
          setError(err.message || 'Failed to re-associate image.');
      } finally {
          setLoading(false);
      }
  };

  const handleImageDelete = async () => {
    if (!window.confirm(`Are you sure you want to delete ${image.name}? This action cannot be undone.`)) {
      return;
    }

    setLoading(true);
    setError(null);

    try {
      await deleteImage(image.name);
      alert("Image deleted successfully!");
      onImageDeleted(); // Refresh image list
      onClose(); // Close modal
    } catch (err: any) {
      setError(err.message || 'Failed to delete image.');
    } finally {
      setLoading(false);
    }
  };

  const handleDownloadImage = async () => {
    setLoading(true);
    setError(null);
    try {
      const imageUrl = getImageBaseUrl() + '/' + image.name;
      const response = await fetch(imageUrl);
      if (!response.ok) {
        throw new Error(`Failed to fetch image: ${response.status} ${response.statusText}`);
      }
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', image.name);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url); // Clean up the object URL
    } catch (err: any) {
      setError(err.message || 'Failed to download image.');
    } finally {
      setLoading(false);
    }
  };

  return (
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
        maxWidth: '600px',
        maxHeight: '90vh',
        display: 'flex',
        flexDirection: 'column',
        boxShadow: `0 10px 40px ${colors.shadowHeavy}`
      }}>
        {/* Modal Header */}
        <div style={{
          padding: '20px 24px',
          borderBottom: `1px solid ${colors.border}`,
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center'
        }}>
          <h2 style={{ margin: 0, fontSize: '20px', fontWeight: '700', color: colors.foreground }}>
            Image Actions: {image.name}
          </h2>
          <button
            onClick={onClose}
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
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px', display: 'flex', flexDirection: 'column', gap: '20px' }}>
          {error && (
            <div style={{ padding: '12px', backgroundColor: colors.dangerLight, color: colors.danger, borderRadius: '4px', marginBottom: '16px' }}>
              Error: {error}
            </div>
          )}

          {/* Current Image Display */}
          <div style={{ textAlign: 'center' }}>
            <img
              src={getImageBaseUrl() + '/' + image.name}
              alt={image.name}
              style={{ maxWidth: '100%', maxHeight: '300px', objectFit: 'contain', border: `1px solid ${colors.border}`, borderRadius: '4px', backgroundColor: 'whitesmoke', padding: '10px' }}
            />
            <div style={{ marginTop: '10px', fontSize: '14px', color: colors.foreground }}>
                <strong>Associated Condition:</strong> {image.mbdCondition || 'None'}
            </div>
          </div>

          {/* Download Button */}
          <button
            onClick={handleDownloadImage}
            disabled={loading}
            style={{
              width: '100%',
              padding: '12px',
              backgroundColor: colors.primary,
              color: '#fff',
              border: 'none',
              borderRadius: '6px',
              cursor: loading ? 'not-allowed' : 'pointer',
              fontSize: '14px',
              fontWeight: '600',
              transition: 'all 0.2s',
              opacity: loading ? 0.7 : 1,
            }}
            onMouseEnter={(e) => { if (!loading) e.currentTarget.style.backgroundColor = colors.primaryHover; }}
            onMouseLeave={(e) => { if (!loading) e.currentTarget.style.backgroundColor = colors.primary; }}
          >
            {loading ? 'Processing...' : 'Download Current Image'}
          </button>

          {/* Change Association Section */}
          <div style={{ borderTop: `1px solid ${colors.border}`, paddingTop: '20px', marginTop: '20px' }}>
              <h6 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '15px', color: colors.foreground }}>Change Association (Rename/Move)</h6>
              <div style={{ display: 'flex', gap: '10px', marginBottom: '15px' }}>
                  <div style={{ flex: 2 }}>
                      <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>New Condition</label>
                      <select
                          value={selectedConditionId}
                          onChange={(e) => setSelectedConditionId(e.target.value)}
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
                          <option value="">Select Condition...</option>
                          {mbdConditionOptions.map(opt => (
                              <option key={opt.id} value={opt.id!}>{opt.name}</option>
                          ))}
                      </select>
                  </div>
                  <div style={{ flex: 1 }}>
                      <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Type</label>
                      <select
                          value={selectedImageType || ''}
                          onChange={(e) => setSelectedImageType(e.target.value as ImageType)}
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
                          <option value="">Select...</option>
                          <option value={ImageType.Negative}>Negative</option>
                          <option value={ImageType.Positive}>Positive</option>
                      </select>
                  </div>
              </div>
              <button
                  onClick={handleMoveImage}
                  disabled={loading || !selectedConditionId || !selectedImageType}
                  style={{
                      width: '100%',
                      padding: '12px',
                      backgroundColor: colors.accent,
                      color: '#fff',
                      border: 'none',
                      borderRadius: '6px',
                      cursor: (loading || !selectedConditionId || !selectedImageType) ? 'not-allowed' : 'pointer',
                      fontSize: '14px',
                      fontWeight: '600',
                      transition: 'all 0.2s',
                      opacity: (loading || !selectedConditionId || !selectedImageType) ? 0.7 : 1,
                  }}
                  onMouseEnter={(e) => { if (!loading && selectedConditionId && selectedImageType) e.currentTarget.style.backgroundColor = colors.accentHover; }}
                  onMouseLeave={(e) => { if (!loading && selectedConditionId && selectedImageType) e.currentTarget.style.backgroundColor = colors.accent; }}
              >
                  {loading ? 'Moving...' : 'Update Association'}
              </button>
          </div>

          {/* Image Upload Form */}
          <div style={{ borderTop: `1px solid ${colors.border}`, paddingTop: '20px', marginTop: '20px' }}>
            <h6 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '15px', color: colors.foreground }}>Replace Image File (Same Name)</h6>
            {previewImageUrl && (
              <div style={{ textAlign: 'center', marginBottom: '15px' }}>
                <img
                  src={previewImageUrl}
                  alt="New Image Preview"
                  style={{ maxWidth: '100%', maxHeight: '200px', objectFit: 'contain', border: `1px solid ${colors.border}`, borderRadius: '4px', backgroundColor: 'whitesmoke', padding: '10px' }}
                />
                <p style={{ fontSize: '12px', color: colors.mutedText, marginTop: '8px' }}>Preview of new image</p>
              </div>
            )}
            <div style={{ marginBottom: '15px' }}>
              <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.lightText }}>Image File</label>
              <div style={{ position: 'relative', overflow: 'hidden', display: 'inline-block', width: '100%' }}>
                <input
                  type="file"
                  id="newImageFile"
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
              onClick={handleImageUpload}
              disabled={loading || !imageFile}
              style={{
                width: '100%',
                padding: '12px',
                backgroundColor: colors.accent,
                color: '#fff',
                border: 'none',
                borderRadius: '6px',
                cursor: (loading || !imageFile) ? 'not-allowed' : 'pointer',
                fontSize: '14px',
                fontWeight: '600',
                transition: 'all 0.2s',
                opacity: (loading || !imageFile) ? 0.7 : 1,
              }}
              onMouseEnter={(e) => { if (!loading && imageFile) e.currentTarget.style.backgroundColor = colors.accentHover; }}
              onMouseLeave={(e) => { if (!loading && imageFile) e.currentTarget.style.backgroundColor = colors.accent; }}
            >
              {loading ? 'Uploading...' : 'Upload & Replace Image'}
            </button>
          </div>

          {/* Delete Button */}
          <div style={{ borderTop: `1px solid ${colors.border}`, paddingTop: '20px', marginTop: '20px', textAlign: 'center' }}>
            <button
              onClick={handleImageDelete}
              disabled={loading}
              style={{
                width: '100%',
                maxWidth: '200px', // Smaller delete button
                padding: '10px 15px',
                backgroundColor: colors.danger,
                color: '#fff',
                border: 'none',
                borderRadius: '6px',
                cursor: loading ? 'not-allowed' : 'pointer',
                fontSize: '14px',
                fontWeight: '600',
                transition: 'all 0.2s',
                opacity: loading ? 0.7 : 1,
              }}
              onMouseEnter={(e) => { if (!loading) e.currentTarget.style.backgroundColor = colors.dangerHover; }}
              onMouseLeave={(e) => { if (!loading) e.currentTarget.style.backgroundColor = colors.danger; }}
            >
              {loading ? 'Deleting...' : 'Delete Image'}
            </button>
          </div>

        </div>

        {/* Modal Footer (optional, can add more actions here) */}
        <div style={{
          padding: '16px 24px',
          borderTop: `1px solid ${colors.border}`,
          display: 'flex',
          justifyContent: 'flex-end'
        }}>
          <button
            onClick={onClose}
            disabled={loading}
            style={{
              padding: '8px 20px',
              backgroundColor: colors.backgroundSecondary,
              color: colors.foreground,
              border: 'none',
              borderRadius: '6px',
              cursor: loading ? 'not-allowed' : 'pointer',
              fontSize: '14px',
              fontWeight: '600',
              transition: 'all 0.2s',
              opacity: loading ? 0.7 : 1,
            }}
            onMouseEnter={(e) => { if (!loading) e.currentTarget.style.backgroundColor = colors.neutralHover; }}
            onMouseLeave={(e) => { if (!loading) e.currentTarget.style.backgroundColor = colors.backgroundSecondary; }}
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

export default ImageActionModal;
