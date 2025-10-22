# How to Create GitHub Issue from Template

This guide explains how to use the generated GitHub issue template for the RSS Feed Management feature.

## Quick Steps

1. **Open GitHub Issues Page**
   - Navigate to: https://github.com/74nu5/watch-manager/issues
   - Click the green "New issue" button

2. **Copy the Template**
   - Open the file `GITHUB_ISSUE_RSS_FEED.md` in this repository
   - Copy all content (Ctrl+A, Ctrl+C)

3. **Create the Issue**
   - Paste the content into the GitHub issue description field
   - Set the title to: **"Add RSS Feed Management - US-001"**

4. **Add Labels** (recommended)
   - `enhancement`
   - `feature`
   - `user-story`
   - `high-priority`

5. **Assign Project** (optional)
   - Assign to appropriate sprint/milestone
   - Add to project board if applicable

6. **Submit**
   - Review the formatted issue
   - Click "Submit new issue"

## What's Included in the Template

The generated issue template (`GITHUB_ISSUE_RSS_FEED.md`) includes:

### ✅ Complete Specification
- User story in standard format (As a... I want... So that...)
- Comprehensive acceptance criteria
- Technical requirements

### 🎨 UI/UX Details
- ASCII mockups for all dialog states:
  - Initial state
  - Validation in progress
  - Success state
  - Error state

### 🧪 Test Scenarios
Five detailed test cases covering:
1. Standard RSS 2.0 feed addition
2. Atom feed support
3. Error handling for invalid URLs
4. Auto-discovery from web pages
5. Duplicate detection

### 🔧 Technical Specs
- **Data Model**: Complete C# entity definitions
- **API Contracts**: RESTful endpoints with request/response examples
- **Implementation Components**: List of required services and components

### 📊 Success Criteria
- User metrics (success rate, UX satisfaction)
- Technical metrics (performance, reliability)
- Quality metrics (error handling, auto-discovery)

### 🚀 Definition of Done
Clear checklist including:
- Code completion requirements
- Test coverage targets
- Performance benchmarks
- Documentation requirements
- Security validation (CodeQL)

## Source Information

This issue template was generated from the user story document:
- **Location**: `backlog/02-integration-flux-rss/01-gestion-flux-rss/us-001-ajout-flux-rss.md`
- **Epic**: Integration RSS Feed
- **Feature**: RSS Feed Management
- **Story**: US-001 - Add RSS Feed

## Notes

- The original user story was in French
- The GitHub issue template is in English for broader accessibility
- All technical specifications from the original document are preserved
- UI mockups have been converted to ASCII art for better GitHub rendering

## Related Files

- `GITHUB_ISSUE_RSS_FEED.md` - The complete issue template
- `backlog/02-integration-flux-rss/01-gestion-flux-rss/us-001-ajout-flux-rss.md` - Original user story (French)
- `backlog/02-integration-flux-rss/epic.md` - Parent epic document
