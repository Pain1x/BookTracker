# Feature: Searchable Dropdowns (Authors & Genres)

## Overview
Implemented searchable dropdown menus for Authors and Genres to simplify data entry in the BookTracker application. This allows users to filter results as they type and provides a "Create New" flow directly from the selection UI.

## Architecture & Implementation
- **DAL Layer:** 
    - `AuthorDbManager` and `GenreDbManager` handle raw data retrieval.
    - Methods: `GetAuthorsForSearchableDropdown(string? searchTerm)` and `GetGenresForSearchableDropdown(string? searchTerm)`.
    - Logic: Uses `.AsNoTracking()`, filters by name if a search term is provided, sorts alphabetically, and applies a display limit.
- **BLL Layer:** 
    - Services in `BookTracker.BLL` map DAL entities to UI models using AutoMapper.
- **UI Layer (Blazor):**
    - Updated `AddBook.razor` to include search input fields and selection logic for the dropdowns.

## Key Constraints & Constants
- **Display Limit:** Controlled by `DropdownConstants.InitialDisplayLimit` (default: 10) to ensure consistent behavior between data fetching and UI rendering.
- **Localization:** All displayed names in dropdowns are localized via translation tables.

## Testing
- Unit tests exist for both managers, verifying sorting, filtering, and limit application using an In-Memory database.