using System;
using System.IO;
using System.Linq;
using AreaCut.Core.Models;

namespace AreaCut.Core.ProjectModel;

/// <summary>
/// Handles project file detection, moved-file relocation, and relink.
/// Original media files are never modified; only references are updated.
/// </summary>
public static class ProjectFile
{
    public const string Extension = ".areacut";

    /// <summary>Check if a path has the .areacut extension.</summary>
    public static bool IsProjectFile(string path)
        => path.EndsWith(Extension, StringComparison.OrdinalIgnoreCase);

    /// <summary>Detect media files that have been moved or renamed.</summary>
    public static void DetectOfflineMedia(AreaCutProject project)
    {
        foreach (var media in project.Media)
        {
            media.IsOffline = !File.Exists(media.FilePath);
        }
    }

    /// <summary>
    /// Try to find a moved media file by searching common locations.
    /// Returns the new path if found, null otherwise.
    /// </summary>
    public static string? TryRelocateMedia(string originalPath, string[] searchDirectories)
    {
        var fileName = Path.GetFileName(originalPath);

        foreach (var dir in searchDirectories)
        {
            try
            {
                var matches = Directory.GetFiles(dir, fileName, SearchOption.AllDirectories);
                if (matches.Length > 0)
                    return matches.First();
            }
            catch
            {
                // Skip inaccessible directories
            }
        }

        return null;
    }

    /// <summary>Relink a media reference to a new file path.</summary>
    public static void RelinkMedia(MediaReference media, string newPath)
    {
        media.FilePath = newPath;
        media.FileName = Path.GetFileName(newPath);
        media.IsOffline = !File.Exists(newPath);
    }
}
