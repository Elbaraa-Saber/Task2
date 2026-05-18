using System.Linq;

namespace LifeSim;

public class Plant : Organism
{
    private const int MatureAge = 6;
    private const double SpreadChance = 0.18;
    private const int MaxAge = 250;
    private const double OldAgeDeathChance = 0.01;

    public Plant(World world, Point2 pos, Gender? gender = null)
        : base(world, pos, gender)
    {
    }

    public override char Glyph => '♣';

    public override System.ConsoleColor? Color => System.ConsoleColor.Green;

    public override void Tick()
    {
        base.Tick();

        SpreadIfMature();
        DieIfTooOld();
    }

    private void SpreadIfMature()
    {
        if (!CanSpread())
        {
            return;
        }

        var emptyNeighborPositions = World.EmptyNeighbors8(Pos).ToList();
        if (emptyNeighborPositions.Count == 0)
        {
            return;
        }

        World.Add(new Plant(World, emptyNeighborPositions.Pick()!));
    }

    private bool CanSpread()
    {
        return Age >= MatureAge && RandomProvider.Chance(SpreadChance);
    }

    private void DieIfTooOld()
    {
        if (Age > MaxAge && RandomProvider.Chance(OldAgeDeathChance))
        {
            World.Remove(this);
        }
    }
}