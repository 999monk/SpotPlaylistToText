using SpotifyPlaylistToText.Models;
using SpotifyPlaylistToText.Services;
using SpotifyPlaylistToText.Utils;

namespace SpotifyPlaylistToText
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== SPOTIFY PLAYLIST .TXT EXTRACTOR by monk999 ===");
            Console.WriteLine();
            Console.WriteLine("Ingresa link de playlist de Spotify: ");
            string? url = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(url) || !UrlValidator.IsValidUrl(url))
            {
                Console.WriteLine("Url inválida");
                return;
            }

            string playlistId = UrlValidator.IdExtract(url);

            using var spotService = new SpotService();
            var tracks = spotService.GetTracksPlaylist(playlistId);

            if (tracks.Count > 0)
            {
                FileService.TracksToFile(tracks);
                Console.WriteLine("Playlist exportada a playlist.txt.");
                Console.WriteLine($"Busca el archivo en: {Directory.GetCurrentDirectory()}");
            }
            else
            {
                Console.WriteLine("No se pudieron obtener las canciones de la playlist.");
            }
        }
    }
}