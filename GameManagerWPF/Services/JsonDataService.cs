using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameManagerWPF.Models;

namespace GameManagerWPF.Services;

public sealed class JsonDataService
{
    private readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GameManagerWPF", "games.json");

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<List<Game>> LoadAsync()
    {
        if (!File.Exists(_filePath)) return new List<Game>();
        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<Game>>(stream, _options) ?? new List<Game>();
    }

    public async Task SaveAsync(IEnumerable<Game> games)
    {
        var directory = Path.GetDirectoryName(_filePath)!;
        Directory.CreateDirectory(directory);
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, games, _options);
    }

    public string StoragePath => _filePath;
}
