using System;

namespace NauraaBot.Core.Providers;

public static class RandomProvider
{
    public static Random Random { get; } = new Random();
}