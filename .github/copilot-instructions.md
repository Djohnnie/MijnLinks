# Copilot Instructions for MijnLinks

## Build & Run

```bash
dotnet build
dotnet run
```

Set `PATH_TO_LINKS_FILE` to the full path of your `links.json` before running.

## Docker

```bash
docker build -t mijnlinks .
docker run -d -p 8080:8080 -v /path/to/data:/data -e PATH_TO_LINKS_FILE=/data/links.json mijnlinks
```

## Architecture

MijnLinks is a .NET 10 Blazor Server application using MudBlazor for UI. It displays categorized links as clickable tiles, with link data read from a local JSON file (mounted via Docker volume).

### Data Flow

`Local JSON file (links.json)` → `LinkService` → Blazor pages render `LinkTile` components.

### Key Components

- **`Services/LinkService.cs`** — Reads and parses the JSON file from the path configured via `PATH_TO_LINKS_FILE`. All link queries (all, by category, important-only) go through this service.
- **`Services/ThemeService.cs`** — Manages light/dark mode preference, persisted in the browser's `localStorage` under the key `"darkMode"`.
- **`Models/Link.cs`** — Link model: `Title`, `Url`, `SvgIcon` (inline SVG string), `IconColor`, `Category`, `IsImportant`.
- **`Components/Shared/LinkTile.razor`** — Reusable card component that renders a clickable tile with an inline SVG icon.
- **`Components/Layout/MainLayout.razor`** — MudBlazor layout with a dynamic nav drawer (categories populated from link data) and a theme toggle button.

### Pages

- **`/`** (Dashboard) — Shows links where `IsImportant == true`.
- **`/category/{CategoryName}`** — Shows links filtered by category. Categories appear dynamically in the nav menu.

## Configuration

All configuration uses environment variables:

| Variable | Purpose | Required |
|---|---|---|
| `PATH_TO_LINKS_FILE` | Full path to the `links.json` file (use Docker volume mount) | Yes |

## Link JSON Schema

The `links.json` file must be an array of objects:

```json
[
  {
    "title": "GitHub",
    "url": "https://github.com",
    "svgIcon": "<svg>...</svg>",
    "iconColor": "#181717",
    "category": "Development",
    "isImportant": true
  }
]
```

- `svgIcon` — Full inline SVG markup (single quotes for attributes). Rendered directly in the tile.
- `iconColor` — Applied as `fill` and `color` to the SVG container.
- `isImportant` — When `true`, the link appears on the Dashboard.
- `category` — Used for grouping; each unique category gets a nav menu entry and dedicated page.

## Conventions

- UI components use MudBlazor (`MudCard`, `MudGrid`, `MudNavLink`, etc.) — do not mix in raw Bootstrap or other CSS frameworks.
- Interactive render mode is Server-side (`AddInteractiveServerComponents`).
- Theme state is scoped per-circuit via `ThemeService` (registered as `Scoped`).
- The `LinkTile` navigates via `NavigationManager.NavigateTo(url, forceLoad: true)` to open external URLs.
- Deployed as a Docker container using the ASP.NET 10 runtime image; links.json is provided via volume mount.
