Commands
Build: dotnet build BookTracker.sln
Run: dotnet run --project BlazorWebApp/BlazorWebApp.csproj
Test: dotnet test BookTracker.sln (currently there is no test project)
Clean: dotnet clean BookTracker.sln
Add migration: dotnet ef migrations add <MigrationName> --project BookTracker.DAL --startup-project BlazorWebApp
Apply migrations: dotnet ef database update --project BookTracker.DAL --startup-project BlazorWebApp

Working with files
Before editing an existing file, read it with Read in this session.Use Edit for changes. old_string must be short, unique in the file, and match character for character (indentation, tabs, line endings). Do not copy line numbers from Read output.If Edit fails, re-read the file and retry with a more specific old_string. Do not rewrite a whole file unless it is very small.If `Edit` fails twice on the same file, do not keep guessing. Re-read the file and use a single-line `old_string`. For small files (like this one), use `Write` for a full rewrite and verify with `git diff`.Create new files with Write, using a path relative to the repository root. Check with ls first that the directory exists and the file does not.Do not modify files through shell commands (sed, >, heredocs) when Edit/Write are available.SDK-style .csproj files include new .cs files automatically; do not edit a .csproj just to add a file.
Follow .editorconfig for line endings and encoding.
Make one logical change at a time, then check it with git diff --stat.
After code changes, run dotnet build and dotnet test once tests exist. The task is not done until the build passes.

When you need to load context use for this skill - [[.claude/skills/project_context/SKILL|SKILL]] and if you haven't found what you were looking for use your tools and inform user that you haven't find something there.