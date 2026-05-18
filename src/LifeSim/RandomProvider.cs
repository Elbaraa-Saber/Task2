using System;
using System.Collections.Generic;

namespace LifeSim;

public static class RandomProvider
{
    private static readonly Random SharedRandom = new();

    public static int Next(int min, int max) => SharedRandom.Next(min, max);

    public static double NextDouble() => SharedRandom.NextDouble();

    public static T? Pick<T>(this IList<T> items) =>
        items.Count == 0 ? default : items[SharedRandom.Next(0, items.Count)];

    public static bool Chance(double probability) => NextDouble() < probability;
}