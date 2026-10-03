CA# Context
The user requested an overview of the `BookTracker` project and suggestions for improvements. Following exploration, a major architectural flaw was identified: `BlazorWebApp` (Presentation Layer) has direct dependencies on `BookTracker.DAL` (Data Access Layer), bypassing the intended layered architecture. This plan outlines the refactoring required to decouple these layers by introducing BLL Domain Models and implementing a mapping layer.

# Implementation Plan

## Phase 1: Model Definition (BLL Layer)
The objective is to create "Domain Models" in `BookTracker.BLL/Models/` that are decoupled from database implementation details (like EF Core navigation properties or specific DB types).

1.  **Define BLL Domain Models**: Create/Update models in `BookTracker.BLL/Models/`.
    - `AuthorModel.cs`: Basic author info without DAL entity baggage.
    - `BookModel.cs`: Include necessary metadata and relationships (e.g., `GenreId`, `AuthorId`) but avoid direct EF Core navigation properties.
    - `GenreModel.cs`: Basic genre definition.
    - `LanguageModel.cs` / `Enum Models`: Define or mirror enums in a way that doesn't require a dependency on `BookTracker.DAL.Entities.Enums`.
    
2.  **Define Job/Task Models**: Create models for background jobs (e.g., `TranslationJobModel`) to replace `BookTracker.DAL.Models.BookTranslationJob`.

## Phase 2: Mapping Layer (BLL & UI Layers)
The objective is to implement the translation logic between DAL Entities and BLL Domain Models, leveraging existing AutoMapper profiles in the UI layer.

1.  **Refactor/Improve Existing Profiles**: Utilize, improve, or refactor the existing mapping profiles located at `BlazorWebApp/AutoMapper/` (e.g., `BooksProfile.cs`, `AuthorsProfile.cs`, `GenresProfile.cs`).
    - Transition these from direct DAL $\rightarrow$ UI mappings to either:
        a) Core Entity $\leftrightarrow$ BLL Model mappings (moved to BLL).
        b) BLL Model $\rightarrow$ UI ViewModel mappings (if needed in Blazor).
2.  **Integrate AutoMapper in BLL**: Add/Configure AutoMapper dependency in `BookTracker.BLL` to handle the primary Entity $\leftrightarrow$ Model mapping logic.
3.  **Implement Mapping Profiles**: Ensure robust mapping profiles exist within `BookTracker.BLL` (e.g., `BookMappingProfile.cs`) that define how:
    - `BookTracker.DAL.Entities.Books.Book` $\rightarrow$ `BookTracker.BLL.Models.BookModel`
    - `BookTracker.DAL.Entities.Authors.Author` $\rightarrow$ `AuthorModel`
    - ...and so on for all relevant entities.

## Phase 3: BLL Refactoring (Business Logic Layer)
The objective is to change the "contract" of the BLL services.

1.  **Update Service Interfaces**: Update `BookTracker.BLL/Abstractions/IBooksService.cs` and others to ensure methods return/accept BLL models instead of DAL entities.
2.  **Refactor Service Implementations**: Update `BookTracker.BLL/Services/BooksService.cs`:
    - Inject DAL abstractions (e.g., `IBookDBManager`).
    - Fetch entities from DAL.
    - Use AutoMapper to map fetched entities to BLL models before returning them to the caller.
3.  **Handle Input Models**: Ensure methods that accept data (e.g., `AddBook(BookModel model)`) use the Domain Model rather than the Entity.

## Phase 4: UI Refactoring (BlazorWebApp Layer)
The objective is to strip all direct references to `BookTracker.DAL` from the web application.

1.  **Clean up Dependency Injection**: Update `BlazorWebApp/Program.cs` to remove `using BookTracker.DAL.DBContexts`. Ensure it only registers BLL services.
2.  **Refactor AutoMapper Profiles**: (See Phase 2) Move or update `BlazorWebApp/AutoMapper/*Profile.cs`.
3.  **Update Background Services**: Refactor `BlazorWebApp/Services/HangfireBookTranslationJobScheduler.cs` to use BLL services and BLL models instead of DAL abstractions/models.
4.  **Refactor Razor Components**: 
    - Update `@using` statements in `.razor` files (e.g., `AddBook.razor`) to point to `BookTracker.BLL.Models` instead of `BookTracker.DAL.Entities`.
    - Update component logic to handle BLL models.

# Verification Plan

1.  **Compilation Check**: Ensure the project builds successfully. A successful build confirms that all direct dependencies on DAL in the UI have been replaced by BLL references.
2.  **Manual UI Verification**: 
    - Navigate through the "Books List" and "Book Details" pages to ensure data is rendered correctly.
    - Test the "Add Book" functionality to ensure data flows from the UI $\rightarrow$ BLL $\rightarrow$ DAL correctly via models.

# Critical Files
- `BookTracker.BLL/Services/BooksService.cs`
- `BookTracker.BLL/Models/BookModel.cs`
- `BlazorWebApp/Program.cs`
- `BlazorWebApp/AutoMapper/BooksProfile.cs`
- `BlazorWebApp/Components/Pages/Books/Details/AddBook.razor`
- `BookTracker.DAL/Abstractions/IBookDBManager.cs`
