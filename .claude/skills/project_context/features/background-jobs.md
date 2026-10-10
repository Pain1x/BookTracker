# Feature: Background Job Processing

## Overview
Hangfire-based background job system for handling asynchronous tasks, primarily translation processing. Allows the UI to remain responsive while heavy operations complete.

## Architecture & Implementation
- **Job Scheduler:** 
    - `HangfireBookTranslationJobScheduler` enqueues translation jobs to Hangfire.
    - Method: `Enqueue(BookTranslationJob)` uses `BackgroundJobClient.Enqueue` to schedule jobs.
- **Job Processor:** 
    - `BookTranslationProcessor` processes translation jobs.
    - Method: `ProcessTranslationAsync(BookTranslationJob)` orchestrates translation API calls and database persistence.
- **Job Models:** 
    - `BookTranslationJob` contains job payload: AuthorPk, AuthorName, BookPk, Title, GenrePk, Genre, TargetLanguage.

## Key Constraints & Constants
- **Hangfire Storage:** Uses PostgreSQL for job persistence with Hangfire.PostgreSql provider.
- **Dashboard Authorization:** Custom `HangfireDashboardAuthorizationFilter` secures the Hangfire dashboard.
- **Async Processing:** All translation operations are queued, never blocking the request thread.
- **Transaction Safety:** Processor uses database transactions with rollback on errors.

## Testing
- Integration tests verify job enqueueing and processing flow.
- Hangfire dashboard can be used to monitor job execution and failures.
