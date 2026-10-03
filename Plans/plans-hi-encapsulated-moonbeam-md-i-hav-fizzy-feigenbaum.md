# Implementation Plan: Layer Separation Formalization

This plan outlines the architectural changes required to strictly enforce separation of concerns between `BookTracker.BLL` (Business Logic Layer) and `BookTracker.DAL` (Data Access Layer), achieving maximum encapsulation and testability by ensuring BLL operates exclusively on clean Domain Models.

The approach focuses on strengthening the contract boundaries: making sure the DAL only exposes data via its abstraction (`IBookDBManager`), and the BLL only consumes/produces pure, non-entity domain models (`BookModel`).

## Implementation Plan: Decoupling BLL and DAL

### Phase 2: Mapping Layer Formalization (The Contract)
**Goal:** Ensure all translation logic is centralized, robust, and strictly adheres to the contract between DAL Entities and BLL Domain Models, preventing accidental leakage of EF Core properties into the business layer.

**Actionable Steps:**
1.  **Review `BookTracker.DAL/Abstractions/IBookDBManager.cs`**: Verify that this interface returns raw DAL entities (e.g., `BookEntity`). This is acceptable as long as BLL never consumes these types directly.
2.  **Refine `BookTracker.BLL/Models/BookModel.cs`**: Audit the existing model to ensure it contains *only* business-relevant properties and has zero dependencies on EF Core attributes (e.g., `[Key]`, navigation property definitions, or complex database types). If any DAL-specific fields exist, they must be removed or replaced with clean domain equivalents.
3.  **Formalize Mapping in `BlazorWebApp/AutoMapper/BooksProfile.cs`**: Ensure the mapping configuration explicitly defines how every field from the raw DAL Entity maps to the corresponding clean `BookModel`. This profile should be the single point of truth for translation, making it easy to audit and test.

### Phase 3: BLL Refactoring (Encapsulation Enforcement)
This phase focuses on modifying the core business logic service (`BooksService`) to strictly adhere to the new contract boundaries established in Phase 2.

**Actionable Steps:**
1.  **Update Service Interfaces (`IBooksService`):** Review all public methods in the BLL service interfaces (e.g., those implemented by `BooksService.cs`). Ensure that *all* method signatures accept and return only `BookModel` or other pure domain models, never DAL entities.
2.  **Refactor Service Implementation (`BookTracker.BLL/Services/BooksService.cs`):**
    *   When calling the DAL abstraction (`IBookDBManager`), ensure that the service receives raw data (DAL Entities).
    *   Immediately upon receiving data from `IBookDBManager`, use AutoMapper (via dependency injection) to translate this entire collection into a list of clean `BookModel` objects *before* any business logic is executed or results are returned.
    *   If a method requires input, validate and map the incoming BLL model parameters before passing them down for persistence via the DAL abstraction.
3.  **Isolate Persistence Logic:** Ensure that `BooksService.cs` only calls high-level methods on `IBookDBManager` (e.g., `AddBook(bookEntity)`), and does not contain any EF Core queries, context management, or complex SQL logic itself.

### Phase 4: UI Refactoring (Contract Adherence)
The UI layer must be updated to confirm it is only consuming the BLL contract (`BookModel`) and not relying on any internal structure of the DAL entities.

**Actionable Steps:**
1.  **Verify Service Consumption in `BlazorWebApp/Program.cs`**: Confirm that the dependency injection setup registers `IBooksService` (the BLL abstraction) as the public-facing contract, ensuring no direct registration or exposure of lower-level DAL services to the UI layer.
2.  **Audit UI Components (`BlazorWebApp/Components/Pages/Books/Details/AddBook.razor`)**: Review all components that consume `IBooksService`. Verify that they bind to and consume `BookModel` instances provided by the BLL service, not raw DAL entities. If a component needs to display complex information, ensure the BLL has already performed necessary transformations into presentation-friendly domain models.

## Anticipated Challenges
*   **Mapping Complexity:** If the DAL entities contain highly complex or deeply nested structures (like JSON columns or specialized EF Core types), mapping them cleanly to a simple `BookModel` may require introducing intermediate Data Transfer Objects (DTOs) within the BLL, which must be carefully managed to avoid reintroducing coupling.
*   **Refactoring Scope:** Ensuring *every* method in `BooksService.cs` is audited and updated to use the mapped `BookModel` instead of raw DAL entities will be time-consuming but critical for success.

### Critical Files for Implementation
These files are most critical as they represent the boundaries being enforced (DAL $\leftrightarrow$ BLL) and the consumers of those boundaries (UI).

*   `BookTracker.BLL/Services/BooksService.cs`
*   `BookTracker.BLL/Models/BookModel.cs`
*   `BlazorWebApp/AutoMapper/BooksProfile.cs`
*   `BookTracker.DAL/Abstractions/IBookDBManager.cs`
*   `BlazorWebApp/Components/Pages/Books/Details/AddBook.razor`