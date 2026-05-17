using System;
using System.Linq;

namespace LifeSim;

public sealed class ConsoleWorldRenderer
{
    public void Render(World world)
    {
        Console.SetCursorPosition(0, 0);

        var plants = world.All.OfType<Plant>().Count();
        var herbivores = world.All.OfType<Herbivore>().Count();
        var predators = world.All.OfType<Predator>().Count();

        Console.ResetColor();
        Console.WriteLine($"Tick: {world.Tick,-8}  Plants: {plants,-5}  Herbivores: {herbivores,-5}  Predators: {predators,-5}   [Space/P] pause, [Q/Esc] quit");

        var snapshot = world.GridSnapshot();
        for (var y = 0; y < world.Height; y++)
        {
            for (var x = 0; x < world.Width; x++)
            {
                if (snapshot.TryGetValue(new Point2(x, y), out var organism))
                {
                    organism.ApplyColor();
                    Console.Write(organism.Glyph);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(' ');
                }
            }

            Console.WriteLine();
        }
    }
}