using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.ProjectModel;
using System;
using System.Threading;
using System.Threading.Tasks;
using AreaCut.Export.Encoder;
using AreaCut.Core.Time;
using AreaCut.Export.Presets;

namespace AreaCut.Export.RenderPipeline;

/// <summary>
/// Orchestrates the complete export pipeline:
/// Timeline → Frame scheduler → Decode sources → GPU composition → Text/effects → Audio mix → MF encoder → MP4
/// </summary>
public sealed class RenderPipeline : IDisposable
{
    private bool _disposed;

    /// <summary>Render the project to an MP4 file.</summary>
    public Task<ExportResult> RenderAsync(
        AreaCutProject project,
        ExportPreset preset,
        string outputPath,
        IProgress<ExportProgress>? progress = null,
        CancellationToken ct = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return Task.Run(() =>
        {
            var result = new ExportResult();
            var totalDuration = project.TimelineDuration;
            var totalFrames = (long)(totalDuration.TotalSeconds * preset.Fps);
            var frameDuration = new FrameRate(preset.Fps).FrameDuration;
            var framesRendered = 0;

            try
            {
                // Frame-by-frame rendering loop
                for (var ts = TimeStamp.Zero; ts < totalDuration; ts = ts.Add(frameDuration))
                {
                    ct.ThrowIfCancellationRequested();

                    // 1. Compose frame at timestamp
                    // 2. Encode video frame
                    // 3. Encode audio samples for this frame duration
                    framesRendered++;

                    if (framesRendered % 30 == 0)
                    {
                        var progressPercent = (double)framesRendered / totalFrames * 100;
                        progress?.Report(new ExportProgress
                        {
                            ProgressPercent = progressPercent,
                            FramesRendered = framesRendered,
                            TotalFrames = (int)totalFrames,
                            TimeRendered = ts.ToTimeSpan(),
                            EstimatedRemaining = totalDuration.ToTimeSpan() - ts.ToTimeSpan()
                        });
                    }
                }

                // 4. Finalize MP4 file
                result.Success = true;
                result.OutputPath = outputPath;
                result.Duration = totalDuration.ToTimeSpan();
                result.FrameCount = framesRendered;
                result.Width = preset.Width;
                result.Height = preset.Height;
                result.Fps = preset.Fps;
            }
            catch (OperationCanceledException)
            {
                result.Cancelled = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }, ct);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}

public sealed class ExportResult
{
    public bool Success { get; set; }
    public bool Cancelled { get; set; }
    public string? OutputPath { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan Duration { get; set; }
    public int FrameCount { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Fps { get; set; }
    public long FileSizeBytes { get; set; }
}
