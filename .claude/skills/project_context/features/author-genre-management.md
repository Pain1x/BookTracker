# Feature: Author and Genre Management

## Overview
Management functionality for Authors and Genres including CRUD operations, searchable dropdowns, and data initialization. These entities support multi-language translations.

## Architecture & Implementation
- **DAL Layer:** 
    - `AuthorDbManager` and `GenreDbManager` handle raw data retrieval and operations.
    - Methods: `GetAuthorsForSearchableDropdown(string?)`, `GetGenresForSearchableDropdown(string?)`.
    - Logic: Uses `.AsNoTracking()`, filters by name if search term provided, sorts alphabetically, applies display limit.
    - Auto-create: Book operations auto-create Author and Genre entities if not found.
- **BLL Layer:** 
    - `AuthorService` and `GenreService` provide service abstractions.
    - Maps DAL entities to UI models using AutoMapper.
    - `DatabaseInitializer` populates initial data on first run.
- **UI Layer (Blazor):**
    - `AddBook.razor`: Includes search input fields and selection logic for author/genre dropdowns.
    - `SearchableDropdown.razor`: Reusable component for searchable selection with debounce-based search.

## Key Constraints & Constants
- **Display Limit:** Controlled by `DropdownConstants.InitialDisplayLimit` (default: 10) for consistent behavior.
- **Localization:** All displayed names support multiple languages via translation tables.
- **Entity Relationships:** Books have foreign key relationships to Authors and Genres (stored as GUIDs).

## Testing
- Unit tests exist for both managers, verifying sorting, filtering, and limit application using In-Memory database.
- `DatabaseInitializer` tests verify initial data population.
