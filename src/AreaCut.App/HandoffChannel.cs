using System;
using System.IO;
using System.IO.Pipes;

namespace AreaCut.App;

/// <summary>
/// The single-instance and file-handover contract between AreaCut processes.
/// A second launch does not open a second editor: it forwards the file it was
/// given to the instance that already owns the window, then exits.
/// </summary>
internal static class HandoffChannel
{
    public const string MutexName = "AreaCut_SingleInstance";

    /// <summary>Named pipe the first instance listens on for incoming files.</summary>
    public const string PipeName = "AreaCut.OpenVideo";

    private static readonly string[] SupportedExtensions = { ".mp4", ".mov", ".m4v", ".avi", ".wmv" };

    /// <summary>
    /// True only when the path exists and carries a video extension AreaCut accepts.
    /// </summary>
    public static bool IsSupportedVideo(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return false;
        }

        var extension = Path.GetExtension(path).ToLowerInvariant();
        return Array.IndexOf(SupportedExtensions, extension) >= 0;
    }

    /// <summary>
    /// Hands a path to the running instance. Returns false when nothing is listening,
    /// which means the caller is on its own.
    /// </summary>
    public static bool TryForward(string path)
    {
        try
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
            client.Connect(1500);

            using var writer = new StreamWriter(client) { AutoFlush = true };
            writer.WriteLine(path);
            return true;
        }
        catch (Exception)
        {
            // No listener, or the pipe closed mid-write. Either way the caller exits.
            return false;
        }
    }
}
