using System;
using System.Collections.Generic;
using System.Linq;

public static class PlayerNames
{
    private static readonly string[] LineSeparators = { "\r\n", "\r", "\n" };

    // One name per line, kept as typed apart from surrounding spaces
    public static List<string> Parse(string text)
    {
        return text.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries)
            .Select(name => name.Trim())
            .Where(name => name.Length > 0)
            .ToList();
    }

    // "Ana" and "ana" count as the same player
    public static List<string> FindDuplicates(List<string> names)
    {
        return names.GroupBy(name => name, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
    }
}
