using System;
using System.Runtime.InteropServices;

namespace AreaCut.Media.MediaFoundation;

/// <summary>
/// Manages Media Foundation startup/shutdown lifecycle.
/// Must be initialized before any MF operations and shut down on app exit.
/// </summary>
public sealed class MediaFoundationRuntime : IDisposable
{
    private bool _initialized;
    private bool _disposed;

    public void Initialize()
    {
        if (_initialized) return;
        var hr = MFStartup(MF_VERSION, MFSTARTUP_NOSOCKET);
        if (hr != 0)
            throw new COMException($"MFStartup failed with HRESULT 0x{hr:X8}", hr);
        _initialized = true;
    }

    public void Shutdown()
    {
        if (!_initialized) return;
        MFShutdown();
        _initialized = false;
    }

    public bool IsInitialized => _initialized;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Shutdown();
    }

    // Media Foundation constants
    private const uint MF_VERSION = 0x0002;
    private const uint MFSTARTUP_NOSOCKET = 0x1;

    [DllImport("mfplat.dll")]
    private static extern int MFStartup(uint version, uint flags);

    [DllImport("mfplat.dll")]
    private static extern int MFShutdown();
}
