using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GameManagerWPF.Commands;
using GameManagerWPF.Models;
using GameManagerWPF.Services;
using GameManagerWPF.Views;

namespace GameManagerWPF.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly JsonDataService _dataService = new();
    private string _currentView = "Home";
    private string _searchText = "";
    private string _selectedSort = "Tytuł A–Z";
    private Game? _selectedGame;
    private string _statusMessage = "Gotowe";

    public ObservableCollection<Game> Games { get; } = new();
    public ICollectionView FilteredGames { get; }
    public string[] SortOptions { get; } = { "Tytuł A–Z", "Tytuł Z–A", "Rok: najnowsze", "Ocena: najwyższa" };

    public string CurrentView { get => _currentView; set { if (SetProperty(ref _currentView, value)) OnPropertyChanged(nameof(IsHome)); } }
    public bool IsHome => CurrentView == "Home";
    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) FilteredGames.Refresh(); }
    }
    public string SelectedSort
    {
        get => _selectedSort;
        set { if (SetProperty(ref _selectedSort, value)) ApplySort(); }
    }
    public Game? SelectedGame { get => _selectedGame; set => SetProperty(ref _selectedGame, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public int TotalGames => Games.Count;
    public int CompletedGames => Games.Count(g => g.Status == "Ukończona");
    public int ToPlayGames => Games.Count(g => g.Status == "Do ogrania");
    public string StoragePath => _dataService.StoragePath;

    public ICommand NavigateCommand { get; }
    public ICommand AddGameCommand { get; }
    public ICommand EditGameCommand { get; }
    public ICommand DeleteGameCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand RefreshCommand { get; }

    public MainViewModel()
    {
        FilteredGames = CollectionViewSource.GetDefaultView(Games);
        FilteredGames.Filter = item => item is Game game &&
            (string.IsNullOrWhiteSpace(SearchText) ||
             game.Title.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
             game.Genre.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
             game.Platform.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase));
        NavigateCommand = new RelayCommand(p => CurrentView = p?.ToString() ?? "Home");
        AddGameCommand = new RelayCommand(_ => EditGame(null));
        EditGameCommand = new RelayCommand(_ => EditGame(SelectedGame), _ => SelectedGame != null);
        DeleteGameCommand = new RelayCommand(_ => DeleteSelectedGame(), _ => SelectedGame != null);
        SaveCommand = new RelayCommand(async _ => await SaveAsync());
        RefreshCommand = new RelayCommand(async _ => await LoadAsync());
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            var loaded = await _dataService.LoadAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Games.Clear();
                foreach (var game in loaded) Games.Add(game);
                OnCountsChanged();
                FilteredGames.Refresh();
                StatusMessage = $"Wczytano {Games.Count} gier";
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Nie udało się odczytać danych.\nSzczegóły: {ex.Message}", "Błąd odczytu",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            await _dataService.SaveAsync(Games);
            StatusMessage = $"Zapisano dane: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Nie udało się zapisać danych.\nSzczegóły: {ex.Message}", "Błąd zapisu",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EditGame(Game? existing)
    {
        var draft = existing?.Copy() ?? new Game();
        var window = new GameEditWindow(draft) { Owner = Application.Current.MainWindow };
        if (window.ShowDialog() != true) return;

        if (existing == null)
        {
            Games.Add(draft);
            SelectedGame = draft;
        }
        else
        {
            existing.Title = draft.Title;
            existing.Genre = draft.Genre;
            existing.Platform = draft.Platform;
            existing.ReleaseYear = draft.ReleaseYear;
            existing.Rating = draft.Rating;
            existing.Status = draft.Status;
            existing.Notes = draft.Notes;
        }
        OnCountsChanged();
        FilteredGames.Refresh();
        _ = SaveAsync();
    }

    private void DeleteSelectedGame()
    {
        if (SelectedGame == null) return;
        var result = MessageBox.Show($"Czy na pewno usunąć grę „{SelectedGame.Title}”?",
            "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;
        Games.Remove(SelectedGame);
        SelectedGame = null;
        OnCountsChanged();
        _ = SaveAsync();
    }

    private void ApplySort()
    {
        using (FilteredGames.DeferRefresh())
        {
            FilteredGames.SortDescriptions.Clear();
            switch (SelectedSort)
            {
                case "Tytuł Z–A":
                    FilteredGames.SortDescriptions.Add(new SortDescription(nameof(Game.Title), ListSortDirection.Descending));
                    break;
                case "Rok: najnowsze":
                    FilteredGames.SortDescriptions.Add(new SortDescription(nameof(Game.ReleaseYear), ListSortDirection.Descending));
                    break;
                case "Ocena: najwyższa":
                    FilteredGames.SortDescriptions.Add(new SortDescription(nameof(Game.Rating), ListSortDirection.Descending));
                    break;
                default:
                    FilteredGames.SortDescriptions.Add(new SortDescription(nameof(Game.Title), ListSortDirection.Ascending));
                    break;
            }
        }
    }

    private void OnCountsChanged()
    {
        OnPropertyChanged(nameof(TotalGames));
        OnPropertyChanged(nameof(CompletedGames));
        OnPropertyChanged(nameof(ToPlayGames));
    }
}
