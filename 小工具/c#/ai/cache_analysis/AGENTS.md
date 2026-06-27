# AGENTS.md

## Purpose
This repository contains a C# WinForms tool that generates a low-token Markdown analysis cache for another C# project.

## Primary Workflow
1. Launch the WinForms app.
2. Select the target project folder.
3. Choose the output folder.
4. Generate the analysis cache.

## Agent Rule
- Prefer reading `analysis-cache.md` first when it exists.
- Treat `analysis-cache.md` as the root index only.
- Use `analysis-cache.cache/tree/` to navigate by directory first.
- Use the full `*.cs` index in `analysis-cache.md` when you do not know the file path.
- Open a per-file cache entry under `analysis-cache.cache/files/` only for paths relevant to the task.
- Re-read original source files only when the per-file cache is insufficient.
- Regenerate the cache after meaningful project structure changes.

## Expected Cache Content
- Main index summary
- Full `*.cs` index table
- Tree cache markdown by directory
- Per-file cache markdown entries
- Incremental cache state
- Compact C# symbol index with hashes

## Notes
- Ignore `bin`, `obj`, `.vs`, and `.git` content during analysis.
- Keep generated Markdown concise to reduce token usage in downstream agent runs.
