# Add RSS Feed Management - US-001

## 📝 Description

As a user of Watch Manager, I want to add an RSS feed by entering its URL, so that I can automatically follow new articles from that source.

## 🎯 Acceptance Criteria

### Core Functionality
- [ ] **Dialog Display**: When I click "Add Feed" button on RSS feed management page, a dialog opens with URL field and "Test Feed" button
- [ ] **Feed Validation**: When I enter a valid RSS URL (e.g., https://devblogs.microsoft.com/dotnet/feed/) and click "Test Feed", the system validates the feed and displays title, description, and article count
- [ ] **Feed Addition**: When the feed is successfully validated and I click "Add Feed", the feed is saved and appears in my list with "Active" status

### Validation Behaviors
- [ ] **URL Format**: Accepts HTTP and HTTPS URLs only
- [ ] **Timeout**: Shows error if feed doesn't respond within 10 seconds
- [ ] **Supported Formats**: Automatically detects RSS 2.0, Atom 1.0, RSS 1.0 (RDF)
- [ ] **Auto-Discovery**: If URL is a web page, attempts to detect associated RSS feed
- [ ] **Duplicate Prevention**: Prevents adding a feed that already exists (based on URL)

### Technical Requirements
- [ ] **Performance**: Validation and addition completes in <2 seconds for standard feed
- [ ] **Error Messages**: Clear messages by error type (404, timeout, invalid format, etc.)
- [ ] **Persistence**: Feed saved to database with all extracted metadata

## 🎨 UI Mockups

### Initial Dialog State
```
┌─────────────────────────────────────────────────────────────┐
│ ➕ Add RSS Feed                                    [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ RSS Feed URL *                                               │
│ [                                                        ]    │
│ 💡 Example: https://devblogs.microsoft.com/dotnet/feed/    │
│                                                              │
│ 🔍 [Test Feed]                                              │
│                                                              │
│               [Cancel]  [Add Feed] (disabled)               │
└─────────────────────────────────────────────────────────────┘
```

### Validation In Progress
```
┌─────────────────────────────────────────────────────────────┐
│ ➕ Add RSS Feed                                    [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ RSS Feed URL *                                               │
│ [https://devblogs.microsoft.com/dotnet/feed/            ]    │
│                                                              │
│ ⏳ Validation in progress...                                │
│ [████████░░░░░░░░]                                          │
│                                                              │
│               [Cancel]  [Add Feed] (disabled)               │
└─────────────────────────────────────────────────────────────┘
```

### Valid Feed Detected
```
┌─────────────────────────────────────────────────────────────┐
│ ➕ Add RSS Feed                                    [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ RSS Feed URL *                                               │
│ [https://devblogs.microsoft.com/dotnet/feed/            ]    │
│ 🔍 [Retest]                                                 │
│                                                              │
│ ✅ Valid RSS 2.0 feed detected!                             │
│ ┌──────────────────────────────────────────────────────┐    │
│ │ 📰 Title: .NET Blog                                  │    │
│ │ 📝 Description: The official .NET team blog from     │    │
│ │    Microsoft, covering .NET news, tips and tricks... │    │
│ │ 📊 25 articles found (latest: 2 days ago)           │    │
│ └──────────────────────────────────────────────────────┘    │
│                                                              │
│ Feed Name (optional)                                        │
│ [.NET Blog Official                                     ]    │
│                                                              │
│ Category                                                     │
│ [.NET & C# ▼]                                               │
│                                                              │
│ Sync Frequency                                              │
│ ⚪ Every hour  ⚫ Every 4h  ⚪ Daily                         │
│                                                              │
│               [Cancel]  [Add Feed]                          │
└─────────────────────────────────────────────────────────────┘
```

### Error State
```
┌─────────────────────────────────────────────────────────────┐
│ ➕ Add RSS Feed                                    [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ RSS Feed URL *                                               │
│ [https://invalid-url.com/feed.xml                       ]    │
│ 🔍 [Retest]                                                 │
│                                                              │
│ ❌ Error: Unable to read feed                               │
│ ┌──────────────────────────────────────────────────────┐    │
│ │ Code: 404 Not Found                                  │    │
│ │ The RSS feed doesn't exist or is no longer accessible. │   │
│ │                                                       │    │
│ │ 💡 Check the URL or try finding the RSS feed        │    │
│ │    on the website.                                   │    │
│ └──────────────────────────────────────────────────────┘    │
│                                                              │
│               [Cancel]  [Add Feed] (disabled)               │
└─────────────────────────────────────────────────────────────┘
```

## 🧪 Test Scenarios

### Test 1: Add standard RSS 2.0 feed
**Given** I am on the RSS feed management page  
**And** I click "Add Feed"  
**When** I enter "https://devblogs.microsoft.com/dotnet/feed/"  
**And** I click "Test Feed"  
**Then** the system displays "✅ Valid RSS 2.0 feed detected!"  
**And** the title ".NET Blog" is displayed  
**And** the article count "25 articles found" is displayed  
**When** I click "Add Feed"  
**Then** the feed appears in my list with "Active" status  
**And** a success notification is displayed

### Test 2: Add Atom feed
**Given** I am on the add feed dialog  
**When** I enter "https://github.com/dotnet/runtime/commits/main.atom"  
**And** I click "Test Feed"  
**Then** the system displays "✅ Valid Atom 1.0 feed detected!"  
**And** the title "Recent Commits to runtime:main" is displayed  
**When** I click "Add Feed"  
**Then** the feed is saved with format "Atom"

### Test 3: Error handling - Invalid URL
**Given** I am on the add feed dialog  
**When** I enter "https://invalid-domain-xyz123.com/feed"  
**And** I click "Test Feed"  
**Then** the system displays "❌ Error: Unable to read feed"  
**And** the error message contains "The remote name could not be resolved"  
**And** the "Add Feed" button remains disabled

### Test 4: Auto-discovery from web page
**Given** I am on the add feed dialog  
**When** I enter "https://devblogs.microsoft.com/dotnet/"  
**And** I click "Test Feed"  
**Then** the system automatically detects the RSS feed in HTML  
**And** displays "✅ Feed automatically discovered"  
**And** the URL is updated to "https://devblogs.microsoft.com/dotnet/feed/"

### Test 5: Duplicate detection
**Given** I already have feed "https://devblogs.microsoft.com/dotnet/feed/"  
**When** I attempt to add the same URL again  
**And** I click "Test Feed"  
**Then** the system displays "⚠️ This feed is already in your list"  
**And** offers to navigate to the existing feed

## 🔧 Technical Specifications

### Data Model
```csharp
public class RssFeed
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? FaviconUrl { get; set; }
    public RssFeedType Type { get; set; } // RSS20, Atom10, RSS10
    public RssFeedStatus Status { get; set; } // Active, Error, Paused
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSyncAt { get; set; }
    public DateTime? LastSuccessfulSyncAt { get; set; }
    public string? LastErrorMessage { get; set; }
    public int SyncFrequencyMinutes { get; set; } = 240; // Default: 4 hours
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<RssFeedItem> Items { get; set; } = [];
}

public enum RssFeedType { RSS20, Atom10, RSS10 }
public enum RssFeedStatus { Active, Error, Paused }
```

### API Endpoints

#### Validate Feed
```http
POST /api/v1/rss-feeds/validate
Authorization: Bearer {token}
Content-Type: application/json

{
  "url": "https://devblogs.microsoft.com/dotnet/feed/"
}

Response 200 OK:
{
  "isValid": true,
  "feedType": "RSS20",
  "metadata": {
    "title": ".NET Blog",
    "description": "The official .NET team blog...",
    "faviconUrl": "https://devblogs.microsoft.com/dotnet/favicon.ico",
    "itemCount": 25,
    "lastPublishedDate": "2025-01-13T10:30:00Z"
  }
}

Response 400 Bad Request:
{
  "isValid": false,
  "error": {
    "code": "FEED_NOT_FOUND",
    "message": "Unable to fetch the RSS feed",
    "details": "404 Not Found"
  }
}
```

#### Add Feed
```http
POST /api/v1/rss-feeds
Authorization: Bearer {token}
Content-Type: application/json

{
  "url": "https://devblogs.microsoft.com/dotnet/feed/",
  "title": ".NET Blog Official",
  "categoryId": 5,
  "syncFrequencyMinutes": 240
}

Response 201 Created:
{
  "id": 42,
  "url": "https://devblogs.microsoft.com/dotnet/feed/",
  "title": ".NET Blog Official",
  "type": "RSS20",
  "status": "Active",
  "createdAt": "2025-01-15T10:30:00Z",
  "categoryId": 5
}
```

### Implementation Components

1. **RssFeedValidationService**: Core validation logic with 10-second timeout
2. **RSS Feed Endpoints**: RESTful API for validate/add/list operations
3. **Blazor Feed Management Page**: UI with dialog for adding feeds
4. **Entity Framework Migration**: Database schema for RssFeed table
5. **Feed Reader Integration**: Using CodeSyndication or similar library

## 📊 Success Metrics

### User Metrics
- **Add Success Rate**: >95% of attempts with valid URLs
- **Average Add Time**: <30 seconds (including validation)
- **UX Satisfaction**: >85% find process intuitive

### Technical Metrics
- **Validation Time**: <2 seconds for 90% of feeds
- **Format Detection Rate**: 100% for RSS 2.0, Atom, RSS 1.0
- **Network Failure Rate**: <5% (excluding legitimate timeouts)

### Quality Metrics
- **Auto-discovery Accuracy**: >80% success on web pages
- **Error Message Quality**: >90% of users understand errors
- **Retry Rate After Error**: >60% retry with corrected URL

## 🚀 Definition of Done

- [ ] **Code**: Validation service + API endpoints + Blazor component functional
- [ ] **Tests**: >90% coverage on validation service
- [ ] **Performance**: <2s for 95% of validations
- [ ] **Documentation**: XML docs on public APIs + README for integration
- [ ] **UX**: Add dialog with all states (loading, success, error)
- [ ] **Integration**: End-to-end tests passing with real RSS feeds
- [ ] **Security**: CodeQL scan passed

---

**Estimation**: 5 points  
**Sprint**: Sprint 1 (RSS Feed Management)  
**Dependencies**: RssFeed Entity + DbContext

**Labels**: enhancement, feature, RSS, user-story  
**Priority**: High  
**Component**: Backend API, Frontend UI, Database
