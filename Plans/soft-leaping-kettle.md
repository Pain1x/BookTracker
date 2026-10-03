# Plan: Update CLAUDE.md for New Modular Structure

## Context
The BookTracker project is being modularized with three new projects: `BookTracker.Jobs`, `BookTracker.Common`, and `BookTraker.Automapper`. This change aims to improve separation of concerns, centralize shared types, isolate background processing, and standardize mapping logic across the application. The existing architecture (BlazorWebApp $\rightarrow$ BookTracker.BLL $\rightarrow$ BookTracker.DAL) is being extended rather than replaced.

## Goal
Update `CLAUDE.md` to accurately reflect this new modular structure, including:
1.  Defining the role and dependency of `BookTracker.Common`.
2.  Formalizing the use of `BookTraker.Automapper` for centralized mapping logic.
3.  Documenting the existence and interaction pattern of `BookTracker.Jobs` (Worker Services).
4.  Updating the overall Dependency Flow diagram/description to incorporate these new layers.

## Implementation Steps & Design Approach

### Step 1: Update Structure and Dependencies in CLAUDE.md
I will revise the "Structure and dependencies" section to include the three new projects and clarify their roles.

*   **BookTracker.Common:** Will be defined as a foundational library, referenced by all other layers for shared types (e.g., Enums, basic models).
*   **BookTraker.Automapper:** Will be defined as a dedicated mapping layer, referenced primarily by `BookTracker.BLL`. This formalizes the rule that mapping logic should not live in BLL or DAL directly.
*   **BookTracker.Jobs:** Will be defined as an external worker service, which interacts with `BookTracker.BLL` to execute business tasks asynchronously.

### Step 2: Refine Dependency Flow Rule
The existing flow (`BlazorWebApp -> BookTracker.BLL -> BookTracker.DAL`) needs refinement. The new rule will be:

*   **Core Flow:** `BlazorWebApp` $\rightarrow$ `BookTracker.BLL` $\rightarrow$ (`BookTraker.Automapper` / `BookTracker.DAL`).
*   **Shared Types:** All projects reference `BookTracker.Common`.
*   **Asynchronous Flow:** `BookTracker.Jobs` $\rightarrow$ `BookTracker.BLL` (to trigger/process tasks).

### Step 3: Review and Refine Existing Rules
I will review existing rules to ensure they are consistent with the new structure, specifically:
*   The rule about using AutoMapper must now explicitly point to `BookTraker.Automapper`.
*   The rule about DbContext usage remains critical but needs to be reinforced that DAL is only accessed via BLL services (which may call jobs).

### Verification Plan
1.  **Code Review:** After implementing the changes, run `dotnet build BookTracker.sln` to ensure all new project references and dependencies are valid.
2.  **Documentation Check:** Verify that the updated `CLAUDE.md` accurately reflects the intended modularity.
3.  **Test (Conceptual):** Confirm that a simple task involving a job (e.g., triggering a translation job) can flow correctly from BLL $\rightarrow$ Jobs $\rightarrow$ DAL, using common types and mapping profiles.

This plan focuses on updating documentation (`CLAUDE.md`) to match the existing file structure, as requested. No code changes are planned in this phase.