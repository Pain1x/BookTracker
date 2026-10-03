# Context
The primary goal is to resolve a critical database integrity error (`duplicate key value violates unique constraint "PK_Languages"`) occurring within the background job processor, specifically in `BookTranslationProcessor.cs`. This error happens because when creating translation records (Book, Author, Genre), a new `Language` entity is instantiated for every record, even if a language with that specific `LanguagePk` already exists in the database. EF Core then attempts to insert this duplicate language entity, violating the unique constraint on the `Languages` table.

# Recommended Approach
The solution is to modify the `EnsureTranslationAsync` method to first check for and retrieve the existing `Language` entity based on `job.TargetLanguage` (which corresponds to `LanguagePk`) *before* constructing or adding any translation record. This ensures that all translation entities reference an existing, single language record in the database, preventing the duplicate key violation.

# Critical Files
- `/home/deck/Code/BookTracker/BookTracker.DAL/Services/BookTranslationProcessor.cs`

# Implementation Details
# Context
The primary goal is to resolve critical database integrity errors occurring within the background job processor (`BookTranslationProcessor.cs`). Two issues were identified and addressed:
1.  **Language PK Violation:** A `duplicate key value violates unique constraint "PK_Languages"` error occurred because a new `Language` entity was instantiated for every translation record, violating the language table's unique constraint. This has been fixed by ensuring all translations reference an existing, pre-fetched `Language` entity.
2.  **Foreign Key Violation:** A subsequent `insert or update on table "BookTranslations" violates foreign key constraint "FK_BookTranslations_Books_BookPk"` error occurred when the job payload contained a zero/default GUID for `BookPk`. This indicates that the primary book entity was not persisted before its translation job was queued.

# Recommended Approach
The solution involves two parts:
1.  **Language Fix (Completed):** Modify `EnsureTranslationAsync` to accept and use a pre-fetched `Language` object, preventing duplicate language entries.
2.  **PK Validation (New):** Add defensive checks at the start of `ProcessTranslationAsync` to validate that all primary keys (`BookPk`, `AuthorPk`, `GenrePk`) in the incoming job payload are valid (non-zero/non-default GUIDs). If any PK is invalid, the job should fail early with a clear exception, preventing EF Core from attempting an insert with a non-existent foreign key.

# Critical Files
- `/home/deck/Code/BookTracker/BookTracker.DAL/Services/BookTranslationProcessor.cs`

# Implementation Details
1.  **PK Validation (New):** At the beginning of `ProcessTranslationAsync`, validate that all required primary keys (`job.BookPk`, `job.AuthorPk`, `job.GenrePk`) are valid GUIDs before proceeding with translation or database operations. If any key is invalid, throw a domain-specific exception indicating bad input data for the job.
2.  **Language Fix (Refined):** In `ProcessTranslationAsync`, fetch the target language entity once using `context.Set<Language>().FindAsync(job.TargetLanguage)` and pass this retrieved `Language` object into `EnsureTranslationAsync`. This ensures all translation entities reference an existing, single language record in the database.
3.  Modify `EnsureTranslationAsync` to accept a pre-fetched `Language` object as a parameter instead of fetching it internally. The method will then use this passed-in language object when constructing the translation entity and proceed with the transaction logic (check for existence, add, save changes, commit). This prevents transactional conflicts by ensuring all database operations are correctly scoped within the explicit transaction block started at the beginning of `EnsureTranslationAsync`.

# Verification
1. Run `dotnet build BookTracker.sln` to ensure compilation succeeds after changes.
2. Monitor job processing logs for the absence of both:
    - `Npgsql.PostgresException (0x80004005): 23505: duplicate key value violates unique constraint "PK_Languages"`
    - `insert or update on table "BookTranslations" violates foreign key constraint "FK_BookTranslations_Books_BookPk"`
3. Test the system by intentionally queuing a job with an invalid/zero PK to ensure it fails gracefully and early, without hitting the database layer for insertion.

# Verification
1. Run `dotnet build BookTracker.sln` to ensure compilation succeeds after changes.
2. Monitor job processing logs for the absence of the `Npgsql.PostgresException (0x80004005): 23505: duplicate key value violates unique constraint "PK_Languages"` error when jobs are processed.