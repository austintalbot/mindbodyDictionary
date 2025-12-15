// admin-app/src/components/ImagesTab.tsx
import React, { useEffect, useState } from 'react';
import { fetchImagesTable, deleteImage, uploadImage, getImageBaseUrl, fetchAilmentsTable } from '../services/apiService';
import { MbdCondition } from '../types'; // Import MbdCondition

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
  const [images, setImages] = useState<Image[]>([]);
  const [ailmentOptions, setAilmentOptions] = useState<AilmentOption[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedImage, setSelectedImage] = useState<Image | null>(null);
  const [showAddImageDiv, setShowAddImageDiv] = useState(false);

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
    setError(null);
    try {
      const data = await fetchImagesTable();
      setImages(data);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch images');
    } finally {
      setLoading(false);
    }
  };

  const loadAilmentOptions = async () => {
    try {
        const data = await fetchAilmentsTable();
        // Assuming AilmentsTable returns objects with { id: string, name: string }
        setAilmentOptions(data.map((ailment: MbdCondition) => ({ id: ailment.id, name: ailment.name })));
    } catch (err: any) {
        // Handle error, maybe set an error state for the form
        console.error("Failed to load ailment options:", err);
    }
  };

  const selectImage = (image: Image) => {
    setSelectedImage(image);
    setShowAddImageDiv(false); // Hide add image form when selecting an image
  };

  const deleteImageConfirm = async (imageName: string) => {
    if (window.confirm(`Are you sure you want to delete ${imageName}?`)) {
      try {
        await deleteImage(imageName);
        loadImages(); // Reload table after deletion
        setSelectedImage(null); // Clear selected image
      } catch (err: any) {
        setError(err.message || 'Failed to delete image');
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
      setError(err.message || 'Failed to upload image');
    }
  };


  if (loading) return <div>Loading Images...</div>;
  if (error) return <div className="alert alert-danger">Error: {error}</div>;

  return (
    <div className="tab-pane fade show active" id="nav-images" role="tabpanel" aria-labelledby="nav-images-tab">
      <div className="card">
        <div className="card-body">
          <h5 className="card-title">Images</h5>
          <button className="btn btn-primary" onClick={loadImages}>Refresh Images</button> {/* Changed to Refresh */}
          <div id="imagesInternalDiv" className="mt-3">
            <table className="display" style={{ width: '100%' }}>
              <thead>
                <tr>
                  <th>View</th>
                  <th>Name</th>
                  <th>Default Ailment</th>
                  <th>Delete</th>
                </tr>
              </thead>
              <tbody>
                {images.map((image) => (
                  <tr key={image.name}>
                    <td>
                      <button className="btn btn-outline-info" onClick={() => selectImage(image)}>
                        <i className="fas fa-image"></i>
                      </button>
                    </td>
                    <td>{image.name}</td>
                    <td>{image.ailment}</td>
                    <td>
                      <button className="btn btn-outline-dark" onClick={() => deleteImageConfirm(image.name)}>
                        <i className="fas fa-trash"></i>
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            <button type="button" id="addImageButton" className="btn btn-sm btn-outline-primary" onClick={addImage}>
              Add
            </button>
          </div>
        </div>
        {selectedImage && (
          <div id="imageDiv" className="text-center mb-3">
            <div id="selectedImageName">{selectedImage.name}</div>
            <img id="selectedImage" style={{ maxWidth: '120px' }} src={getImageBaseUrl() + '/' + selectedImage.name} alt={selectedImage.name} />
          </div>
        )}
        {showAddImageDiv && (
          <div id="addImageDiv" className="row">
            <div className="offset-sm-4 col-sm-4">
              <div className="form-group">
                <select className="form-control" id="imageAilment" value={imageAilment} onChange={(e) => setImageAilment(e.target.value)}>
                  <option value="0">Select Ailment...</option>
                  {ailmentOptions.map(opt => <option key={opt.id} value={opt.id!}>{opt.name}</option>)}
                </select>
              </div>
              <div className="form-group">
                <select className="form-control" id="imageType" value={imageType} onChange={(e) => setImageType(e.target.value)}>
                  <option value="0">Select Image Type...</option>
                  <option value="1">Negative</option>
                  <option value="2">Positive</option>
                </select>
              </div>
              <div className="input-group mb-3">
                <div className="custom-file">
                  <input type="file" className="custom-file-input" id="imageFile" onChange={handleFileChange} />
                  <label className="custom-file-label" htmlFor="imageFile">{fileLabel}</label>
                </div>
              </div>
              <button className="btn btn-primary" onClick={submitImage}>Submit</button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ImagesTab;
