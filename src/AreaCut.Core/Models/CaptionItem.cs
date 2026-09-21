using AreaCut.Core.Time;

namespace AreaCut.Core.Models;

/// <summary>
/// Single caption/subtitle entry with timing and text.
/// </summary>
public sealed class CaptionItem
{
    public string Id { get; } = System.Guid.NewGuid().ToString("N");
    public TimeStamp Start { get; set; }
    public TimeStamp End { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Style { get; set; }

    public TimeStamp Duration => End.Subtract(Start);

    public CaptionItem(TimeStamp start, TimeStamp end, string text)
    {
        if (end < start) throw new System.ArgumentException("End must be >= Start");
        Start = start;
        End = end;
        Text = text ?? throw new System.ArgumentNullException(nameof(text));
    }
}
