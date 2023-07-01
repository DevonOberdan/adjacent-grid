
using System.Collections.Generic;
using System.Linq;

public static class ExtensionMethods
{
    public static bool Between(this float index, float min, float max) => index >= min && index <= max;

    public static bool Between(this int index, int min, int max) => index >= min && index <= max;
    public static bool IsValidIndex<T>(this IEnumerable<T> list, int index) => index.Between(0, list.Count()-1);
}
