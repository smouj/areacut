using System;
using System.Threading;

namespace AreaCut.App;

/// <summary>
/// Entry point for AreaCut.
/// Handles single-instance enforcement and command-line video import.
/// </summary>
public static class Program
{
    private const string MutexName = "AreaCut_SingleInstance";

    [STAThread]
    static void Main(string[] args)
    {
        // Single instance enforcement
        using var mutex = new Mutex(true, MutexName, out var createdNew);
        if (!createdNew)
        {
            // Another instance is running. In production, send the file path
            // to the existing instance via named pipe or similar IPC.
            // For now, just exit.
            return;
        }

        // If a video file was passed via command line, open it directly
        string? videoPath = null;
        if (args.Length > 0 && System.IO.File.Exists(args[0]))
        {
            var ext = System.IO.Path.GetExtension(args[0]).ToLowerInvariant();
            if (ext is ".mp4" or ".mov" or ".m4v" or ".avi" or ".wmv")
                videoPath = args[0];
        }

        // Launch the WinUI application
        Microsoft.UI.Xaml.Application.Start(p =>
        {
            var app = new App();
            if (videoPath != null)
            {
                // Open the video directly (AreaRec integration path)
                app.OpenVideoDirectly(videoPath);
            }
        });
    }
}
