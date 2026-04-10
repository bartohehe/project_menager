# Project Manager — WPF Desktop Application

## Stack
- C# WPF (.NET 8), MVVM with CommunityToolkit.Mvvm
- MongoDB (MongoDB.Driver 2.28.0 + GridFS)
- DPAPI for connection string encryption

## Build & Run
```bash
dotnet restore src/ProjectManager/ProjectManager.csproj
dotnet build src/ProjectManager/ProjectManager.csproj
dotnet run --project src/ProjectManager/ProjectManager.csproj
```

## Architecture
- **MVVM**: ViewModels in `ViewModels/`, Views in `Views/`, Models in `Models/`
- **Navigation**: ViewModel-first via `NavigationService` + implicit DataTemplates in `App.xaml`
- **DI**: `Microsoft.Extensions.DependencyInjection` configured in `App.xaml.cs`
- **Repository pattern**: `IProjectRepository`, `ICategoryRepository` for data access
- **GridFS**: `IFileStorageService` for file storage (photos, attachments)
- **Security**: `IConnectionStringProtector` with DPAPI, `InputValidator` for sanitization

## Key conventions
- Polish UI labels, English code
- All colors/brushes defined in `Helpers/Theme.cs` and `Themes/Colors.xaml`
- Custom window chrome (WindowStyle=None + WindowChrome)
- Dark theme: BgDark #0D111B, BgPanel #141B2A, Accent #00BC8C
- Embedded subdocuments in MongoDB (timeline, sections, attachment metadata inside project document)
- Binary files in GridFS, metadata links back via projectId

## MongoDB collections
- `projects` — main documents with embedded timeline, sections, attachments
- `categories` — project categories with color/icon
- `fs.files` + `fs.chunks` — GridFS for binary files
