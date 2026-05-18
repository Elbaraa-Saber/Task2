using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeSim;

public sealed class ConsoleWorldRenderer
{
    public void Render(World world)
    {
        Console.SetCursorPosition(0, 0);

        RenderHeader(world);
        RenderGrid(world);
    }

    private static void RenderHeader(World world)
    {
        var plants = CountOrganisms<Plant>(world);
        var herbivores = CountOrganisms<Herbivore>(world);
        var predators = CountOrganisms<Predator>(world);

        Console.ResetColor();
        Console.WriteLine($"Tick: {world.Tick,-8}  Plants: {plants,-5}  Herbivores: {herbivores,-5}  Predators: {predators,-5}   [Space/P] pause, [Q/Esc] quit");
    }

    private static int CountOrganisms<T>(World world)
        where T : Organism
    {
        return world.All.OfType<T>().Count();
    }

    private static void RenderGrid(World world)
    {
        var snapshot = world.GridSnapshot();

        for (var y = 0; y < world.Height; y++)
        {
            RenderRow(world, snapshot, y);
        }
    }

    private static void RenderRow(
        World world,
        IReadOnlyDictionary<Point2, Organism> snapshot,
        int y)
    {
        for (var x = 0; x < world.Width; x++)
        {
            RenderCell(snapshot, x, y);
        }

        Console.WriteLine();
    }

    private static void RenderCell(
        IReadOnlyDictionary<Point2, Organism> snapshot,
        int x,
        int y)
    {
        if (snapshot.TryGetValue(new Point2(x, y), out var organism))
        {
            RenderOrganism(organism);
            return;
        }

        Console.Write(' ');
    }

    private static void RenderOrganism(Organism organism)
    {
        ApplyColor(organism);
        Console.Write(organism.Glyph);
        Console.ResetColor();
    }

    private static void ApplyColor(Organism organism)
    {
        if (organism.Color.HasValue)
        {
            Console.ForegroundColor = organism.Color.Value;
        }
    }
}