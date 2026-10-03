BookTracker

Book tracking web app. C#, .NET 8, Blazor Server, EF Core with PostgreSQL (Npgsql), AutoMapper, built-in .NET DI.

Structure and dependencies
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

Dependency flow: The application follows a layered architecture with specialized modules:
1.  **Presentation:** `BlazorWebApp` $\rightarrow$ `BookTracker.BLL`. (References DAL only for DI registration).
2.  **Business Logic:** `BookTracker.BLL` $\rightarrow$ (`BookTracker.Automapper`, `BookTracker.DAL`).
3.  **Data Access:** `BookTracker.DAL` handles persistence via EF Core/PostgreSQL.
4.  **Shared Components:** All layers reference `BookTracker.Common` for shared types and constants.
5.  **Asynchronous Tasks:** `BookTracker.Jobs` $\rightarrow$ `BookTracker.BLL`. (Jobs execute business logic asynchronously).

UI code (pages, components) must not use the DbContext or DAL types directly; go through BLL services.
Mapping between DAL entities and BLL models is done via profiles defined in `BookTracker.Automapper`.
The DbContext is used through IDbContextFactory<T>. In DAL code, create a short-lived context per operation (await using var context = await factory.CreateDbContextAsync();). Never inject or store a DbContext directly, and never share one across operations or components.

Never edit migrations by hand or touch bin/ and obj/.
Commands

The solution is ./BookTracker.sln.

Build: dotnet build BookTracker.sln
Run: dotnet run --project BlazorWebApp/BlazorWebApp.csproj
Test: dotnet test BookTracker.sln (currently there is no test project)
Clean: dotnet clean BookTracker.sln
Add migration: dotnet ef migrations add <MigrationName> --project BookTracker.DAL --startup-project BlazorWebApp
Apply migrations: dotnet ef database update --project BookTracker.DAL --startup-project BlazorWebApp

Working with files
Before editing an existing file, read it with Read in this session.
Use Edit for changes. old_string must be short, unique in the file, and match character for character (indentation, tabs, line endings). Do not copy line numbers from Read output.
If Edit fails, re-read the file and retry with a more specific old_string. Do not rewrite a whole file unless it is very small.
If `Edit` fails twice on the same file, do not keep guessing. Re-read the file and use a single-line `old_string`. For small files (like this one), use `Write` for a full rewrite and verify with `git diff`.
Create new files with Write, using a path relative to the repository root. Check with ls first that the directory exists and the file does not.
Do not modify files through shell commands (sed, >, heredocs) when Edit/Write are available.
SDK-style .csproj files include new .cs files automatically; do not edit a .csproj just to add a file.

Error Handling: All exceptions originating in `BookTracker.DAL` must be caught and translated by the `BookTracker.BLL` layer into custom, domain-specific exception types before being exposed to the UI or other services. This prevents leaking database implementation details.

Follow .editorconfig for line endings and encoding.
Make one logical change at a time, then check it with git diff --stat.
After code changes, run dotnet build and dotnet test once tests exist. The task is not done until the build passes.

Git
Branch names: feature/<short-name>, fix/<short-name>.
Commit messages: short, imperative, English.
Commit when a task is complete; run git push only when asked.
