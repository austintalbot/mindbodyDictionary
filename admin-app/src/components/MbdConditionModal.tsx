import React from 'react';
import { Tabs, TabsList, TabsTrigger, TabsContent } from './components/ui/tabs';
import { useTheme } from '../theme/useTheme';
import { MbdCondition, Recommendation, RecommendationType, Image } from '../types';
import ImageActionModal from './ImageActionModal';

interface MbdConditionModalProps {
  isOpen: boolean;
  mbdCondition: MbdCondition | null;
  onClose: () => void;
  onSave: () => void;
  onChange: (mbdCondition: MbdCondition) => void;
  getImageUrl: (type: 'negative' | 'positive') => string;
}

const MbdConditionModal: React.FC<MbdConditionModalProps> = ({
  isOpen,
  mbdCondition,
  onClose,
  onSave,
  onChange,
  getImageUrl,
}) => {
  const { colors } = useTheme();
  const [showImageActionModal, setShowImageActionModal] = React.useState(false);
  const [selectedImageForAction, setSelectedImageForAction] = React.useState<Image | null>(null);

  if (!mbdCondition) return null;
  console.log("Condition data in modal:", mbdCondition);

  const handleImageClick = (imageName: string | undefined) => {
    if (!imageName) return;
    const imageObj: Image = {
      name: imageName,
    };
    setSelectedImageForAction(imageObj);
    setShowImageActionModal(true);
  };

  // Log image URLs for debugging
  console.log("Image URL (negative):", getImageUrl('negative'));
  console.log("Image URL (positive):", getImageUrl('positive'));

  return (
    <div style={{
      display: isOpen ? 'flex' : 'none',
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
        maxWidth: '900px',
        maxHeight: '90vh',
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
              <h2 style={{ margin: 0, fontSize: '20px', fontWeight: '700', color: colors.foreground }}>
                {mbdCondition?.id ? `Edit: ${mbdCondition.name}` : 'Add New Condition'}
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
        <div style={{ flex: 1, overflowY: 'auto', padding: '32px' }}>
          <Tabs defaultValue="basicInfo" className="w-full">
            <TabsList className="grid w-full grid-cols-5" style={{
              marginBottom: '20px',
              borderBottom: `2px solid ${colors.border}`,
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
                <h5 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '20px', color: colors.foreground }}>Basic Information</h5>
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px' }}>
                  <div>
                    <div style={{ marginBottom: '20px' }}>
                      <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.mutedText }} htmlFor="id">ID</label>
                      <input
                        type="text"
                        id="id"
                        disabled
                        value={mbdCondition.id || ''}
                        style={{
                          width: '100%',
                          padding: '10px 12px',
                          fontSize: '14px',
                          border: `1px solid ${colors.border}`,
                          borderRadius: '6px',
                          backgroundColor: colors.backgroundSecondary,
                          color: colors.mutedText,
                          cursor: 'not-allowed'
                        }}
                      />
                    </div>
                    <div>
                      <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.mutedText }} htmlFor="name">Name</label>
                      <input
                        type="text"
                        id="name"
                        value={mbdCondition.name}
                        onChange={(e) => onChange({ ...mbdCondition, name: e.target.value })}
                        style={{
                          width: '100%',
                          padding: '10px 12px',
                          fontSize: '14px',
                          border: `1px solid ${colors.inputBorder}`,
                          borderRadius: '6px',
                          transition: 'all 0.2s',
                          backgroundColor: colors.inputBackground,
                          color: colors.foreground
                        }}
                        onFocus={(e) => {
                          e.currentTarget.style.borderColor = colors.inputFocus;
                          e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                        }}
                        onBlur={(e) => {
                          e.currentTarget.style.borderColor = colors.inputBorder;
                          e.currentTarget.style.boxShadow = 'none';
                        }}
                      />
                    </div>
                  </div>
                  <div>
                    <div style={{ marginBottom: '20px' }}>
                      <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.mutedText }} htmlFor="imageNegative">Image Negative</label>
                      <input
                        type="text"
                        id="imageNegative"
                        value={mbdCondition.imageNegative || ''}
                        onChange={(e) => onChange({ ...mbdCondition, imageNegative: e.target.value })}
                        style={{
                          width: '100%',
                          padding: '10px 12px',
                          fontSize: '14px',
                          border: `1px solid ${colors.inputBorder}`,
                          borderRadius: '6px',
                          transition: 'all 0.2s',
                          backgroundColor: colors.inputBackground,
                          color: colors.foreground
                        }}
                        onFocus={(e) => {
                          e.currentTarget.style.borderColor = colors.inputFocus;
                          e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                        }}
                        onBlur={(e) => {
                          e.currentTarget.style.borderColor = colors.inputBorder;
                          e.currentTarget.style.boxShadow = 'none';
                        }}
                      />
                    </div>
                    <div>
                      <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.mutedText }} htmlFor="imagePositive">Image Positive</label>
                      <input
                        type="text"
                        id="imagePositive"
                        value={mbdCondition.imagePositive || ''}
                        onChange={(e) => onChange({ ...mbdCondition, imagePositive: e.target.value })}
                        style={{
                          width: '100%',
                          padding: '10px 12px',
                          fontSize: '14px',
                          border: `1px solid ${colors.inputBorder}`,
                          borderRadius: '6px',
                          transition: 'all 0.2s',
                          backgroundColor: colors.inputBackground,
                          color: colors.foreground
                        }}
                        onFocus={(e) => {
                          e.currentTarget.style.borderColor = colors.inputFocus;
                          e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                        }}
                        onBlur={(e) => {
                          e.currentTarget.style.borderColor = colors.inputBorder;
                          e.currentTarget.style.boxShadow = 'none';
                        }}
                      />
                    </div>
                  </div>
                </div>
                <div style={{ marginTop: '24px', borderTop: `1px solid ${colors.border}`, paddingTop: '20px' }}>
                  <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.mutedText }} htmlFor="summaryNegative">Summary Negative</label>
                  <textarea
                    id="summaryNegative"
                    value={mbdCondition.summaryNegative}
                    onChange={(e) => onChange({ ...mbdCondition, summaryNegative: e.target.value })}
                    style={{
                      width: '100%',
                      padding: '10px 12px',
                      fontSize: '14px',
                      border: `1px solid ${colors.inputBorder}`,
                      borderRadius: '6px',
                      minHeight: '100px',
                      resize: 'vertical',
                      fontFamily: 'inherit',
                      transition: 'all 0.2s',
                      backgroundColor: colors.inputBackground,
                      color: colors.foreground
                    }}
                    onFocus={(e) => {
                      e.currentTarget.style.borderColor = colors.inputFocus;
                      e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                    }}
                    onBlur={(e) => {
                      e.currentTarget.style.borderColor = colors.inputBorder;
                      e.currentTarget.style.boxShadow = 'none';
                    }}
                  />
                </div>
                <div style={{ marginTop: '24px', borderTop: `1px solid ${colors.border}`, paddingTop: '20px' }}>
                  <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: colors.mutedText }} htmlFor="summaryPositive">Summary Positive</label>
                  <textarea
                    id="summaryPositive"
                    value={mbdCondition.summaryPositive}
                    onChange={(e) => onChange({ ...mbdCondition, summaryPositive: e.target.value })}
                    style={{
                      width: '100%',
                      padding: '10px 12px',
                      fontSize: '14px',
                      border: `1px solid ${colors.inputBorder}`,
                      borderRadius: '6px',
                      minHeight: '100px',
                      resize: 'vertical',
                      fontFamily: 'inherit',
                      transition: 'all 0.2s',
                      backgroundColor: colors.inputBackground,
                      color: colors.foreground
                    }}
                    onFocus={(e) => {
                      e.currentTarget.style.borderColor = colors.inputFocus;
                      e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                    }}
                    onBlur={(e) => {
                      e.currentTarget.style.borderColor = colors.inputBorder;
                      e.currentTarget.style.boxShadow = 'none';
                    }}
                  />
                </div>
                <div style={{ marginTop: '32px', display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', padding: '32px', borderTop: `1px solid ${colors.border}` }}>
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '8px' }}>
                    <div 
                      onClick={() => handleImageClick(mbdCondition.imageNegative)}
                      style={{ cursor: 'pointer', width: '100%', position: 'relative' }}
                      title="Click to manage image"
                    >
                      <img src={getImageUrl('negative')} alt="Negative" style={{ width: '100%',margin: "10px", maxHeight: '250px', objectFit: 'contain', borderRadius: '6px', backgroundColor: 'whitesmoke', padding: '12px', border: `1px solid ${colors.border}` }} />
                    </div>
                    <p style={{ fontSize: '12px', color: colors.mutedText, margin: 0 }}>Negative</p>
                  </div>
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '8px' }}>
                    <div 
                      onClick={() => handleImageClick(mbdCondition.imagePositive)}
                      style={{ cursor: 'pointer', width: '100%', position: 'relative' }}
                      title="Click to manage image"
                    >
                      <img src={getImageUrl('positive')} alt="Positive" style={{ width: '100%',margin: "10px", maxHeight: '250px', objectFit: 'contain', borderRadius: '6px', backgroundColor: 'whitesmoke', padding: '12px', border: `1px solid ${colors.border}` }} />
                    </div>
                    <p style={{ fontSize: '12px', color: colors.mutedText, margin: 0 }}>Positive</p>
                  </div>
                </div>
              </div>
            </TabsContent>

            <TabsContent value="affirmations">
              <div style={{ padding: '20px' }}>
                <h5 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '20px', color: colors.foreground }}>Affirmations</h5>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                  {(mbdCondition.affirmations || []).map((aff: string, idx: number) => (
                    <div key={idx} style={{ display: 'flex', gap: '8px' }}>
                      <textarea
                        value={aff}
                        onChange={(e) => {
                          const newAffirmations = [...(mbdCondition.affirmations || [])];
                          newAffirmations[idx] = e.target.value;
                          onChange({ ...mbdCondition, affirmations: newAffirmations });
                        }}
                        style={{
                          flex: 1,
                          padding: '10px 12px',
                          fontSize: '14px',
                          border: `1px solid ${colors.inputBorder}`,
                          borderRadius: '6px',
                          minHeight: '60px',
                          fontFamily: 'inherit',
                          transition: 'all 0.2s',
                          backgroundColor: colors.inputBackground,
                          color: colors.foreground
                        }}
                        onFocus={(e) => {
                          e.currentTarget.style.borderColor = colors.inputFocus;
                          e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                        }}
                        onBlur={(e) => {
                          e.currentTarget.style.borderColor = colors.inputBorder;
                          e.currentTarget.style.boxShadow = 'none';
                        }}
                      />
                      <button
                        onClick={() => {
                          const newAffirmations = mbdCondition.affirmations?.filter((_: string, i: number) => i !== idx) || [];
                          onChange({ ...mbdCondition, affirmations: newAffirmations });
                        }}
                        style={{
                          padding: '8px 12px',
                          backgroundColor: colors.danger,
                          color: colors.buttonText,
                          border: 'none',
                          borderRadius: '6px',
                          cursor: 'pointer',
                          fontSize: '14px',
                          fontWeight: '600',
                          transition: 'all 0.2s',
                          height: 'fit-content',
                          marginTop: '10px'
                        }}
                        onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.dangerHover}
                        onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.danger}
                      >
                        Remove
                      </button>
                    </div>
                  ))}
                  <button
                    onClick={() => {
                      const newAffirmations = [...(mbdCondition.affirmations || []), ''];
                      onChange({ ...mbdCondition, affirmations: newAffirmations });
                    }}
                    style={{
                      padding: '10px 20px',
                      backgroundColor: colors.primary,
                      color: colors.buttonText,
                      border: 'none',
                      borderRadius: '6px',
                      cursor: 'pointer',
                      fontSize: '14px',
                      fontWeight: '600',
                      transition: 'all 0.2s',
                      marginTop: '12px'
                    }}
                    onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.primaryHover}
                    onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.primary}
                  >
                    + Add Affirmation
                  </button>
                </div>
              </div>
            </TabsContent>

            <TabsContent value="physicalConnections">
              <div style={{ padding: '20px' }}>
                <h5 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '20px', color: colors.foreground }}>Physical Connections</h5>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                  {(mbdCondition.physicalConnections || []).map((conn: string, idx: number) => (
                    <div key={idx} style={{ display: 'flex', gap: '8px' }}>
                      <input
                        type="text"
                        value={conn}
                        onChange={(e) => {
                          const newConnections = [...(mbdCondition.physicalConnections || [])];
                          newConnections[idx] = e.target.value;
                          onChange({ ...mbdCondition, physicalConnections: newConnections });
                        }}
                        style={{
                          flex: 1,
                          padding: '10px 12px',
                          fontSize: '14px',
                          border: `1px solid ${colors.inputBorder}`,
                          borderRadius: '6px',
                          transition: 'all 0.2s',
                          backgroundColor: colors.inputBackground,
                          color: colors.foreground
                        }}
                        onFocus={(e) => {
                          e.currentTarget.style.borderColor = colors.inputFocus;
                          e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                        }}
                        onBlur={(e) => {
                          e.currentTarget.style.borderColor = colors.inputBorder;
                          e.currentTarget.style.boxShadow = 'none';
                        }}
                      />
                      <button
                        onClick={() => {
                          const newConnections = mbdCondition.physicalConnections?.filter((_: string, i: number) => i !== idx) || [];
                          onChange({ ...mbdCondition, physicalConnections: newConnections });
                        }}
                        style={{
                          padding: '8px 12px',
                          backgroundColor: colors.danger,
                          color: colors.buttonText,
                          border: 'none',
                          borderRadius: '6px',
                          cursor: 'pointer',
                          fontSize: '14px',
                          fontWeight: '600',
                          transition: 'all 0.2s'
                        }}
                        onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.dangerHover}
                        onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.danger}
                      >
                        Remove
                      </button>
                    </div>
                  ))}
                  <button
                    onClick={() => {
                      const newConnections = [...(mbdCondition.physicalConnections || []), ''];
                      onChange({ ...mbdCondition, physicalConnections: newConnections });
                    }}
                    style={{
                      padding: '10px 20px',
                      backgroundColor: colors.primary,
                      color: colors.buttonText,
                      border: 'none',
                      borderRadius: '6px',
                      cursor: 'pointer',
                      fontSize: '14px',
                      fontWeight: '600',
                      transition: 'all 0.2s',
                      marginTop: '12px'
                    }}
                    onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.primaryHover}
                    onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.primary}
                  >
                    + Add Connection
                  </button>
                </div>
              </div>
            </TabsContent>

            <TabsContent value="tags">
              <div style={{ padding: '20px' }}>
                <h5 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '20px', color: colors.foreground }}>Tags</h5>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                  {(mbdCondition.tags || []).map((tag: string, idx: number) => (
                    <div key={idx} style={{ display: 'flex', gap: '8px' }}>
                      <input
                        type="text"
                        value={tag}
                        onChange={(e) => {
                          const newTags = [...(mbdCondition.tags || [])];
                          newTags[idx] = e.target.value;
                          onChange({ ...mbdCondition, tags: newTags });
                        }}
                        style={{
                          flex: 1,
                          padding: '10px 12px',
                          fontSize: '14px',
                          border: `1px solid ${colors.inputBorder}`,
                          borderRadius: '6px',
                          transition: 'all 0.2s',
                          backgroundColor: colors.inputBackground,
                          color: colors.foreground
                        }}
                        onFocus={(e) => {
                          e.currentTarget.style.borderColor = colors.inputFocus;
                          e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                        }}
                        onBlur={(e) => {
                          e.currentTarget.style.borderColor = colors.inputBorder;
                          e.currentTarget.style.boxShadow = 'none';
                        }}
                      />
                      <button
                        onClick={() => {
                          const newTags = mbdCondition.tags?.filter((_: string, i: number) => i !== idx) || [];
                          onChange({ ...mbdCondition, tags: newTags });
                        }}
                        style={{
                          padding: '8px 12px',
                          backgroundColor: colors.danger,
                          color: colors.buttonText,
                          border: 'none',
                          borderRadius: '6px',
                          cursor: 'pointer',
                          fontSize: '14px',
                          fontWeight: '600',
                          transition: 'all 0.2s'
                        }}
                        onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.dangerHover}
                        onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.danger}
                      >
                        Remove
                      </button>
                    </div>
                  ))}
                  <button
                    onClick={() => {
                      const newTags = [...(mbdCondition.tags || []), ''];
                      onChange({ ...mbdCondition, tags: newTags });
                    }}
                    style={{
                      padding: '10px 20px',
                      backgroundColor: colors.primary,
                      color: colors.buttonText,
                      border: 'none',
                      borderRadius: '6px',
                      cursor: 'pointer',
                      fontSize: '14px',
                      fontWeight: '600',
                      transition: 'all 0.2s',
                      marginTop: '12px'
                    }}
                    onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.primaryHover}
                    onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.primary}
                  >
                    + Add Tag
                  </button>
                </div>
              </div>
            </TabsContent>

            <TabsContent value="recommendations">
              <div style={{ padding: '20px' }}>
                <h5 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '20px', color: colors.foreground }}>Recommendations</h5>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                  {(mbdCondition.recommendations || []).map((rec: Recommendation, idx: number) => (
                    <div key={idx} style={{ display: 'flex', flexDirection: 'column', gap: '8px', padding: '12px', backgroundColor: colors.backgroundSecondary, borderRadius: '6px' }}>
                      <div style={{ display: 'flex', gap: '8px' }}>
                        <textarea
                          value={typeof rec === 'string' ? rec : rec?.name || ''}
                          onChange={(e) => {
                            const newRecommendations = [...(mbdCondition.recommendations || [])];
                            if (typeof rec === 'string') {
                              newRecommendations[idx] = { name: e.target.value, url: '', recommendationType: 0 } as Recommendation;
                            } else {
                              newRecommendations[idx] = { ...rec, name: e.target.value };
                            }
                            onChange({ ...mbdCondition, recommendations: newRecommendations });
                          }}
                          placeholder="Recommendation name"
                          style={{
                            flex: 1,
                            padding: '10px 12px',
                            fontSize: '14px',
                            border: `1px solid ${colors.inputBorder}`,
                            borderRadius: '6px',
                            minHeight: '60px',
                            fontFamily: 'inherit',
                            transition: 'all 0.2s',
                            backgroundColor: colors.inputBackground,
                            color: colors.foreground
                          }}
                          onFocus={(e) => {
                            e.currentTarget.style.borderColor = colors.inputFocus;
                            e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                          }}
                          onBlur={(e) => {
                            e.currentTarget.style.borderColor = colors.inputBorder;
                            e.currentTarget.style.boxShadow = 'none';
                          }}
                        />
                        <button
                          onClick={() => {
                            const newRecommendations = mbdCondition.recommendations?.filter((_: Recommendation, i: number) => i !== idx) || [];
                            onChange({ ...mbdCondition, recommendations: newRecommendations });
                          }}
                          style={{
                            padding: '8px 12px',
                            backgroundColor: colors.danger,
                            color: colors.buttonText,
                            border: 'none',
                            borderRadius: '6px',
                            cursor: 'pointer',
                            fontSize: '14px',
                            fontWeight: '600',
                            transition: 'all 0.2s',
                            height: 'fit-content',
                            marginTop: '10px'
                          }}
                          onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.dangerHover}
                          onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.danger}
                        >
                          Remove
                        </button>
                      </div>
                      <select
                        value={typeof rec === 'string' ? RecommendationType.Product : rec?.recommendationType || RecommendationType.Product}
                        onChange={(e) => {
                          const newRecommendations = [...(mbdCondition.recommendations || [])];
                          const typeValue = parseInt(e.target.value, 10);
                          if (typeof rec === 'string') {
                            newRecommendations[idx] = { name: '', url: '', recommendationType: typeValue } as Recommendation;
                          } else {
                            newRecommendations[idx] = { ...rec, recommendationType: typeValue };
                          }
                          onChange({ ...mbdCondition, recommendations: newRecommendations });
                        }}
                        style={{
                          padding: '8px 12px',
                          fontSize: '14px',
                          border: `1px solid ${colors.inputBorder}`,
                          borderRadius: '6px',
                          backgroundColor: colors.inputBackground,
                          color: colors.foreground,
                          transition: 'all 0.2s',
                          cursor: 'pointer'
                        }}
                        onFocus={(e) => {
                          e.currentTarget.style.borderColor = colors.inputFocus;
                          e.currentTarget.style.boxShadow = `0 0 0 3px ${colors.inputFocusRing}`;
                        }}
                        onBlur={(e) => {
                          e.currentTarget.style.borderColor = colors.inputBorder;
                          e.currentTarget.style.boxShadow = 'none';
                        }}
                      >
                        <option value={RecommendationType.Product}>Supplement / Product</option>
                        <option value={RecommendationType.Book}>Book / Resource</option>
                        <option value={RecommendationType.Food}>Food</option>
                      </select>
                    </div>
                  ))}
                  <button
                    onClick={() => {
                      const newRecommendations = [...(mbdCondition.recommendations || []), { name: '', url: '', recommendationType: 0 }];
                      onChange({ ...mbdCondition, recommendations: newRecommendations });
                    }}
                    style={{
                      padding: '10px 20px',
                      backgroundColor: colors.primary,
                      color: colors.buttonText,
                      border: 'none',
                      borderRadius: '6px',
                      cursor: 'pointer',
                      fontSize: '14px',
                      fontWeight: '600',
                      transition: 'all 0.2s',
                      marginTop: '12px'
                    }}
                    onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.primaryHover}
                    onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.primary}
                  >
                    + Add Recommendation
                  </button>
                </div>
              </div>
            </TabsContent>
          </Tabs>
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
            onClick={onClose}
            style={{
              padding: '10px 24px',
              backgroundColor: colors.backgroundSecondary,
              color: colors.foreground,
              border: 'none',
              borderRadius: '6px',
              cursor: 'pointer',
              fontSize: '14px',
              fontWeight: '600',
              transition: 'all 0.2s'
            }}
            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = colors.neutralHover}
            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = colors.backgroundSecondary}
          >
            Cancel
          </button>
          <button
            onClick={onSave}
            style={{
              padding: '10px 24px',
              backgroundColor: colors.primary,
              color: colors.buttonText,
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
            Save Condition
          </button>
        </div>
      </div>
      {showImageActionModal && selectedImageForAction && (
        <ImageActionModal
          isOpen={showImageActionModal}
          onClose={() => {
            setShowImageActionModal(false);
            setSelectedImageForAction(null);
          }}
          image={selectedImageForAction}
          onImageDeleted={() => {
            setShowImageActionModal(false);
            setSelectedImageForAction(null);
            // Optionally refresh ailment or notify parent
          }}
          onImageUploaded={() => {
            setShowImageActionModal(false);
            setSelectedImageForAction(null);
            // Optionally refresh ailment or notify parent
          }}
        />
      )}
    </div>
  );
};

export default MbdConditionModal;