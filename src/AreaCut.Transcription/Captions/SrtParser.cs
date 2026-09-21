using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AreaCut.Core.Models;
using AreaCut.Core.Time;

namespace AreaCut.Transcription.Captions;

/// <summary>
/// Parses SRT subtitle files into CaptionItem objects.
/// </summary>
public static class SrtParser
{
    /// <summary>Parse an SRT file into caption items.</summary>
    public static List<CaptionItem> Parse(string srtContent)
    {
        var captions = new List<CaptionItem>();
        var blocks = srtContent.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        foreach (var block in blocks)
        {
            var lines = block.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 3) continue;

            // Line 0: sequence number (ignored)
            // Line 1: timestamp range "00:00:01,000 --> 00:00:04,000"
            // Line 2+: subtitle text
            var timeLine = lines[1].Trim();
            if (!TryParseSrtTimeRange(timeLine, out var start, out var end)) continue;

            var text = string.Join("\n", lines.Skip(2));
            captions.Add(new CaptionItem(start, end, text));
        }

        return captions;
    }

    /// <summary>Parse an SRT file from disk.</summary>
    public static List<CaptionItem> ParseFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        return Parse(content);
    }

    /// <summary>Export caption items to SRT format string.</summary>
    public static string ToSrt(IEnumerable<CaptionItem> captions)
    {
        var sb = new System.Text.StringBuilder();
        var index = 1;

        foreach (var caption in captions.OrderBy(c => c.Start.Ticks))
        {
            sb.AppendLine(index.ToString());
            sb.AppendLine($"{FormatSrtTime(caption.Start)} --> {FormatSrtTime(caption.End)}");
            sb.AppendLine(caption.Text);
            sb.AppendLine();
            index++;
        }

        return sb.ToString();
    }

    private static bool TryParseSrtTimeRange(string line, out TimeStamp start, out TimeStamp end)
    {
        start = TimeStamp.Zero;
        end = TimeStamp.Zero;

        var parts = line.Split("-->", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2) return false;

        if (!TryParseSrtTime(parts[0], out start)) return false;
        if (!TryParseSrtTime(parts[1], out end)) return false;
        return true;
    }

    private static bool TryParseSrtTime(string time, out TimeStamp ts)
    {
        ts = TimeStamp.Zero;
        // Format: HH:MM:SS,mmm or HH:MM:SS.mmm
        time = time.Trim().Replace(',', '.');
        if (!TimeSpan.TryParseExact(time, @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture, out var span))
            return false;
        ts = TimeStamp.FromTimeSpan(span);
        return true;
    }

    private static string FormatSrtTime(TimeStamp ts)
    {
        var span = ts.ToTimeSpan();
        return $"{(int)span.TotalHours:D2}:{span.Minutes:D2}:{span.Seconds:D2},{span.Milliseconds:D3}";
    }
}
