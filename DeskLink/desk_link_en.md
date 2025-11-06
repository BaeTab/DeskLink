# DeskLink – Internal Systems Launcher Hub

Paste this prompt into your coding AI to generate a **production‑grade** desktop app using C# WPF + DevExpress. The goal is a shippable app, not an MVP.

---

## Engineering Prompt (English)

You are a senior C# WPF architect and DevExpress expert. Implement **DeskLink – Internal Systems Launcher Hub** using .NET 8, C# 12, WPF, and DevExpress v25.1.5 (must be primary UI toolkit).

### 0) One‑liner
A launcher hub to open and manage internal systems (Mail, Intranet, ERP, DB, NAS, BI, Git, Jenkins, etc.) from one screen with **Tiles/List/Search**.

### 1) Core Features
1. Link Types
   - URL (http/https), File/Folder (UNC/local path), Executable (.exe with args), RDP file (.rdp), SSH/Telnet (invoke external client), Custom schemes (e.g., mailto:)
   - Per‑link metadata: icon, color, tags, description, owner, last health‑check time
2. Views
   - **Tile Board** (DevExpress TileControl), **List/Grid** (GridControl), **Accordion Category View** (AccordionControl)
   - Favorites / Recent / Recommendations (rule‑based, local)
3. Search & Filter
   - Instant search (case‑insensitive Contains), tag/category/department filters, sort by last used/frequency/name
4. Profiles / Environments
   - **Profiles (Office/Remote/Site)** that affect visibility/credentials/paths
   - Async network availability checks for UNC/HTTP and status badges in UI
5. Roles / Access
   - Roles (Admin/Editor/Viewer) and per‑link **visibility rules** (Role/Team/OU tags)
   - Sensitive links: confirmation dialog and optional extra password (SecureString)
6. Import/Export
   - CSV/JSON import/export, clipboard parser from existing office docs
7. Drag & Drop
   - Drag targets from Desktop/File Explorer/Browser to create links and infer metadata
8. Hotkeys & Quick Launch
   - Global hotkey (e.g., Alt+Space) for **Quick Open** panel → search then Enter to run
   - Per‑item hotkey mapping (Ctrl+1…Ctrl+9)
9. Health‑Check
   - Scheduled link health checks (HTTP 200, path exists, port open)
   - On failure: badges/tooltips/logs and admin report (PDF/Excel)
10. Distribution / Sync
   - Local SQLite store + optional **shared repository (JSON/SQLite)** sync (read‑only or two‑way)
   - Conflict resolution (latest‑wins / admin approval queue)

### 2) Non‑functional Requirements
- .NET 8 / AnyCPU / Windows 10+; DPI aware (PerMonitorV2); Dark/Light themes (DevExpress themes)
- Responsiveness: all IO/network async; no UI freezes
- Performance: 3,000 links with <200ms search/filter; tile virtualization
- Reliability: exception logging (Serilog) + recovery (auto‑retry/fallback); crash‑recovery
- Security: never persist credentials; DPAPI for sensitive local fields; mask secrets in logs
- Accessibility: keyboard navigation; proper AutomationProperties/tooltips
- Diagnostics: rolling file logs; optional user feedback logs

### 3) Solution Layout
```
DeskLink.sln
  src/
    DeskLink.App/            (WPF app; Prism or DevExpress MVVM)
    DeskLink.Core/           (domain models, service interfaces, shared utils)
    DeskLink.Infrastructure/ (SQLite, file IO, health‑check, sync impl)
    DeskLink.Tests/          (xUnit)
  assets/
    icons/ (SVG/PNG; DevExpress SvgImageSource)
    fonts/
  build/
    wix/ | msix/             (installer scripts)
```

### 4) UI Composition
- MainWindow: Custom TitleBar (DevExpress ThemedWindow + WindowChrome), RibbonControl or Fluent UI
- Left Pane: AccordionControl (categories/tags/role filters)
- Center: TileControl (large/medium/small tiles) ↔ GridControl (list view) toggle
- Right Pane: Property panel (PropertyGridControl) for selected link properties/permissions/health
- Bottom: StatusBar (health‑check progress, sync state, notifications)
- Quick Open: ThemedDialog + SearchLookUpEdit

### 5) Data Model (summary)
```csharp
class LinkItem {
  Guid Id; string Name; string Description;
  LinkType Type; // Url, File, Folder, Exe, Rdp, Ssh, Custom
  string Target; string Arguments; string WorkingDir;
  string IconKey; string ColorHex; string[] Tags; string Category; string Owner;
  string VisibilityRule; DateTime CreatedAt; DateTime UpdatedAt; DateTime? LastCheckedAt;
  LinkHealthStatus Health;
}
```

