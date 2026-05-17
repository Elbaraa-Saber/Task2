using System;
using System.Threading;

namespace LifeSim;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        const int width = 50;
        const int height = 22;
        var initialPlants = (int)(width * height * 0.22);
        const int initialHerbivores = 28;
        const int initialPredators = 10;

        var world = new World(width, height);
        world.Seed<Plant>(initialPlants);
        world.Seed<Herbivore>(initialHerbivores);
        world.Seed<Predator>(initialPredators);

        var renderer = new ConsoleWorldRenderer();

        var paused = false;
        const int delayMs = 120;

        while (true)
        {
            while (!Console.IsInputRedirected && Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
                {
                    Console.ResetColor();
                    Console.CursorVisible = true;
                    return;
                }

                if (key == ConsoleKey.Spacebar || key == ConsoleKey.P)
                {
                    paused = !paused;
                }
            }

            if (!paused)
            {
                world.Step();
                renderer.Render(world);
            }

            Thread.Sleep(delayMs);
        }
    }
}