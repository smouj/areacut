using System;

namespace AreaCut.Core.Time;

/// <summary>
/// Rational frame rate to avoid floating-point precision loss.
/// Common rates: 24, 25, 30, 50, 60 expressed as numerator/denominator.
/// </summary>
public readonly struct FrameRate : IEquatable<FrameRate>
{
    public int Numerator { get; }
    public int Denominator { get; }

    public FrameRate(int numerator, int denominator = 1)
    {
        if (denominator <= 0) throw new ArgumentOutOfRangeException(nameof(denominator));
        if (numerator <= 0) throw new ArgumentOutOfRangeException(nameof(numerator));
        Numerator = numerator;
        Denominator = denominator;
    }

    public double FramesPerSecond => (double)Numerator / Denominator;
    public TimeStamp FrameDuration => new(TimeSpan.TicksPerSecond * Denominator / Numerator);

    /// <summary>Convert a frame number to a timestamp at this frame rate.</summary>
    public TimeStamp FrameToTimeStamp(long frame) => new(frame * TimeSpan.TicksPerSecond * Denominator / Numerator);

    /// <summary>Convert a timestamp to a frame number at this frame rate.</summary>
    public long TimeStampToFrame(TimeStamp ts) => ts.Ticks * Numerator / (TimeSpan.TicksPerSecond * Denominator);

    public static FrameRate Fps24 => new(24);
    public static FrameRate Fps25 => new(25);
    public static FrameRate Fps30 => new(30);
    public static FrameRate Fps50 => new(50);
    public static FrameRate Fps60 => new(60);
    public static FrameRate Fps2997 => new(30000, 1001);
    public static FrameRate Fps23976 => new(24000, 1001);
    public static FrameRate Fps5994 => new(60000, 1001);

    public bool Equals(FrameRate other) => Numerator == other.Numerator && Denominator == other.Denominator;
    public override bool Equals(object? obj) => obj is FrameRate fr && Equals(fr);
    public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);
    public override string ToString() => Denominator == 1 ? $"{Numerator}fps" : $"{Numerator}/{Denominator}fps";
    public static bool operator ==(FrameRate a, FrameRate b) => a.Equals(b);
    public static bool operator !=(FrameRate a, FrameRate b) => !a.Equals(b);
}
