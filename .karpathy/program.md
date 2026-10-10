# Program: implement Author/Genre Management to pass acceptance tests

## Objective
Metric: `acceptance_test_failures` (lower is better), measured by `.karpathy/eval.sh`.
Baseline: TBD (run `loop-step.sh --baseline`). Target: 0 failures.

## Scope
You may edit only:
- `BookTracker.DAL/DBManagers/AuthorDbManager.cs`
- `BookTracker.DAL/DBManagers/GenreDbManager.cs`
- `BookTracker.BLL/Services/AuthorService.cs`
- `BookTracker.BLL/Services/GenreService.cs`

Everything else, including tests, build props, analyzers, and `.karpathy/`, is read-only.

## Hard rules
- Behavior must not change. The guard (`dotnet test --nologo -v q`) must pass.
- No metric gaming: no special-casing eval inputs, no suppressing tests, no weakening checks.
- One idea per experiment. Small diffs. Simpler code at equal metric is preferred.

## Ideas to try
- Implement the public API to satisfy test requirements
- Handle edge cases and error conditions
- Add proper validation and data handling

## Known dead ends
- None yet

## Context
## Feature: Author and Genre Management

This feature provides searchable dropdown functionality for Authors and Genres in the BookTracker application.

### Architecture & Implementation

**DAL Layer:**
- `AuthorDbManager` and `GenreDbManager` handle raw data retrieval and operations.
- Methods: `GetAuthorsForSearchableDropdown(string?)`, `GetGenresForSearchableDropdown(string?)`.
- Logic: Uses `.AsNoTracking()`, filters by name if search term provided, sorts alphabetically, applies display limit.
- Auto-create: Book operations auto-create Author and Genre entities if not found.

**BLL Layer:**
- `AuthorService` and `GenreService` provide service abstractions.
- Maps DAL entities to UI models using AutoMapper.
- `DatabaseInitializer` populates initial data on first run.

**UI Layer (Blazor):**
- `AddBook.razor`: Includes search input fields and selection logic for author/genre dropdowns.
- `SearchableDropdown.razor`: Reusable component for searchable selection with debounce-based search.

### Key Constraints & Constants
- **Display Limit:** Controlled by `DropdownConstants.InitialDisplayLimit` (default: 10) for consistent behavior.
- **Localization:** All displayed names support multiple languages via translation tables.
- **Entity Relationships:** Books have foreign key relationships to Authors and Genres (stored as GUIDs).

### Testing
- Unit tests exist for both managers, verifying sorting, filtering, and limit application using In-Memory database.
- `DatabaseInitializer` tests verify initial data population.

## Test Order (T01 to Txx)

### AuthorDropdown Tests
- T01: GetAuthorsForSearchableDropdown_ReturnsSortedList
- T02: GetAuthorsForSearchableDropdown_FiltersBySearchTerm
- T03: GetAuthorsForSearchableDropdown_AppliesDisplayLimit
- T04: GetAuthorsForSearchableDropdown_ReturnsEmptyWhenNoMatch
- T05: GetAuthorsForSearchableDropdown_CaseSensitiveSearch
- T06: GetAuthorsForSearchableDropdown_MatchesMultipleResults
- T07: GetAuthorsForSearchableDropdown_WhitespacesNotTrimmed
- T08: GetAuthorsForSearchableDropdown_EmptyStringTreatedAsNull
- T09: GetAuthorsForSearchableDropdown_WithSpecialCharacters

### GenreDropdown Tests
- T10: GetGenresForSearchableDropdown_ReturnsSortedList
- T11: GetGenresForSearchableDropdown_FiltersBySearchTerm
- T12: GetGenresForSearchableDropdown_ReturnsEmptyWhenNoMatch
- T13: GetGenresForSearchableDropdown_CaseSensitiveSearch
- T14: GetGenresForSearchableDropdown_PartialMatch
- T15: GetGenresForSearchableDropdown_MatchesMultipleResults
- T16: GetGenresForSearchableDropdown_WhitespacesNotTrimmed
- T17: GetGenresForSearchableDropdown_EmptyStringTreatedAsNull
- T18: GetGenresForSearchableDropdown_LimitApplied
- T19: GetGenresForSearchableDropdown_WithSpecialCharacters
