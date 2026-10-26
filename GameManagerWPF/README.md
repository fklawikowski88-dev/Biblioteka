# GameVault — Menedżer gier (WPF, .NET 8)

## Wymagania
- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022 z workloadem „Programowanie aplikacji klasycznych .NET” albo terminal z .NET SDK

## Uruchomienie
1. Rozpakuj folder `GameManagerWPF`.
2. Otwórz `GameManagerWPF.csproj` w Visual Studio 2022.
3. Wybierz konfigurację Debug i uruchom klawiszem F5.

Alternatywnie w terminalu, w folderze projektu:
```powershell
dotnet restore
dotnet build
dotnet run
```

## Funkcje
- Trzy widoki: pulpit, biblioteka gier i statystyki.
- Okno dodawania/edycji otwierane przez `ShowDialog()`.
- CRUD: dodawanie, wyświetlanie, edycja, usuwanie.
- Wyszukiwanie po tytule, gatunku i platformie.
- Sortowanie po tytule, roku wydania i ocenie.
- Walidacja tytułu, roku (1970–2100) i oceny (0–10).
- Zapis/odczyt danych JSON.
- Potwierdzenie usuwania, obsługa błędów i komunikaty dla użytkownika.
- MVVM: model `Game`, `MainViewModel`, widoki XAML, `RelayCommand`.
- Spójny ciemny styl z `ResourceDictionary`, stylem kontrolek i `DataTemplate`.

## Lokalizacja danych
Dane są zapisywane w:
`%LOCALAPPDATA%\GameManagerWPF\games.json`

## Uwagi
- Interfejs i komunikaty są po polsku.
- Edycja odbywa się na kopii obiektu, więc anulowanie dialogu nie zmienia zapisanej gry.
- W przypadku błędnego pliku JSON aplikacja wyświetli komunikat błędu zamiast po cichu nadpisywać plik.
