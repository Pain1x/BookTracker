# Project Context: BookTracker

## Overview
Book tracking web app built with C#, .NET 8, Blazor Server, EF Core with PostgreSQL (Npgsql), AutoMapper, and built-in .NET DI.

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

## Common Commands
- **Build:** `dotnet build BookTracker.sln`
- **Run:** `dotnet run --project BlazorWebApp/BlazorWebApp.csproj`
- **Test:** `dotnet test BookTracker.sln`
- **Add Migration:** `dotnet ef migrations add <MigrationName> --project BookTracker.DAL --startup-project BlazorWebApp`
- **Apply Migrations:** `dotnet ef database update --project BookTracker.DAL --startup-project BlazorWebApp`

## Git Workflow
- Branch names: `feature/<short-name>`, `fix/<short-name>`
- Commit messages: short, imperative, English.

## New Features & Logic (Added Oct 2026)
- [[searchable-dropdowns]]
- 