using System.Text.RegularExpressions;

namespace SpotifyPlaylistToText.Utils;

public class UrlValidator
{
    // validar url ingresada, bool. Extraer Id.

    private static readonly Regex SpotifyPlaylistRegex = new Regex(
        @"^https://open\.spotify\.com/playlist/([a-zA-Z0-9]{22})(?:\?.*)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    
    public static bool IsValidUrl(string url)
    {
        return SpotifyPlaylistRegex.IsMatch(url);
    }

    public static string IdExtract(string url)
    {
        var match = SpotifyPlaylistRegex.Match(url);
        if (match.Success && match.Groups.Count > 1)
        {
            return match.Groups[1].Value;
        }
        return string.Empty;
    }
}