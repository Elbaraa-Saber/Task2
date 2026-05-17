using System;

namespace LifeSim;

public abstract class Organism
{
    protected Organism(World world, Point2 pos, Gender? gender = null)
    {
        World = world;
        Pos = world.Wrap(pos);
        Gender = gender ?? PickGender();
    }

    public World World { get; }

    public Point2 Pos { get; internal set; }

    public bool IsAlive { get; private set; } = true;

    public int Age { get; private set; }

    public abstract char Glyph { get; }

    public virtual ConsoleColor? Color => null;

    public Gender Gender { get; }

    public virtual void Tick() => Age++;

    internal void MarkDead()
    {
        IsAlive = false;
    }

    private static Gender PickGender() => Rand.Chance(0.5) ? Gender.Female : Gender.Male;
}