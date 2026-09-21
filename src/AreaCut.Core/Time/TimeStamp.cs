using System;

namespace AreaCut.Core.Time;

/// <summary>
/// High-precision timestamp based on ticks (100ns units).
/// Avoids floating-point drift in video timeline calculations.
/// </summary>
public readonly struct TimeStamp : IEquatable<TimeStamp>, IComparable<TimeStamp>
{
    /// <summary>Timestamp in ticks (100 nanoseconds).</summary>
    public long Ticks { get; }

    public TimeStamp(long ticks) => Ticks = ticks;

    public static TimeStamp Zero => new(0);

    public static TimeStamp FromSeconds(double seconds) => new((long)(seconds * TimeSpan.TicksPerSecond));
    public static TimeStamp FromMilliseconds(double ms) => new((long)(ms * TimeSpan.TicksPerMillisecond));
    public static TimeStamp FromTimeSpan(TimeSpan ts) => new(ts.Ticks);

    public double TotalSeconds => (double)Ticks / TimeSpan.TicksPerSecond;
    public double TotalMilliseconds => (double)Ticks / TimeSpan.TicksPerMillisecond;
    public TimeSpan ToTimeSpan() => new(Ticks);

    public TimeStamp Add(TimeStamp other) => new(Ticks + other.Ticks);
    public TimeStamp Subtract(TimeStamp other) => new(Ticks - other.Ticks);
    public TimeStamp Multiply(double factor) => new((long)(Ticks * factor));
    public TimeStamp Clamp(TimeStamp min, TimeStamp max) => this < min ? min : this > max ? max : this;

    public static bool operator <(TimeStamp a, TimeStamp b) => a.Ticks < b.Ticks;
    public static bool operator >(TimeStamp a, TimeStamp b) => a.Ticks > b.Ticks;
    public static bool operator <=(TimeStamp a, TimeStamp b) => a.Ticks <= b.Ticks;
    public static bool operator >=(TimeStamp a, TimeStamp b) => a.Ticks >= b.Ticks;
    public static TimeStamp operator +(TimeStamp a, TimeStamp b) => a.Add(b);
    public static TimeStamp operator -(TimeStamp a, TimeStamp b) => a.Subtract(b);
    public static TimeStamp operator *(TimeStamp t, double f) => t.Multiply(f);

    public bool Equals(TimeStamp other) => Ticks == other.Ticks;
    public override bool Equals(object? obj) => obj is TimeStamp ts && Equals(ts);
    public override int GetHashCode() => Ticks.GetHashCode();
    public override string ToString() => ToTimeSpan().ToString(@"hh\:mm\:ss\.fff");
    public int CompareTo(TimeStamp other) => Ticks.CompareTo(other.Ticks);

    public static bool operator ==(TimeStamp a, TimeStamp b) => a.Equals(b);
    public static bool operator !=(TimeStamp a, TimeStamp b) => !a.Equals(b);
}
