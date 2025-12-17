import React from 'react';
import { Tabs, TabsList, TabsTrigger, TabsContent } from './components/ui/tabs';
import { useTheme } from '../theme/useTheme';
import { MbdCondition, Recommendation, RecommendationType } from '../types';

interface AilmentModalProps {
  isOpen: boolean;
  ailment: MbdCondition | null;
  onClose: () => void;
  onSave: () => void;
  onChange: (ailment: MbdCondition) => void;
  getImageUrl: (type: 'negative' | 'positive') => string;
}

const AilmentModal: React.FC<AilmentModalProps> = ({
  isOpen,
  ailment,
  onClose,
  onSave,
  onChange,
  getImageUrl,
}) => {
  const { colors } = useTheme();
  if (!ailment) return null;
  console.log("Ailment data in modal:", ailment);

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
                {ailment?.id ? `Edit: ${ailment.name}` : 'Add New Ailment'}
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
                        id="id"
                        disabled
                        value={ailment.id || ''}
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
                      />
                    </div>
                    <div>
                      <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: '#495057' }} htmlFor="name">Name</label>
                      <input
                        type="text"
                        id="name"
                        value={ailment.name}
                        onChange={(e) => onChange({ ...ailment, name: e.target.value })}
                        style={{
                          width: '100%',
                          padding: '10px 12px',
                          fontSize: '14px',
                          border: '1px solid #d0d0d0',
                          borderRadius: '6px',
                          transition: 'all 0.2s'
                        }}
                        onFocus={(e) => {
                          e.currentTarget.style.borderColor = '#0066cc';
                          e.currentTarget.style.boxShadow = '0 0 0 3px rgba(0,102,204,0.1)';
                        }}
                        onBlur={(e) => {
                          e.currentTarget.style.borderColor = '#d0d0d0';
                          e.currentTarget.style.boxShadow = 'none';
                        }}
                      />
                    </div>
                  </div>
                  <div>
                    <div style={{ marginBottom: '20px' }}>
                      <label style={{ display: 'block', fontSize: '13px', fontWeight: '600', marginBottom: '8px', color: '#495057' }} htmlFor="imageNegative">Image Negative</label>
                      <input
                        type="text"
                        id="imageNegative"
                        value={ailment.imageNegative || ''}
                        onChange={(e) => onChange({ ...ailment, imageNegative: e.target.value })}
                        style={{
                          width: '100%',
                          padding: '10px 12px',
                          fontSize: '14px',
                          border: '1px solid #d0d0d0',
                          borderRadius: '6px',
                          transition: 'all 0.2s'
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
                        value={ailment.imagePositive || ''}
                        onChange={(e) => onChange({ ...ailment, imagePositive: e.target.value })}
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
                    value={ailment.summaryNegative}
                    onChange={(e) => onChange({ ...ailment, summaryNegative: e.target.value })}
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
                    value={ailment.summaryPositive}
                    onChange={(e) => onChange({ ...ailment, summaryPositive: e.target.value })}
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
                <div style={{ marginTop: '32px', display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', paddingTop: '32px', borderTop: `1px solid ${colors.border}` }}>
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '8px' }}>
                    <img src={getImageUrl('negative')} alt="Negative" style={{ width: '100%', maxHeight: '250px', objectFit: 'contain', borderRadius: '6px', backgroundColor: '#ffffff', padding: '12px', border: `1px solid ${colors.border}` }} />
                    <p style={{ fontSize: '12px', color: colors.mutedText, margin: 0 }}>Negative</p>
                  </div>
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '8px' }}>
                    <img src={getImageUrl('positive')} alt="Positive" style={{ width: '100%', maxHeight: '250px', objectFit: 'contain', borderRadius: '6px', backgroundColor: '#ffffff', padding: '12px', border: `1px solid ${colors.border}` }} />
                    <p style={{ fontSize: '12px', color: colors.mutedText, margin: 0 }}>Positive</p>
                  </div>
                </div>
              </div>
            </TabsContent>

            <TabsContent value="affirmations">
              <div style={{ padding: '20px' }}>
                <h5 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '20px', color: colors.foreground }}>Affirmations</h5>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                  {(ailment.affirmations || []).map((aff: string, idx: number) => (
                    <div key={idx} style={{ display: 'flex', gap: '8px' }}>
                      <textarea
                        value={aff}
                        onChange={(e) => {
                          const newAffirmations = [...(ailment.affirmations || [])];
                          newAffirmations[idx] = e.target.value;
                          onChange({ ...ailment, affirmations: newAffirmations });
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
                          const newAffirmations = ailment.affirmations?.filter((_: string, i: number) => i !== idx) || [];
                          onChange({ ...ailment, affirmations: newAffirmations });
                        }}
                        style={{
                          padding: '8px 12px',
                          backgroundColor: colors.danger,
                          color: '#fff',
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
                      const newAffirmations = [...(ailment.affirmations || []), ''];
                      onChange({ ...ailment, affirmations: newAffirmations });
                    }}
                    style={{
                      padding: '10px 20px',
                      backgroundColor: colors.primary,
                      color: '#fff',
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
                  {(ailment.physicalConnections || []).map((conn: string, idx: number) => (
                    <div key={idx} style={{ display: 'flex', gap: '8px' }}>
                      <input
                        type="text"
                        value={conn}
                        onChange={(e) => {
                          const newConnections = [...(ailment.physicalConnections || [])];
                          newConnections[idx] = e.target.value;
                          onChange({ ...ailment, physicalConnections: newConnections });
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
                          const newConnections = ailment.physicalConnections?.filter((_: string, i: number) => i !== idx) || [];
                          onChange({ ...ailment, physicalConnections: newConnections });
                        }}
                        style={{
                          padding: '8px 12px',
                          backgroundColor: colors.danger,
                          color: '#fff',
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
                      const newConnections = [...(ailment.physicalConnections || []), ''];
                      onChange({ ...ailment, physicalConnections: newConnections });
                    }}
                    style={{
                      padding: '10px 20px',
                      backgroundColor: colors.primary,
                      color: '#fff',
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
                  {(ailment.tags || []).map((tag: string, idx: number) => (
                    <div key={idx} style={{ display: 'flex', gap: '8px' }}>
                      <input
                        type="text"
                        value={tag}
                        onChange={(e) => {
                          const newTags = [...(ailment.tags || [])];
                          newTags[idx] = e.target.value;
                          onChange({ ...ailment, tags: newTags });
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
                          const newTags = ailment.tags?.filter((_: string, i: number) => i !== idx) || [];
                          onChange({ ...ailment, tags: newTags });
                        }}
                        style={{
                          padding: '8px 12px',
                          backgroundColor: colors.danger,
                          color: '#fff',
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
                      const newTags = [...(ailment.tags || []), ''];
                      onChange({ ...ailment, tags: newTags });
                    }}
                    style={{
                      padding: '10px 20px',
                      backgroundColor: colors.primary,
                      color: '#fff',
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
                  {(ailment.recommendations || []).map((rec: Recommendation, idx: number) => (
                    <div key={idx} style={{ display: 'flex', flexDirection: 'column', gap: '8px', padding: '12px', backgroundColor: colors.backgroundSecondary, borderRadius: '6px' }}>
                      <div style={{ display: 'flex', gap: '8px' }}>
                        <textarea
                          value={typeof rec === 'string' ? rec : rec?.name || ''}
                          onChange={(e) => {
                            const newRecommendations = [...(ailment.recommendations || [])];
                            if (typeof rec === 'string') {
                              newRecommendations[idx] = { name: e.target.value, url: '', recommendationType: 0 } as Recommendation;
                            } else {
                              newRecommendations[idx] = { ...rec, name: e.target.value };
                            }
                            onChange({ ...ailment, recommendations: newRecommendations });
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
                            const newRecommendations = ailment.recommendations?.filter((_: Recommendation, i: number) => i !== idx) || [];
                            onChange({ ...ailment, recommendations: newRecommendations });
                          }}
                          style={{
                            padding: '8px 12px',
                            backgroundColor: colors.danger,
                            color: '#fff',
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
                          const newRecommendations = [...(ailment.recommendations || [])];
                          const typeValue = parseInt(e.target.value, 10);
                          if (typeof rec === 'string') {
                            newRecommendations[idx] = { name: '', url: '', recommendationType: typeValue } as Recommendation;
                          } else {
                            newRecommendations[idx] = { ...rec, recommendationType: typeValue };
                          }
                          onChange({ ...ailment, recommendations: newRecommendations });
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
                      const newRecommendations = [...(ailment.recommendations || []), { name: '', url: '', recommendationType: 0 }];
                      onChange({ ...ailment, recommendations: newRecommendations });
                    }}
                    style={{
                      padding: '10px 20px',
                      backgroundColor: colors.primary,
                      color: '#fff',
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
            Save Ailment
          </button>
        </div>
      </div>
    </div>
  );
};

export default AilmentModal;
