using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AreaCut.Core.Models;
using AreaCut.Core.Project;
using ProjectModel = AreaCut.Core.Project.Project;
using AreaCut.Core.Time;

namespace AreaCut.Core.Serialization;

/// <summary>
/// Serializes and deserializes .areacut project files (versioned JSON).
/// Atomic writes via temp file + rename to prevent corruption.
/// </summary>
public sealed class ProjectSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new TimeStampJsonConverter(), new TimeRangeJsonConverter() }
    };

    /// <summary>Current project format version.</summary>
    public const int CurrentFormatVersion = 1;

    /// <summary>Save a project to a .areacut file atomically.</summary>
    public async Task SaveAsync(ProjectModel project, string filePath, CancellationToken ct = default)
    {
        var document = ProjectDocument.FromProject(project);
        var json = JsonSerializer.Serialize(document, JsonOptions);

        // Atomic write: temp file + rename
        var tempFile = filePath + ".tmp";
        await File.WriteAllTextAsync(tempFile, json, ct).ConfigureAwait(false);
        if (File.Exists(filePath))
            File.Delete(filePath);
        File.Move(tempFile, filePath);

        project.ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>Load a project from a .areacut file.</summary>
    public async Task<ProjectModel> LoadAsync(string filePath, CancellationToken ct = default)
    {
        var json = await File.ReadAllTextAsync(filePath, ct).ConfigureAwait(false);
        var document = JsonSerializer.Deserialize<ProjectDocument>(json, JsonOptions)
            ?? throw new InvalidDataException("Failed to deserialize project file");

        // Version migration
        if (document.Version < CurrentFormatVersion)
            document = ProjectMigrator.Migrate(document);

        return document.ToProject();
    }

    /// <summary>Check if a file is a valid .areacut project.</summary>
    public async Task<bool> IsValidProjectFile(string filePath, CancellationToken ct = default)
    {
        try
        {
            var json = await File.ReadAllTextAsync(filePath, ct).ConfigureAwait(false);
            var doc = JsonSerializer.Deserialize<ProjectDocument>(json, JsonOptions);
            return doc?.Version <= CurrentFormatVersion;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// JSON document format for .areacut files.
/// </summary>
public sealed class ProjectDocument
{
    public int Version { get; set; } = ProjectSerializer.CurrentFormatVersion;
    public ProjectDocumentCanvas Canvas { get; set; } = new();
    public List<TrackDocument> Tracks { get; set; } = new();
    public List<ClipDocument> Clips { get; set; } = new();
    public List<TextClipDocument> TextClips { get; set; } = new();
    public List<MediaReferenceDocument> Media { get; set; } = new();
    public List<CaptionItemDocument> Captions { get; set; } = new();
    public List<TransitionDocument> Transitions { get; set; } = new();
    public ProjectSettingsDocument Settings { get; set; } = new();
    public string Name { get; set; } = "Untitled";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public static ProjectDocument FromProject(ProjectModel project) => new()
    {
        Name = project.Name,
        Canvas = ProjectDocumentCanvas.FromCanvas(project.Canvas),
        Tracks = project.Tracks.ConvertAll(TrackDocument.FromTrack),
        Clips = project.Clips.ConvertAll(ClipDocument.FromClip),
        TextClips = project.TextClips.ConvertAll(TextClipDocument.FromTextClip),
        Media = project.Media.ConvertAll(MediaReferenceDocument.FromMediaReference),
        Captions = project.Captions.ConvertAll(CaptionItemDocument.FromCaptionItem),
        Transitions = project.Transitions.ConvertAll(TransitionDocument.FromTransition),
        Settings = ProjectSettingsDocument.FromSettings(project.Settings),
        CreatedAt = project.CreatedAt,
        ModifiedAt = project.ModifiedAt,
    };

    public ProjectModel ToProject()
    {
        var project = new ProjectModel
        {
            Name = Name,
            Canvas = Canvas.ToCanvas(),
            Settings = Settings.ToSettings(),
            CreatedAt = CreatedAt,
            ModifiedAt = ModifiedAt,
        };
        foreach (var t in Tracks) project.Tracks.Add(t.ToTrack());
        foreach (var m in Media) project.Media.Add(m.ToMediaReference());
        foreach (var c in Clips) project.Clips.Add(c.ToClip());
        foreach (var t in TextClips) project.TextClips.Add(t.ToTextClip());
        foreach (var c in Captions) project.Captions.Add(c.ToCaptionItem());
        foreach (var t in Transitions) project.Transitions.Add(t.ToTransition());
        return project;
    }
}

// Document DTOs for JSON serialization

public sealed class ProjectDocumentCanvas
{
    public int Width { get; set; } = 1080;
    public int Height { get; set; } = 1920;
    public int Fps { get; set; } = 30;

    public static ProjectDocumentCanvas FromCanvas(CanvasSpec canvas) => new()
    {
        Width = canvas.Width, Height = canvas.Height, Fps = canvas.Fps
    };

    public CanvasSpec ToCanvas() => new() { Width = Width, Height = Height, Fps = Fps };
}

public sealed class TrackDocument
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Kind { get; set; } = "Video";
    public bool IsMuted { get; set; }
    public bool IsLocked { get; set; }
    public bool IsVisible { get; set; } = true;
    public int ZOrder { get; set; }

    public static TrackDocument FromTrack(Track t) => new()
    {
        Id = t.Id, Name = t.Name, Kind = t.Kind.ToString(),
        IsMuted = t.IsMuted, IsLocked = t.IsLocked, IsVisible = t.IsVisible, ZOrder = t.ZOrder
    };

    public Track ToTrack() => new(Enum.Parse<TrackKind>(Kind), Name)
    {
        IsMuted = IsMuted, IsLocked = IsLocked, IsVisible = IsVisible, ZOrder = ZOrder
    };
}

public sealed class ClipDocument
{
    public string Id { get; set; } = "";
    public string SourceMediaId { get; set; } = "";
    public long SourceInTicks { get; set; }
    public long SourceOutTicks { get; set; }
    public long TimelineStartTicks { get; set; }
    public string TrackId { get; set; } = "";
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double ScaleX { get; set; } = 1.0;
    public double ScaleY { get; set; } = 1.0;
    public double RotationDegrees { get; set; }
    public double Opacity { get; set; } = 1.0;
    public double PlaybackRate { get; set; } = 1.0;
    public double Volume { get; set; } = 1.0;
    public double CropLeft { get; set; }
    public double CropTop { get; set; }
    public double CropRight { get; set; } = 1.0;
    public double CropBottom { get; set; } = 1.0;

    public static ClipDocument FromClip(Clip c) => new()
    {
        Id = c.Id, SourceMediaId = c.SourceMediaId, TrackId = c.TrackId,
        SourceInTicks = c.SourceIn.Ticks, SourceOutTicks = c.SourceOut.Ticks,
        TimelineStartTicks = c.TimelineStart.Ticks,
        OffsetX = c.Transform.OffsetX, OffsetY = c.Transform.OffsetY,
        ScaleX = c.Transform.ScaleX, ScaleY = c.Transform.ScaleY,
        RotationDegrees = c.Transform.RotationDegrees, Opacity = c.Transform.Opacity,
        PlaybackRate = c.PlaybackRate, Volume = c.Volume,
        CropLeft = c.Crop.Left, CropTop = c.Crop.Top, CropRight = c.Crop.Right, CropBottom = c.Crop.Bottom,
    };

    public Clip ToClip() => new(SourceMediaId, TrackId)
    {
        SourceIn = new TimeStamp(SourceInTicks), SourceOut = new TimeStamp(SourceOutTicks),
        TimelineStart = new TimeStamp(TimelineStartTicks),
        Transform = new Transform
        {
            OffsetX = OffsetX, OffsetY = OffsetY, ScaleX = ScaleX, ScaleY = ScaleY,
            RotationDegrees = RotationDegrees, Opacity = Opacity,
        },
        Crop = new Crop { Left = CropLeft, Top = CropTop, Right = CropRight, Bottom = CropBottom },
        PlaybackRate = PlaybackRate, Volume = Volume,
    };
}

public sealed class TextClipDocument
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";
    public string TrackId { get; set; } = "";
    public long TimelineStartTicks { get; set; }
    public long TimelineEndTicks { get; set; }
    public string FontFamily { get; set; } = "Segoe UI";
    public double FontSize { get; set; } = 48;
    public string FontWeight { get; set; } = "Bold";
    public string Alignment { get; set; } = "Center";
    public string Foreground { get; set; } = "#FFFFFF";
    public string Background { get; set; } = "transparent";
    public double Opacity { get; set; } = 1.0;
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double Scale { get; set; } = 1.0;
    public double RotationDegrees { get; set; }
    public bool HasShadow { get; set; }
    public bool HasOutline { get; set; }
    public string? OutlineColor { get; set; }
    public double OutlineThickness { get; set; }
    public long? FadeInTicks { get; set; }
    public long? FadeOutTicks { get; set; }

    public static TextClipDocument FromTextClip(TextClip t) => new()
    {
        Id = t.Id, Content = t.Content, TrackId = t.TrackId,
        TimelineStartTicks = t.TimelineStart.Ticks, TimelineEndTicks = t.TimelineEnd.Ticks,
        FontFamily = t.FontFamily, FontSize = t.FontSize, FontWeight = t.FontWeight,
        Alignment = t.Alignment, Foreground = t.Foreground, Background = t.Background,
        Opacity = t.Opacity, OffsetX = t.OffsetX, OffsetY = t.OffsetY,
        Scale = t.Scale, RotationDegrees = t.RotationDegrees,
        HasShadow = t.HasShadow, HasOutline = t.HasOutline,
        OutlineColor = t.OutlineColor, OutlineThickness = t.OutlineThickness,
        FadeInTicks = t.FadeInDuration?.Ticks, FadeOutTicks = t.FadeOutDuration?.Ticks,
    };

    public TextClip ToTextClip() => new(TrackId)
    {
        Content = Content,
        TimelineStart = new TimeStamp(TimelineStartTicks), TimelineEnd = new TimeStamp(TimelineEndTicks),
        FontFamily = FontFamily, FontSize = FontSize, FontWeight = FontWeight,
        Alignment = Alignment, Foreground = Foreground, Background = Background,
        Opacity = Opacity, OffsetX = OffsetX, OffsetY = OffsetY,
        Scale = Scale, RotationDegrees = RotationDegrees,
        HasShadow = HasShadow, HasOutline = HasOutline,
        OutlineColor = OutlineColor, OutlineThickness = OutlineThickness,
        FadeInDuration = FadeInTicks.HasValue ? new TimeStamp(FadeInTicks.Value) : null,
        FadeOutDuration = FadeOutTicks.HasValue ? new TimeStamp(FadeOutTicks.Value) : null,
    };
}

public sealed class MediaReferenceDocument
{
    public string Id { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string? FileName { get; set; }
    public string Kind { get; set; } = "Video";
    public double? DurationSeconds { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public double? Fps { get; set; }
    public string? Codec { get; set; }
    public string? AudioCodec { get; set; }
    public int? AudioChannels { get; set; }
    public int? AudioSampleRate { get; set; }
    public long? Bitrate { get; set; }

    public static MediaReferenceDocument FromMediaReference(MediaReference m) => new()
    {
        Id = m.Id, FilePath = m.FilePath, FileName = m.FileName, Kind = m.Kind.ToString(),
        DurationSeconds = m.Duration?.TotalSeconds, Width = m.Width, Height = m.Height,
        Fps = m.Fps, Codec = m.Codec, AudioCodec = m.AudioCodec,
        AudioChannels = m.AudioChannels, AudioSampleRate = m.AudioSampleRate, Bitrate = m.Bitrate,
    };

    public MediaReference ToMediaReference()
    {
        var kind = Enum.Parse<MediaKind>(Kind);
        var m = new MediaReference(FilePath, kind)
        {
            Id = Guid.TryParse(Id, out var guid) ? Id : Guid.NewGuid().ToString("N"),
            FileName = FileName, Width = Width, Height = Height,
            Fps = Fps, Codec = Codec, AudioCodec = AudioCodec,
            AudioChannels = AudioChannels, AudioSampleRate = AudioSampleRate, Bitrate = Bitrate,
        };
        if (DurationSeconds.HasValue) m.Duration = TimeSpan.FromSeconds(DurationSeconds.Value);
        return m;
    }
}

public sealed class CaptionItemDocument
{
    public string Id { get; set; } = "";
    public long StartTicks { get; set; }
    public long EndTicks { get; set; }
    public string Text { get; set; } = "";
    public string? Style { get; set; }

    public static CaptionItemDocument FromCaptionItem(CaptionItem c) => new()
    {
        Id = c.Id, StartTicks = c.Start.Ticks, EndTicks = c.End.Ticks, Text = c.Text, Style = c.Style,
    };

    public CaptionItem ToCaptionItem() => new(new TimeStamp(StartTicks), new TimeStamp(EndTicks), Text) { Style = Style };
}

public sealed class TransitionDocument
{
    public string Id { get; set; } = "";
    public string Kind { get; set; } = "Cut";
    public double DurationSeconds { get; set; }
    public string FromClipId { get; set; } = "";
    public string ToClipId { get; set; } = "";

    public static TransitionDocument FromTransition(Transition t) => new()
    {
        Id = t.Id, Kind = t.Kind.ToString(), DurationSeconds = t.DurationSeconds,
        FromClipId = t.FromClipId, ToClipId = t.ToClipId,
    };

    public Transition ToTransition() => new()
    {
        Id = Id, Kind = Enum.Parse<TransitionKind>(Kind), DurationSeconds = DurationSeconds,
        FromClipId = FromClipId, ToClipId = ToClipId,
    };
}

public sealed class ProjectSettingsDocument
{
    public string ExportPath { get; set; } = "";
    public string ExportFileName { get; set; } = "export";
    public bool UseHardwareEncoding { get; set; } = true;
    public int ExportQuality { get; set; } = 80;
    public bool AutoSaveEnabled { get; set; } = true;
    public int AutoSaveIntervalSeconds { get; set; } = 120;
    public long MaxCacheSizeMb { get; set; } = 2048;
    public string CachePath { get; set; } = ".cache";

    public static ProjectSettingsDocument FromSettings(ProjectSettings s) => new()
    {
        ExportPath = s.ExportPath, ExportFileName = s.ExportFileName,
        UseHardwareEncoding = s.UseHardwareEncoding, ExportQuality = s.ExportQuality,
        AutoSaveEnabled = s.AutoSaveEnabled, AutoSaveIntervalSeconds = s.AutoSaveIntervalSeconds,
        MaxCacheSizeMb = s.MaxCacheSizeMb, CachePath = s.CachePath,
    };

    public ProjectSettings ToSettings() => new()
    {
        ExportPath = ExportPath, ExportFileName = ExportFileName,
        UseHardwareEncoding = UseHardwareEncoding, ExportQuality = ExportQuality,
        AutoSaveEnabled = AutoSaveEnabled, AutoSaveIntervalSeconds = AutoSaveIntervalSeconds,
        MaxCacheSizeMb = MaxCacheSizeMb, CachePath = CachePath,
    };
}

// JSON converters for TimeStamp and TimeRange

internal sealed class TimeStampJsonConverter : JsonConverter<TimeStamp>
{
    public override TimeStamp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetInt64());

    public override void Write(Utf8JsonWriter writer, TimeStamp value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value.Ticks);
}

internal sealed class TimeRangeJsonConverter : JsonConverter<TimeRange>
{
    public override TimeRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        return new TimeRange(new TimeStamp(root.GetProperty("start").GetInt64()), new TimeStamp(root.GetProperty("end").GetInt64()));
    }

    public override void Write(Utf8JsonWriter writer, TimeRange value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("start", value.Start.Ticks);
        writer.WriteNumber("end", value.End.Ticks);
        writer.WriteEndObject();
    }
}
