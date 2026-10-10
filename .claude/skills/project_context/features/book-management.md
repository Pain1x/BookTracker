# Feature: Book CRUD Operations

## Overview
Core book management functionality including Create, Read, Update, and Delete operations for books in the BookTracker application. This includes localized data retrieval and statistical reporting.

## Architecture & Implementation
- **DAL Layer:** 
    - `BookDbManager` handles all book data operations.
    - Methods: `AddBook(Book)`, `UpdateBook(Book)`, `GetAllBooksLocalized(byte languagePk)`, `FindBookByPkLocalized(Guid, byte)`, `CountBooksByYears()`.
    - Logic: Uses `DbContextFactory` for short-lived context instances, handles author/genre auto-creation, and applies translation lookups for localized data.
- **BLL Layer:** 
    - `BooksService` orchestrates book operations with translation queuing.
    - On add: Enqueues translation jobs for title, author name, and genre name to the Hangfire scheduler.
    - Maps DAL entities to BLL models using AutoMapper.
- **UI Layer (Blazor):**
    - `BooksList.razor`: Displays paginated list of books with search and filtering.
    - `BookDetails.razor`: Shows book details and links to edit mode.
    - `AddBook.razor`: Form for creating/editing books with dropdowns for authors and genres.

## Key Constraints & Constants
- **Transaction Scope:** All book operations use short-lived DbContext instances via `DbContextFactory`, never directly injected.
- **Localization:** All book data is localized per language via translation tables. `GetAllBooksLocalized` and `FindBookByPkLocalized` fetch translations for title, author name, and genre name.
- **Year Statistics:** `CountBooksByYears` reports books read in the last 5 years (current year minus 4 to current year).

## Testing
- Unit tests exist for `BookDbManager` verifying sorting, filtering, and data retrieval using an In-Memory database.
