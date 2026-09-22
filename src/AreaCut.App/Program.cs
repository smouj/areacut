using System;
using System.Threading;

namespace AreaCut.App;

/// <summary>
/// Entry point for AreaCut.
/// Handles single-instance enforcement and the AreaRec file handover.
/// </summary>
public static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // Only accept a path we could actually open, before deciding anything else.
        var requestedVideo = args.Length > 0 && HandoffChannel.IsSupportedVideo(args[0]) ? args[0] : null;

        // Single instance enforcement
        using var mutex = new Mutex(true, HandoffChannel.MutexName, out var createdNew);
        if (!createdNew)
        {
            // Another AreaCut already owns the window. Hand it the file and leave:
            // this is the AreaRec integration path, and it must not open a second window.
            if (requestedVideo is not null)
            {
                HandoffChannel.TryForward(requestedVideo);
            }

            return;
        }

        // Launch the WinUI application, telling it what to open.
        Microsoft.UI.Xaml.Application.Start(callbackParams =>
        {
            _ = new App { PendingVideoPath = requestedVideo };
        });
    }
}
