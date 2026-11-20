# BeFit

Dziennik treningowy: ASP.NET Core MVC (.NET 8), Entity Framework Core (SQLite), ASP.NET Core Identity z rolami.

## Uruchomienie

```
dotnet restore
dotnet run
```

Baza `befit.db` tworzy się automatycznie przy pierwszym uruchomieniu (razem z rolą i kontem administratora oraz kilkoma typami ćwiczeń).

Konto administratora (zmienne w `appsettings.json`, sekcja `Admin`):

- e-mail: `admin@befit.pl`
- hasło: `Admin123!`

## Funkcje

- Typy ćwiczeń: lista i szczegóły dla wszystkich, tworzenie/edycja/usuwanie tylko dla roli Administrator.
- Sesje treningowe i wykonane ćwiczenia: pełny CRUD, dostępne wyłącznie dla właściciela (UserId ustawiany automatycznie, cudze wpisy zwracają 404).
- Statystyki: ćwiczenia z ostatnich 28 dni pogrupowane wg typu (liczba wykonań, suma powtórzeń, średnie i maksymalne obciążenie).
