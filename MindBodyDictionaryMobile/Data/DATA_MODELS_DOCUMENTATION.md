# MindBodyDictionary Mobile - Data Models Documentation

## Overview

This directory contains all data models for the MindBodyDictionary Mobile application. This document serves as a guide to the visual documentation (Excalidraw diagrams) that document the complete data model architecture.

## Visual Diagrams

All diagrams are provided as `.excalidraw.svg` files and can be opened in:

- [Excalidraw Online Editor](https://excalidraw.com)
- Any SVG viewer
- Web browsers (drag and drop)

### 1. **DataModels_ERD.excalidraw.svg** - Entity Relationship Diagram

**Purpose:** High-level overview of all entities and their relationships

**Contents:**

- All 7 core entities (Category, Project, ProjectTask, Tag, ProjectsTags, ImageCache, DeviceInstallation)
- One-to-many and many-to-many relationships
- Primary and foreign key indicators
- Visual flow of data relationships

**Best For:** Understanding the overall data structure and how entities connect

---

### 2. **DataModels_Properties.excalidraw.svg** - Detailed Properties & Types

**Purpose:** Complete reference for all properties, types, and attributes

**Contents:**

- All properties of each entity with their C# types
- Computed properties (like ColorBrush, DisplayColor variants)
- Navigation properties (references to related entities)
- Default values and constraints
- Computed properties vs. persistent storage

**Best For:** Understanding what data each model stores and how it's typed

---

### 3. **DataModels_Relationships.excalidraw.svg** - Relationships & Usage Patterns

**Purpose:** Business logic and usage patterns for each entity

**Contents:**

- Four main groupings:
  1. **Core Domain Logic:** Projects, Categories, and Tasks
  2. **Tagging & Classification:** Tags and ProjectsTags junction table
  3. **Media & Caching:** ImageCache for offline access
  4. **Notifications & Device:** Push notification setup
- Use cases and benefits for each entity
- How entities work together in the application

**Best For:** Understanding WHY certain models exist and how they're used

---

### 4. **DataModels_DatabaseSchema.excalidraw.svg** - Database Schema & Storage

**Purpose:** Physical database storage and implementation details

**Contents:**

- Table structures with column types
- Primary key and foreign key relationships
- Indexing strategy for performance
- Local SQLite storage vs. cloud backend
- JSON serialization format
- Performance optimization notes
- Example JSON data structure

**Best For:** Database developers and understanding persistence layer

---

## Data Model Reference

### Core Entities

#### **Project**

```
ID (int, PK)
Name (string)
Description (string)
Icon (string) - URL or asset path
CategoryID (int, FK) → Category
```

- Main entity representing a user's project or goal
- Contains many Tasks
- Can have multiple Tags (many-to-many via ProjectsTags)
- Related to exactly one Category

**Navigation Properties:**

- `Category` - Parent category
- `Tasks` - Collection of project tasks
- `Tags` - Collection of tags (via junction table)

---

#### **ProjectTask**

```
ID (int, PK)
Title (string)
IsCompleted (bool)
ProjectID (int, FK) → Project
```

- Represents a todo/task within a project
- Simple structure: just title and completion status
- Belongs to exactly one Project

---

#### **Category**

```
ID (int, PK)
Title (string)
Color (string) - Hex color code
```

- Groups projects visually
- Color-coded for UI organization
- Has many Projects

**Computed Properties:**

- `ColorBrush` - Renders the hex color as a Brush for MAUI

---

#### **Tag**

```
ID (int, PK)
Title (string)
Color (string) - Hex color code
IsSelected (bool) - UI state
```

- Metadata labels for projects
- Color-coded like categories but with more sophisticated color variants

**Computed Properties:**

- `ColorBrush` - Base color rendering
- `DisplayColor` - Normal color
- `DisplayDarkColor` - Darkened version (0.8 opacity)
- `DisplayLightColor` - Lightened version (0.2 opacity)

---

#### **ProjectsTags** (Junction Table)

```
ID (int, PK)
ProjectID (int, FK) → Project
TagID (int, FK) → Tag
```

- Implements many-to-many relationship between Projects and Tags
- One project can have multiple tags
- One tag can be applied to multiple projects

---

#### **ImageCache**

```
ID (int, PK)
FileName (string)
ImageData (byte[])
CachedAt (DateTime) - UTC timestamp
ContentType (string) - MIME type
```

- Local storage for cached images
- Supports offline access
- Reduces network traffic on repeated access
- Indexed by FileName for fast lookup

**Note:** This is a local storage entity, not synced to cloud

---

#### **DeviceInstallation**

```
InstallationId (string) - Unique per device
Platform (string) - iOS or Android
PushChannel (string) - Device token
Tags (List<string>) - Notification tags
```

- Server-side only (not in mobile local database)
- Tracks device registrations for push notifications
- Enables targeted notifications by tag
- Used for notification routing

---

### Support/Utility Entities

#### **IconData**

```
Icon (string?)
Description (string?)
```

- Simple data holder for icon references
- Provides accessibility descriptions

---

#### **CategoryChartData**

```
Title (string)
Count (int)
```

- View model for dashboard statistics
- Aggregates count of projects per category
- Created dynamically from queries, not stored

---

#### **NotificationAction** (Enum)

```
ProjectUpdate
TaskReminder
Custom
```

- Determines how the app responds to push notifications
- Used by backend to categorize notifications
- Client uses this to route to appropriate page

---

## Key Design Patterns

### 1. **Many-to-Many with Junction Table**

Projects ↔ Tags relationship uses `ProjectsTags` table

- Allows flexible tagging
- Supports adding/removing tags without modifying original entities
- Enables tag reuse across projects

### 2. **Color Storage & Rendering**

- Stored as hex strings in database
- Converted to Color/Brush objects via computed properties
- Multiple variants (dark/light) for UI flexibility

### 3. **Foreign Keys for Relationships**

- All one-to-many relationships use FK columns (CategoryID, ProjectID)
- Maintains referential integrity
- Enables efficient JOINs in queries

### 4. **Computed vs. Persistent Properties**

- Computed: ColorBrush, DisplayColor variants, AccessibilityDescription
- These are calculated at runtime, not stored in database
- Reduces storage and keeps data normalized

### 5. **JsonIgnore for Serialization**

- Foreign key columns (CategoryID, ProjectID) marked with [JsonIgnore]
- Prevents redundant data in JSON
- Navigation properties used instead (Category object, not CategoryID)

### 6. **Offline-First with Image Caching**

- ImageCache allows app to function offline
- Images cached locally with timestamp
- Decision logic for cache invalidation

### 7. **Server-Side Only Data**

- DeviceInstallation not stored locally
- Synced to backend only for push notifications
- Improves privacy and reduces mobile storage

---

## Data Type Reference

| Type       | Purpose                      | Example                                |
| ---------- | ---------------------------- | -------------------------------------- |
| `int`      | Primary/Foreign keys, counts | `ID`, `ProjectID`, `Count`             |
| `string`   | Text, URLs, color codes      | `Name`, `Icon`, `Color: "#FF0000"`     |
| `bool`     | Flags, status                | `IsCompleted`, `IsSelected`            |
| `DateTime` | Timestamps                   | `CachedAt` (UTC)                       |
| `byte[]`   | Binary data                  | `ImageData` blob                       |
| `List<T>`  | Collections                  | `Tasks`, `Tags`, `Tags (notification)` |
| `Brush`    | UI rendering                 | `ColorBrush` (computed)                |

---

## Database Storage Strategy

### Local Storage (Mobile Device)

- SQLite database
- All project, task, category, and tag data
- ImageCache for offline images
- User's complete offline experience

**Syncing:** Data can be synced to backend

### Cloud Storage (Backend)

- DeviceInstallation records for push routing
- User authentication/profiles
- Audit logs
- Cloud backups of user data

**Implementation:** CosmosDB or SQL Database (per your architecture)

---

## JSON Serialization Example

```json
{
  "id": 1,
  "name": "Fitness Goals",
  "description": "2024 Wellness Program",
  "icon": "https://example.com/icons/fitness.png",
  "category": {
    "id": 1,
    "title": "Health",
    "color": "#4CAF50"
  },
  "tasks": [
    {
      "id": 1,
      "title": "Morning Run",
      "isCompleted": true
    }
  ],
  "tags": [
    {
      "id": 1,
      "title": "Cardio",
      "color": "#FF5722"
    }
  ]
}
```

**Note:** CategoryID is not included (JsonIgnore), but full Category object is

---

## Performance Optimization

### Indexing

- Primary keys automatically indexed
- Foreign keys should be indexed for JOINs
- ProjectsTags: composite index on (ProjectID, TagID)
- ImageCache: index on FileName for cache lookups

### Query Patterns

- Eager load related data (avoid N+1 queries)
- Use navigation properties efficiently
- Cache frequently accessed categories and tags
- Paginate large result sets

### Image Caching

- Store only frequently accessed images locally
- Use FileName as cache key
- Consider timestamp for cache invalidation

---

## How to Use These Diagrams

1. **Start with ERD.svg** - Get the big picture
2. **Review Properties.svg** - Understand data types
3. **Study Relationships.svg** - Learn use cases
4. **Reference DatabaseSchema.svg** - For implementation details

### Opening in Excalidraw

1. Go to https://excalidraw.com
2. Click "Open" → Select the .svg file
3. Edit, add notes, or export as needed

### Integration with Repositories

All C# model files are in the `/Models/` directory:

- Category.cs
- Project.cs
- ProjectTask.cs
- Tag.cs
- ProjectsTags.cs
- ImageCache.cs
- DeviceInstallation.cs
- NotificationAction.cs
- IconData.cs
- CategoryChartData.cs

---

## Related Code Files

### Data Access Layer

- `CategoryRepository.cs` - Category queries
- `ProjectRepository.cs` - Project queries
- `TagRepository.cs` - Tag queries
- `TaskRepository.cs` - ProjectTask queries
- `ImageCacheRepository.cs` - Image cache management

### Serialization

- `JsonContext.cs` - JSON serialization context

### Services

- `SeedDataService.cs` - Initial data loading
- `ImageCacheService.cs` - Image caching logic

---

## Notes & Conventions

- All IDs are auto-incrementing integers
- Timestamps stored as UTC DateTime
- Colors stored as hex strings (#RRGGBB format)
- Foreign key naming: [EntityName]ID
- Navigation properties nullable when optional
- Computed properties marked with [JsonIgnore]

---

**Last Updated:** November 23, 2024
**Diagram Format:** SVG (Excalidraw compatible)
**Total Diagrams:** 4 comprehensive visual references