### 6) Storage & Schema
- SQLite (local) via EF Core 8, code‑first
```sql
CREATE TABLE Links(
  Id TEXT PRIMARY KEY,
  Name TEXT NOT NULL,
  Description TEXT,
  Type INTEGER NOT NULL,
  Target TEXT NOT NULL,
  Arguments TEXT,
  WorkingDir TEXT,
  IconKey TEXT, ColorHex TEXT,
  Tags TEXT,
  Category TEXT,
  Owner TEXT,
  VisibilityRule TEXT,
  CreatedAt TEXT NOT NULL,
  UpdatedAt TEXT NOT NULL,
  LastCheckedAt TEXT,
  Health INTEGER NOT NULL
);
CREATE INDEX IX_Links_Name ON Links(Name);
CREATE INDEX IX_Links_Tags ON Links(Tags);
CREATE INDEX IX_Links_Category ON Links(Category);
```
- JSON snapshot (for sync)
```json
{
  "version":"1.0",
  "updated":"2025-11-06T00:00:00Z",
  "links":[{"id":"...","name":"ERP","type":"Url","target":"https://erp.company"}]
}
```

### 7) Services
- ILinkRepository (CRUD, search/filter, bulk import/export)
- IHealthCheckService (HTTP/Path/Port checks, polling schedule)
- ISyncService (shared store pull/push, conflict resolution)
- IShortcutService (global hotkeys, Quick Open)
- IIconService (SVG/PNG loading, theme coloring)

### 8) MVVM Design
- ViewModels: MainVm, BoardVm (tile/grid switch), DetailVm, QuickOpenVm, SettingsVm, ImportExportVm
- Commands: NewLink, EditLink, DeleteLink, Duplicate, RunLink, ToggleFavorite, HealthCheckNow, SyncNow
- Messaging: EventAggregator for state/notification broadcast

### 9) Settings (Options)
```json
{
  "theme":"Office2019Black",
  "hotkeys":{"QuickOpen":"Alt+Space"},
  "healthCheck":{"intervalMinutes":10,"timeoutMs":3000},
  "sync":{"mode":"Pull","sharedPath":"\\\\NAS\\DeskLink\\repo.json"},
  "profiles":[{"name":"Office","enabled":true},{"name":"Remote","enabled":false}]
}
```

### 10) Execution Rules
- Url → default browser (Process.Start)
- File/Folder → Explorer/ShellExecute
- Exe → arguments/working dir; optional "Run as admin"
- Rdp/SSH → invoke registered external client with args

### 11) Health‑Check Details
- HTTP: HEAD/GET; 200–399 OK; 401/403 flagged
- FILE/FOLDER: Directory/File.Exists
- PORT: TcpClient.ConnectAsync(host, port) with timeout
- Cache results; delayed UI badge updates

### 12) UX Details
- Custom titlebar (icon, app name, min/max/close, drag move)
- First‑run onboarding with sample links (Mail/ERP/NAS)
- Multi‑select edit; bulk tag/color updates
- Tile size presets (L/M/S) with layout persistence

### 13) Theme & Icons
- DevExpress themes (Office2019, Fluent, Visual Studio)
- SVG icons with theme‑aware coloring

### 14) Logging & Reports
- Serilog rolling file
- Admin reports: Top N failing links (last 7 days) → PDF/Excel

### 15) Testing & Quality
- xUnit unit tests: repository, health‑check, sync conflicts
- Optional UI tests: Playwright for .NET (WPF) or TestStack.White
- Static analysis: Roslyn analyzers, StyleCop, nullable enable

### 16) Packaging
- MSIX or WiX installer
- Internal update model: version JSON on shared path; user‑initiated updates

### 17) Definition of Done
- 3,000 links; search <200ms; first screen <2s
- Execution success/failure logged; actionable error prompts
- DPI 125–200% renders correctly on multi‑monitor
- Visibility rules enforce correctly by role/team

### 18) Seed Data
- ERP, GroupMail, Intranet, NAS Projects, Jenkins, Git, BI Dashboard, VPN Portal, etc. (≥20)

### 19) XAML Sketch (excerpt)
```xml
<dx:ThemedWindow ...>
  <DockPanel>
    <dxnav:AccordionControl DockPanel.Dock="Left" ItemsSource="{Binding.SideFilters}"/>
    <Grid>
      <ContentControl Content="{Binding CurrentView}" />
    </Grid>
    <StatusBar DockPanel.Dock="Bottom">...</StatusBar>
  </DockPanel>
</dx:ThemedWindow>
```

### 20) Security & Compliance
- Do not persist passwords/tokens; integrate with OS credential manager or prompt on demand
- Anonymization option for host/path info in logs/reports

Generate complete, organized source code and build scripts that meet the above. Provide README with build/deploy/seed/upgrade guides.

