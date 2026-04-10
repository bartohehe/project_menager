# Project Manager

Desktopowa aplikacja do zarządzania projektami, zbudowana w technologii WPF (.NET 8) z bazą danych MongoDB.

## Funkcje

- Tworzenie i zarządzanie projektami z kategoriami, tagami i statusami
- Oś czasu projektu z kamieniami milowymi
- Sekcje i notatki w obrębie projektu
- Załączniki i zdjęcia przechowywane w GridFS
- Szyfrowanie połączenia z bazą danych (DPAPI)
- Ciemny motyw z niestandardowym chrome okna

## Stack technologiczny

- **Frontend**: C# WPF (.NET 8), MVVM z CommunityToolkit.Mvvm
- **Baza danych**: MongoDB (MongoDB.Driver 2.28.0) + GridFS dla plików binarnych
- **Bezpieczeństwo**: DPAPI do szyfrowania connection stringa

## Wymagania

- .NET 8 SDK
- MongoDB (lokalne lub zdalne)

## Uruchomienie

```bash
# Przywróć zależności
dotnet restore src/ProjectManager/ProjectManager.csproj

# Zbuduj projekt
dotnet build src/ProjectManager/ProjectManager.csproj

# Uruchom aplikację
dotnet run --project src/ProjectManager/ProjectManager.csproj
```

Przy pierwszym uruchomieniu aplikacja poprosi o podanie connection stringa do MongoDB — zostanie on zaszyfrowany i zapisany lokalnie za pomocą DPAPI.

## Struktura projektu

```
src/ProjectManager/
├── Models/          # Modele danych (Project, Category, ...)
├── ViewModels/      # Logika widoków (MVVM)
├── Views/           # Okna i kontrolki XAML
├── Services/        # Repozytorium, GridFS, nawigacja, DI
├── Helpers/         # Theme, walidacja, konwertery
├── Controls/        # Własne kontrolki WPF
└── Themes/          # Słowniki zasobów XAML (kolory, style)
```

## Kolekcje MongoDB

| Kolekcja | Opis |
|---|---|
| `projects` | Główne dokumenty z osadzoną osią czasu, sekcjami i metadanymi załączników |
| `categories` | Kategorie projektów (kolor, ikona) |
| `fs.files` / `fs.chunks` | GridFS — pliki binarne (zdjęcia, załączniki) |

## Motyw

Ciemny motyw z paletą:
- Tło: `#0D111B` / `#141B2A`
- Akcent: `#00BC8C`