using System;
using System.Threading;

namespace LifeSim;

public sealed class SimulationRunner
{
    private const int Width = 50;
    private const int Height = 22;
    private const int InitialHerbivores = 28;
    private const int InitialPredators = 10;
    private const int DelayMilliseconds = 120;

    private readonly ConsoleWorldRenderer _renderer = new();

    private bool _paused;

    public void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        var world = CreateWorld();

        while (true)
        {
            if (ShouldQuit())
            {
                ResetConsole();
                return;
            }

            if (!_paused)
            {
                world.Step();
                _renderer.Render(world);
            }

            Thread.Sleep(DelayMilliseconds);
        }
    }

    private static World CreateWorld()
    {
        var initialPlants = (int)(Width * Height * 0.22);

        var world = new World(Width, Height);
        world.Seed(initialPlants, position => new Plant(world, position));
        world.Seed(InitialHerbivores, position => new Herbivore(world, position));
        world.Seed(InitialPredators, position => new Predator(world, position));

        return world;
    }

    private bool ShouldQuit()
    {
        while (!Console.IsInputRedirected && Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
            {
                return true;
            }

            if (key == ConsoleKey.Spacebar || key == ConsoleKey.P)
            {
                _paused = !_paused;
            }
        }

        return false;
    }

    private static void ResetConsole()
    {
        Console.ResetColor();
        Console.CursorVisible = true;
    }
}