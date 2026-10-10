# Feature: Multi-Language Translation System

## Overview
A background job-based translation system that supports multiple languages for books, authors, and genres. Uses Hangfire for deferred processing and asynchronous translation via external services.

## Architecture & Implementation
- **DAL Layer:** 
    - `BookTranslationProcessor` orchestrates translation processing for books, authors, and genres.
    - `TranslationsDbManager` handles translation entity persistence (BookTranslation, AuthorTranslation, GenreTranslation).
    - Methods: `ProcessTranslationAsync(BookTranslationJob)`, `CreateBookTranslation()`, `CreateAuthorTranslation()`, `CreateGenreTranslation()`.
    - Logic: Uses transactions to ensure atomicity, handles duplicate prevention, and rolls back on errors.
- **BLL Layer:** 
    - `BooksService` enqueues translation jobs when adding/updating books.
    - `ConfigurableTextTranslator` handles actual translation API calls.
- **UI Layer (Blazor):**
    - `CultureSelector.razor`: Language selection dropdown for users to switch display language.
    - Localization: UI uses `IStringLocalizer<ApplicationResources>` for translatable strings.

## Key Constraints & Constants
- **Job Queuing:** Hangfire is used for background job scheduling with dashboard authorization.
- **Translation Flow:** Translations are queued asynchronously when books are added/updated, not blocking the user.
- **Entity Types:** Supports translations for Book (title), Author (name), and Genre (name).
- **Language Mapping:** Uses `Languages` enum with `InvertLanguage()` extension for language pair mapping.

## Testing
- Unit tests verify translation job enqueueing and processor logic.
- Transaction handling ensures data consistency across multiple entity translations.
