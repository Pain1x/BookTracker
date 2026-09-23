# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 📚 Project Architecture Overview

The BookTracker application follows a standard layered architecture, separating concerns into three primary projects:

1.  **`BookTracker.DAL` (Data Access Layer):**
    *   Handles all direct database interactions using an ORM (likely Entity Framework Core given the structure).
    *   Contains `Entities/`, `Models/`, and `Migrations/`.
    *   Interaction is managed through abstract interfaces in `Abstractions/` and concrete implementations in `DBContexts/` and `Services/`.
    *   **Key components:** Database context setup and raw data retrieval logic.

2.  **`BookTracker.BLL` (Business Logic Layer):**
    *   Contains the core business rules, use cases, and workflows that orchestrate data flow between the UI and the DAL.
    *   It depends on `BookTracker.DAL`'s abstractions.
    *   Logic is often segmented into `Services/` and complex background tasks in `BackgroundJobs/`.

3.  **`BlazorWebApp` (Presentation Layer):**
    *   The main application entry point, built using Blazor.
    *   This layer consumes the services exposed by `BookTracker.BLL` to render UI components (`Components/`) and handle user input.
    *   API communication is managed via `Controllers/`.

**Data Flow:** Client $\rightarrow$ `BlazorWebApp` Controllers $\rightarrow$ `BookTracker.BLL` Services $\rightarrow$ `BookTracker.DAL` Contexts $\rightarrow$ Database.

## 🛠️ Development Commands and Workflow

The primary commands for developing, testing, and running the application are typically found via the IDE's context menus or dedicated CLI scripts (which should be reviewed in the project root).

### Build & Run
*   **To build the solution:** Use the standard Build/Run button provided by the IDE on `BookTracker.sln`.
*   **To run the application (Development):** Right-click on `Program.cs` within `BlazorWebApp` and select 'Run' or use the configured launch profile. This will start the Blazor WebAssembly/Server hosting environment.

### Testing
*   **General Test Execution:** Run tests by selecting a test method in the project or file (e.g., in `BookTracker.BLL`) and invoking the 'Test' command from the IDE.
*   **Running a Single Test:** Right-click on a specific test method body/signature within a test class and select 'Run Test'. This provides focused feedback without running the whole test suite.

### Linting & Code Quality
*   **Linting:** Run code analysis (linting) across multiple files using the IDE's dedicated "Analyze" or "Inspect Code" functionality on selected file paths (`*.cs` files). For a batch of changes, consider leveraging `mcp_rider_lint_files`.

## 💡 Development Practices and Conventions
*   **Dependency Flow:** Dependencies must flow *inward*: `BlazorWebApp` $\rightarrow$ `BookTracker.BLL` $\rightarrow$ `BookTracker.DAL`. Components should never reference layers outside of their direct dependency scope (e.g., DAL should not know about Blazor types).
*   **Abstraction:** Always interact with services and data access via interfaces defined in the `Abstractions/` folders to ensure testability and loose coupling.

## 📁 Important Directories
*   `.gitignore`: Contains patterns for files/folders that Git should ignore (e.g., binaries, build outputs).
*   `docker-compose.yml`: Defines the service dependencies for local development environment setup (database services, API containers, etc.).