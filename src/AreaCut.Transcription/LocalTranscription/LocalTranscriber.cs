using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AreaCut.Core.Models;
using AreaCut.Core.Time;

namespace AreaCut.Transcription.LocalTranscription;

/// <summary>
/// Local transcription engine using Whisper-compatible runtime.
/// Completely local — no API calls, no audio leaves the PC.
/// If the model is not available, transcription is skipped gracefully;
/// the app still works without it.
/// </summary>
public sealed class LocalTranscriber : IDisposable
{
    private bool _disposed;
    private bool _modelAvailable;

    public bool IsModelAvailable => _modelAvailable;
    public string? ModelPath { get; private set; }

    /// <summary>Check if a local transcription model is available.</summary>
    public Task<bool> CheckModelAvailabilityAsync(CancellationToken ct = default)
    {
        // In production: check for whisper.cpp or ONNX model files
        // in known locations (app data, user-specified path)
        _modelAvailable = false;
        ModelPath = null;
        return Task.FromResult(_modelAvailable);
    }

    /// <summary>
    /// Transcribe an audio/video file locally.
    /// Returns caption items with timestamps.
    /// </summary>
    public Task<List<CaptionItem>> TranscribeAsync(
        string filePath,
        string? language = null,
        IProgress<TranscriptionProgress>? progress = null,
        CancellationToken ct = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_modelAvailable)
            throw new InvalidOperationException("No transcription model available. Transcription is optional.");

        // In production:
        // 1. Extract audio from the media file
        // 2. Convert to 16kHz mono WAV
        // 3. Run through whisper.cpp or ONNX Runtime
        // 4. Parse SRT/segments output into CaptionItems
        progress?.Report(new TranscriptionProgress { Percent = 100 });

        return Task.FromResult(new List<CaptionItem>());
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}

public sealed class TranscriptionProgress
{
    public double Percent { get; set; }
    public string? Status { get; set; }
}
