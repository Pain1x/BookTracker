# Project Context: BookTracker

## Overview
Book tracking web app built with C#, .NET 8, Blazor Server, EF Core with PostgreSQL (Npgsql), AutoMapper, and built-in .NET DI.

## Structure and dependencies
BookTracker/
├── BlazorWebApp/          # Presentation layer (Blazor Server UI)
│   ├── Components/         # Reusable UI components (e.g., PaginatedList, NavMenu)
│   ├── Configurations/     # Service and database configuration extensions
│   ├── Pages/              # Razor pages for specific views (e.g., Books.razor)
│   └── wwwroot/            # Static assets (CSS, JS, images)
├── BookTracker.BLL/       # Business logic layer
│   ├── Abstractions/      # Service contract interfaces (e.g., IBooksService)
│   ├── Models/             # Domain models (BookModel, AuthorModel)
│   └── Services/           # Business logic implementations (BooksService)
├── BookTracker.DAL/       # Data access layer
│   ├── Abstractions/      # Data access interfaces (e.g., IBookDBManager)
│   ├── DBContexts/         # EF Core DbContext setup
│   ├── Entities/           # Database entities (Author, Book, Genre)
│   └── DbManagers/         # Data access implementations (BookDbManager)
├── BookTracker.Common/    # Shared library for common types and enums
│   ├── Enums/              # Shared enumerations (e.g., StatusEnum)
│   └── BookTracker.Common.csproj
├── BookTraker.Automapper/  # Centralized AutoMapper profiles
│   ├── AutoMapper/         # All mapping profile definitions
│   └── BookTraker.Automapper.csproj
├── BookTracker.Jobs/      # Background worker services and scheduled tasks
│   ├── Models/             # Data models specific to job payloads (e.g., JobStatus)
│   └── BookTracker.Jobs.csproj
├── BlazorWebApp.sln       # Solution file
└── AGENTS.md               # This file

## Architecture & Layers
1.  **Presentation (`BlazorWebApp`):** UI layer. Must not use DbContext or DAL types directly; always go through BLL services.
2.  **Business Logic (`BookTracker.BLL`):** Core logic. References `BookTracker.Automapper` and `BookTracker.DAL`.
3.  **Data Access (`BookTracker.DAL`):** Persistence via EF Core/PostgreSQL. Uses `IDbContextFactory<T>` to create short-lived contexts per operation.
4.  **Shared Components (`BookTracker.Common`):** Shared types and constants used by all layers.
5.  **AutoMapper (`BookTraker.Automapper`):** Centralized mapping profiles between DAL entities and BLL models.
6.  **Asynchronous Tasks (`BookTracker.Jobs`):** Background worker services executing business logic asynchronously via `BookTracker.BLL`.
7.  **Testing (`BookTracker.Tests`):** Unit testing project for the DAL using xUnit, Moq, and EF Core In-Memory Database.

## Key Rules & Constraints
- **Context Management:** Never inject or store a `DbContext` directly. Use `IDbContextFactory<T>` and create a short-lived context per operation: `await using var context = await factory.CreateDbContextAsync();`.
- **Error Handling:** All exceptions from `BookTracker.DAL` must be caught and translated by `BookTracker.BLL` into custom, domain-specific exception types before reaching the UI.
- **Migrations:** Never edit migrations by hand or touch `bin/` and `obj/`.
- **Dependency Flow:** 
    - `BlazorWebApp` $\rightarrow$ `BookTracker.BLL`
    - `BookTracker.BLL` $\rightarrow$ (`BookTracker.Automapper`, `BookTracker.DAL`)
    - `BookTracker.Jobs` $\rightarrow$ `BookTracker.BLL`
## New Features & Logic (Added Oct 2026)
- [[searchable-dropdowns]]
- [[book-management]]
- [[translation-system]]
- [[author-genre-management]]
- [[background-jobs]]
- 