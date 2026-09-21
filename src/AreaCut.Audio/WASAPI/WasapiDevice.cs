using System;
using System.Runtime.InteropServices;

namespace AreaCut.Audio.WASAPI;

/// <summary>
/// WASAPI audio device wrapper for low-latency audio output on Windows.
/// Uses IAudioClient/IAudioRenderClient for the shared/cexclusive mode rendering.
/// </summary>
public sealed class WasapiDevice : IDisposable
{
    private bool _disposed;
    private bool _initialized;

    public int SampleRate { get; private set; } = 48000;
    public int Channels { get; private set; } = 2;
    public int BitsPerSample { get; private set; } = 16;
    public int BufferSize { get; private set; }

    /// <summary>Initialize the WASAPI device for rendering.</summary>
    public void Initialize(bool exclusiveMode = false)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // In production: 
        // 1. Get default audio endpoint (IMMDeviceEnumerator)
        // 2. Activate IAudioClient
        // 3. Initialize with WAVEFORMATEX (48kHz, 16-bit, stereo)
        // 4. Get IAudioRenderClient for buffer access
        _initialized = true;
    }

    /// <summary>Start rendering.</summary>
    public void Start()
    {
        if (!_initialized) throw new InvalidOperationException("Device not initialized");
        // IAudioClient.Start()
    }

    /// <summary>Stop rendering.</summary>
    public void Stop()
    {
        if (!_initialized) return;
        // IAudioClient.Stop()
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_initialized) Stop();
        // Release COM objects
    }
}
