using System;

namespace DurationParser;

public static class Duration
{
    /// <summary>
    /// Parses a human-readable duration string and returns total seconds.
    /// Supported formats: "30s", "5m", "2h", "1d", "1h30m", "2d12h", "1h30m45s"
    /// Units: s = seconds, m = minutes, h = hours, d = days
    /// Compound durations must be in descending unit order (d, h, m, s).
    /// </summary>
    /// <param name="input">The duration string to parse.</param>
    /// <returns>Total seconds represented by the duration string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
    /// <exception cref="FormatException">Thrown when input is invalid.</exception>
    public static int ParseDuration(string input)
    {
        // TODO: The AI will implement this after the tests are written.
        throw new NotImplementedException();
    }
}
