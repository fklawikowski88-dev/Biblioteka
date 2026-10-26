using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using GameManagerWPF.Models;

namespace GameManagerWPF.Views;

public partial class GameEditWindow : Window
{
    private readonly Game _game;

    public GameEditWindow(Game game)
    {
        InitializeComponent();
        _game = game;
        TitleBox.Text = game.Title;
        NotesBox.Text = game.Notes;
        SelectComboValue(GenreBox, game.Genre);
        SelectComboValue(PlatformBox, game.Platform);
        YearBox.Text = game.ReleaseYear.ToString(CultureInfo.InvariantCulture);
        RatingBox.Text = game.Rating.ToString(CultureInfo.InvariantCulture);
        SelectComboValue(StatusBox, game.Status);
    }

    private static void SelectComboValue(ComboBox combo, string value)
    {
        foreach (var item in combo.Items)
        {
            if (item is ComboBoxItem cbi && (string)cbi.Content == value)
            {
                combo.SelectedItem = cbi;
                return;
            }
        }
        combo.Text = value;
    }

    private static string ComboText(ComboBox combo)
    {
        if (combo.SelectedItem is ComboBoxItem item) return item.Content?.ToString() ?? "";
        return combo.Text.Trim();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var title = TitleBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            ErrorText.Text = "Podaj tytuł gry.";
            TitleBox.Focus();
            return;
        }
        if (!int.TryParse(YearBox.Text.Trim(), out var year) || year < 1970 || year > 2100)
        {
            ErrorText.Text = "Rok musi być liczbą z zakresu 1970–2100.";
            YearBox.Focus();
            return;
        }
        var ratingText = RatingBox.Text.Trim().Replace(',', '.');
        if (!double.TryParse(ratingText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var rating)
            || rating < 0 || rating > 10)
        {
            ErrorText.Text = "Ocena musi być liczbą z zakresu 0–10, np. 8.5.";
            RatingBox.Focus();
            return;
        }

        _game.Title = title;
        _game.Genre = string.IsNullOrWhiteSpace(ComboText(GenreBox)) ? "Inne" : ComboText(GenreBox);
        _game.Platform = string.IsNullOrWhiteSpace(ComboText(PlatformBox)) ? "PC" : ComboText(PlatformBox);
        _game.ReleaseYear = year;
        _game.Rating = rating;
        _game.Status = string.IsNullOrWhiteSpace(ComboText(StatusBox)) ? "Do ogrania" : ComboText(StatusBox);
        _game.Notes = NotesBox.Text.Trim();
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
