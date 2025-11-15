# Mind-Body Dictionary: Ailments → Conditions Refactoring Summary

## Executive Summary

**Goal**: Rename and restructure the `Ailment` domain concept to `Condition` with improved scalability, semantics, and maintainability.

**Current State**: 245 conditions in CosmosDB, mixed medical/wellness data with duplicate entity types and no versioning.

**Proposed State**: Single unified Condition model with structured recommendations, metadata tracking, and support for future expansion.

---

## Key Changes at a Glance

### 1. Data Model Changes

| Aspect | Current | New |
|--------|---------|-----|
| **Entity Name** | `Ailment` | `Condition` |
| **Entity Variants** | 3 types (Ailment, AilmentShort, AilmentRandom) | 1 unified model |
| **Type System** | Implicit (all ailments) | Explicit: CONDITION, WELLNESS_STATE, BODY_SYSTEM, SYMPTOM |
| **Physical Connections** | String array `["Stomach", "Digestion"]` | Structured objects with type/description |
| **Recommendation Types** | Magic numbers (0, 2, 3) | Named enum: FOOD, BOOK, SUPPLEMENT, etc. |
| **Versioning** | None | Full metadata with timestamps, version, creator |
| **Audit Trail** | None | createdAt, updatedAt, createdBy, updatedBy |
| **Data Quality** | Not tracked | Completeness score + quality flags |

### 2. API Changes

| Endpoint | Current | New |
|----------|---------|-----|
| **List** | `GET /Ailments?code=...` | `GET /api/conditions?page=1&pageSize=20` |
| **Detail** | `GET /Ailment?code=...&id={id}` | `GET /api/conditions/{id}` |
| **Random** | `GET /AilmentsRandom?code=...` | `GET /api/conditions/random` |
| **Search** | Not available | `POST /api/conditions/search` |
| **Create** | `POST /UpsertAilment` | `POST /api/conditions` |
| **Update** | `POST /UpsertAilment` | `PUT /api/conditions/{id}` |
| **Delete** | `DELETE /DeleteAilment` | `DELETE /api/conditions/{id}` |

### 3. Code Structure Changes

**Remove:**
- `MindBodyDictionary.Core.Entities.Ailment.cs`
- `MindBodyDictionary.Core.Entities.AilmentShort.cs`
- `MindBodyDictionary.Core.Entities.AilmentRandom.cs`
- `MindBodyDictionary.Core.Client.AilmentClient.cs`
- `MindBodyDictionary.AdminApi.Ailment.cs`
- `MindBodyDictionary.AdminApi.AilmentsTable.cs`
- `MindBodyDictionary.AdminApi.UpsertAilment.cs`
- `MindBodyDictionary.AdminApi.DeleteAilment.cs`

**Add:**
- `MindBodyDictionary.Core.Entities.Condition.cs`
- `MindBodyDictionary.Core.Entities.ConditionType.cs` (enum)
- `MindBodyDictionary.Core.Entities.RecommendationType.cs` (enum)
- `MindBodyDictionary.Core.ViewModels.ConditionDetailView.cs`
- `MindBodyDictionary.Core.ViewModels.ConditionSummaryView.cs`
- `MindBodyDictionary.Core.ViewModels.ConditionInsightView.cs`
- `MindBodyDictionary.Core.Client.ConditionClient.cs`
- `MindBodyDictionary.Core.Client.IConditionClient.cs`
- `MindBodyDictionary.AdminApi.ConditionsFunction.cs`

**Update:**
- `MindBodyDictionary.Core.Constants.cs` (container names)
- All references in other classes

### 4. Database Changes

```
CosmosDB:
  Database: MindBodyDictionary
    Container: Ailments → Conditions
      Partition Key: /id
      Index: /name (for search)
      Documents: 245 → migrated with new schema
```

---

## Critical Issues Fixed

### 🔴 Critical

1. **Performance Bottleneck**: Ailment.cs fetches ALL 245 items into memory, then filters
   - **Risk**: Scales terribly, will timeout at 1000+ items
   - **Solution**: Direct CosmosDB queries with indexes

