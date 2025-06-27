using System.Text;
using System.Text.Json;
using SpotifyPlaylistToText.Models;

namespace SpotifyPlaylistToText.Services;

// modelos api spotify

public class SpotifyPlaylistTracks
{
    public required List<SpotifyPlaylistItem> items { get; set; }
    public string? next { get; set; }
}

public class SpotifyPlaylistItem
{
    public required SpotifyTrack track { get; set; }
}

public class SpotifyTrack
{
    public string? name { get; set; }
    public required List<SpotifyArtist> artists { get; set; }
    public bool is_local { get; set; }
}

public class SpotifyArtist
{
    public string? name { get; set; }
}

public class SpotifyTokenResponse
{
    public string? access_token { get; set; }
    public string? token_type { get; set; }
    public int expires_in { get; set; }
}

public class SpotService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private string? _accessToken;
    private DateTime _tokenExpiry;

    private const string AUTH_URL = "https://accounts.spotify.com/api/token";
    private const string API_BASE_URL = "https://api.spotify.com/v1";
    
    public SpotService()
    {
        // cargar variables .env
        DotNetEnv.Env.Load();
        
        _clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
        _clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
        
        if (string.IsNullOrWhiteSpace(_clientId))
            throw new InvalidOperationException("SPOTIFY_CLIENT_ID no encontrado en .env");
        
        if (string.IsNullOrWhiteSpace(_clientSecret))
            throw new InvalidOperationException("SPOTIFY_CLIENT_SECRET no encontrado en .env");

        _httpClient = new HttpClient();
    }
    
    public List<Track> GetTracksPlaylist(string playlistId)
    {
        try
        {
            return GetTracksPlaylistAsync(playlistId).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo playlist: {ex.Message}");
            return new List<Track>();
        }
    }
    private async Task<List<Track>> GetTracksPlaylistAsync(string playlistId)
    {
        if (string.IsNullOrWhiteSpace(playlistId))
            throw new ArgumentException("Playlist ID es requerido", nameof(playlistId));

        Console.WriteLine("Autenticando con Spotify...");
        await EnsureValidTokenAsync();

        Console.WriteLine($" Obteniendo canciones de playlist: {playlistId}");
        
        var allTracks = new List<Track>();
        string nextUrl = $"{API_BASE_URL}/playlists/{playlistId}/tracks?limit=100&fields=items(track(name,artists(name),is_local)),next";

        int pageCount = 0;
        while (!string.IsNullOrEmpty(nextUrl))
        {
            pageCount++;
            Console.WriteLine($"Procesando página {pageCount}...");
            
            var playlistData = await GetPlaylistTracksAsync(nextUrl);
            if (playlistData?.items != null)
            {
                var tracks = ConvertToTrackModels(playlistData.items); 
                allTracks.AddRange(tracks);
                nextUrl = playlistData.next;
            }
            else
            {
                break;
            }
        }

        Console.WriteLine($"Se obtuvieron {allTracks.Count} canciones");
        return allTracks;
    }
     private async Task EnsureValidTokenAsync()
    {
        if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiry)
        {
            await AuthenticateAsync();
        }
    }

    private async Task AuthenticateAsync()
    {
        string credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")
        );

        var request = new HttpRequestMessage(HttpMethod.Post, AUTH_URL);
        request.Headers.Add("Authorization", $"Basic {credentials}");
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        var response = await _httpClient.SendAsync(request)!;
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Error de autenticación: {response.StatusCode} - {errorContent}");
        }

        var responseContent = (await response.Content.ReadAsStringAsync())!; ;
        var tokenResponse = JsonSerializer.Deserialize<SpotifyTokenResponse>(responseContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        _accessToken = tokenResponse.access_token;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in - 60);
    }

    private async Task<SpotifyPlaylistTracks> GetPlaylistTracksAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Bearer {_accessToken}");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException("Playlist no encontrada. Verifica que el ID sea correcto y que la playlist sea pública.");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException("No autorizado. Verifica tus credenciales de Spotify.");
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Error API: {response.StatusCode} - {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<SpotifyPlaylistTracks>(responseContent,
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
               ?? throw new InvalidOperationException("La respuesta no es válida.");
    }

    private List<Track> ConvertToTrackModels(List<SpotifyPlaylistItem> items)
    {
        var tracks = new List<Track>();

        foreach (var item in items)
        {
            if (item?.track == null || item.track.is_local) 
                continue;

            var track = new Track
            {
                songName = item.track.name ?? "Canción Desconocida",
                artistName = item.track.artists?.FirstOrDefault()?.name ?? "Artista Desconocido"
            };
            
            if (item.track.artists?.Count > 1)
            {
                track.artistName = string.Join(", ", item.track.artists.Select(a => a.name));
            }

            tracks.Add(track);
        }

        return tracks;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
