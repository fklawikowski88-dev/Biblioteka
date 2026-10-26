using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameManagerWPF.Models;

public class Game : INotifyPropertyChanged
{
    private string _title = "";
    private string _genre = "Inne";
    private string _platform = "PC";
    private int _releaseYear = DateTime.Now.Year;
    private double _rating = 5;
    private string _status = "Do ogrania";
    private string _notes = "";

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get => _title; set => SetField(ref _title, value); }
    public string Genre { get => _genre; set => SetField(ref _genre, value); }
    public string Platform { get => _platform; set => SetField(ref _platform, value); }
    public int ReleaseYear { get => _releaseYear; set => SetField(ref _releaseYear, value); }
    public double Rating { get => _rating; set => SetField(ref _rating, value); }
    public string Status { get => _status; set => SetField(ref _status, value); }
    public string Notes { get => _notes; set => SetField(ref _notes, value); }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Game Copy() => new()
    {
        Id = Id,
        Title = Title,
        Genre = Genre,
        Platform = Platform,
        ReleaseYear = ReleaseYear,
        Rating = Rating,
        Status = Status,
        Notes = Notes
    };
}