2. **Security**: API keys hardcoded in client code
   - **Risk**: Keys exposed in source repositories
   - **Solution**: Move to configuration/Azure Key Vault

3. **Scalability**: No pagination support
   - **Risk**: Cannot handle large result sets
   - **Solution**: Native pagination in queries

### 🟠 High Priority

1. **Data Quality**: Whitespace in affirmations, inconsistent naming
   - **Fix**: Cleanup during migration, add validation

2. **No Audit Trail**: Cannot track who changed what or when
   - **Fix**: Add metadata with timestamps and creators

3. **Magic Numbers**: Recommendation types are 0, 2, 3 with no documentation
   - **Fix**: Named enums with clear meaning

### 🟡 Medium Priority

1. **Unstructured Data**: Physical connections are strings mixed with function names
   - **Fix**: Structured objects with type classification

2. **Duplicate Code**: Image URL generation in 2+ places
   - **Fix**: Single media reference handler

3. **Poor Semantics**: "Ailment" doesn't capture wellness states or educational content
   - **Fix**: "Condition" with explicit type system allows expansion

---

## Migration Complexity Assessment

| Component | Complexity | Effort | Risk |
|-----------|-----------|--------|------|
| Entity Rename | Low | 4h | Low |
| API Refactor | Medium | 8h | Medium |
| CosmosDB Migration | Medium | 6h | High |
| Client Update | Low | 2h | Low |
| Backward Compat | High | 12h | High |
| Testing | Medium | 8h | Medium |
| **Total** | **Medium** | **~40h** | **Medium** |

---

## Actual Data Insights (245 conditions)

**Top Categories by Tag Count:**
- Digestive: ~30 conditions
- Musculoskeletal: ~25 conditions
- Mental/Emotional: ~40 conditions
- Respiratory: ~15 conditions
- Other: ~135 conditions

**Recommendation Stats:**
- Avg 15-25 recommendations per condition
- Most common: Foods (~60%)
- Books (~15%)
- Supplements (~25%)

**Subscription Distribution:**
- Most conditions: subscriptionOnly = true
- Free tier: ~20 conditions
- Premium: ~225 conditions

**Data Quality Gaps:**
- Missing URLs in recommendations: ~30%
- Whitespace issues in text: ~5%
- Duplicate tags: ~15%
- No categories assigned: 100%

---

## Implementation Checklist

### Phase 1: Preparation (Week 1)
- [ ] Create feature branch: `refactor/ailments-to-conditions`
- [ ] Create new entity models (Condition, enums)
- [ ] Create view models (DetailView, SummaryView, InsightView)
- [ ] Write migration script (test on copy first)
- [ ] Update Constants.cs with new container names

### Phase 2: API Layer (Week 2)
- [ ] Create IConditionClient interface
- [ ] Implement ConditionClient
- [ ] Create ConditionsFunction (Azure Functions)
- [ ] Add pagination/filtering support
- [ ] Remove hardcoded API keys (use config)

### Phase 3: Database Migration (Week 3)
- [ ] Create "Conditions" container in test environment
- [ ] Run migration script on test data
- [ ] Validate data integrity
- [ ] Verify queries work correctly
- [ ] Test backward compatibility layer

### Phase 4: Deployment (Week 4)
- [ ] Deploy to staging
- [ ] Run smoke tests
- [ ] Update client apps
- [ ] Monitor for errors
- [ ] Keep legacy endpoints active for 1-2 releases

### Phase 5: Cleanup (Ongoing)
- [ ] Deprecate old AilmentClient (add obsolete warnings)
- [ ] Update documentation
- [ ] Remove legacy endpoints (after deprecation period)
- [ ] Decommission old Ailments container

---

## File Reference Guide

