using System;

namespace AreaCut.Core.Time;

/// <summary>
/// A half-open time interval [Start, End) for clip ranges on the timeline
/// and source media sections.
/// </summary>
public readonly struct TimeRange : IEquatable<TimeRange>
{
    public TimeStamp Start { get; }
    public TimeStamp End { get; }
    public TimeStamp Duration => End.Subtract(Start);
    public bool IsEmpty => End <= Start;

    public TimeRange(TimeStamp start, TimeStamp end)
    {
        if (end < start) throw new ArgumentException("End must be >= Start");
        Start = start;
        End = end;
    }

    public static TimeRange FromDuration(TimeStamp start, TimeStamp duration) => new(start, start.Add(duration));

    public bool Contains(TimeStamp ts) => ts >= Start && ts < End;
    public bool Overlaps(TimeRange other) => Start < other.End && other.Start < End;
    public TimeRange Intersect(TimeRange other)
    {
        var s = Start > other.Start ? Start : other.Start;
        var e = End < other.End ? End : other.End;
        return s < e ? new TimeRange(s, e) : default;
    }

    public bool Equals(TimeRange other) => Start.Equals(other.Start) && End.Equals(other.End);
    public override bool Equals(object? obj) => obj is TimeRange tr && Equals(tr);
    public override int GetHashCode() => HashCode.Combine(Start, End);
    public override string ToString() => $"[{Start} - {End})";
}
