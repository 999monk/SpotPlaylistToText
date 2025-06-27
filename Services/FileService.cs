using SpotifyPlaylistToText.Models;

namespace SpotifyPlaylistToText.Services;

public static class FileService
{
    public static void TracksToFile(List<Track> tracks, string baseFileName = "playlist")
    {
        string dirPath = Directory.GetCurrentDirectory();
        string filePath;
        int fileNumber = 1;
        
        do
        {
            string fileName = fileNumber == 1 ? $"{baseFileName}.txt" : $"{baseFileName}{fileNumber}.txt";
            filePath = Path.Combine(dirPath, fileName);
            fileNumber++;
        } while (File.Exists(filePath));
        
        var lines = tracks.Select(track => $"{track.artistName} - {track.songName}");
        File.WriteAllLines(filePath, lines);

    }
    
}