### Core Entities
```
MindBodyDictionary.Core/Entities/
├── Condition.cs                    # Main model (NEW)
├── ConditionType.cs                # Enum (NEW)
├── RecommendationType.cs           # Enum (NEW)
├── MindBodyPerspective.cs          # Supporting model
├── PhysicalConnection.cs           # Supporting model
├── Recommendation.cs               # Updated structure
├── MediaReferences.cs              # Supporting model
├── AccessControl.cs                # Supporting model
├── Metadata.cs                     # NEW - audit trail
└── DataQuality.cs                  # NEW - quality tracking
```

### View Models
```
MindBodyDictionary.Core/ViewModels/
├── ConditionDetailView.cs          # Full response (NEW)
├── ConditionSummaryView.cs         # List response (NEW)
├── ConditionInsightView.cs         # Quick insight (NEW)
└── ConditionFilter.cs              # Search filters (NEW)
```

### Clients
```
MindBodyDictionary.Core/Client/
├── IConditionClient.cs             # Interface (NEW)
├── ConditionClient.cs              # Implementation (NEW)
└── Interfaces/IConditionClient.cs  # Alias location
```

### Azure Functions
```
MindBodyDictionary.AdminApi/
├── ConditionsFunction.cs           # Main endpoint (NEW)
├── CreateCondition.cs              # POST handler (NEW)
├── UpdateCondition.cs              # PUT handler (NEW)
├── DeleteCondition.cs              # DELETE handler (NEW)
└── SearchConditions.cs             # Search endpoint (NEW)
```

---

## Key Design Decisions

### Why "Condition" over "Ailment"?

1. **Semantically inclusive**: Encompasses medical conditions, wellness states, body systems, and symptoms
2. **Future-proof**: `ConditionType` enum allows expansion without schema changes
3. **Industry standard**: Used in healthcare apps (Conditions in FHIR)
4. **Holistic**: Aligns with mind-body philosophy beyond just "ailments"

### Why Structured Physical Connections?

**Before** (searchable but not queryable):
```json
"physicalConnections": ["Stomach", "Large Intestine", "Digestion"]
```

**After** (fully queryable and typed):
```json
"physicalConnections": [
  {"id": "stomach", "name": "Stomach", "type": "ORGAN"},
  {"id": "large_intestine", "name": "Large Intestine", "type": "ORGAN"},
  {"id": "digestion", "name": "Digestion", "type": "SYSTEM"}
]
```

### Why DTO Models?

- **Response flexibility**: Return different data based on use case
- **Forward compatibility**: Can add fields to DTOs without changing API contract
- **Performance**: DetailView returns full data, SummaryView is lightweight
- **Security**: Can exclude sensitive data from certain views

### Why Enum for Recommendation Types?

**Before** (unclear meaning):
```json
"recommendationType": 0  // What does 0 mean?
```

**After** (self-documenting):
```json
"type": "SUPPLEMENT"  // Clear intent
```

---

## Testing Strategy

### Unit Tests
- [ ] Condition model serialization/deserialization
- [ ] View model mapping (Condition → DTO)
- [ ] Filter query building
- [ ] Migration script transformations

### Integration Tests
- [ ] CosmosDB queries with real data
- [ ] Pagination boundaries
- [ ] Search functionality
- [ ] Backward compatibility adapter

### Migration Tests
- [ ] All 245 documents transform correctly
- [ ] No data loss
- [ ] IDs preserved
- [ ] Recommendations maintain structure

### Performance Tests
- [ ] Single document retrieval (<100ms)
- [ ] List with pagination (<500ms)
- [ ] Search with filters (<1s)
- [ ] No in-memory loading for any query

---

## Rollback Plan

If issues occur:

1. **Pre-migration**: Keep Ailments container intact
2. **During migration**: Test on copy first
3. **Post-migration**: 
   - Keep legacy endpoints running
   - Monitor error rates
   - Have script to revert CosmosDB if needed
   - Within 24h can roll back to old client code

---

## Questions for Stakeholders

1. How long do we support legacy `/ailments` endpoints?
2. Should we auto-migrate existing API keys or require reconfiguration?
3. Are there external apps relying on current API structure that need notification?
4. Do you want to backfill historical metadata or just timestamp new entries?
5. Should physical connections be reviewed for type classification, or use heuristics?

