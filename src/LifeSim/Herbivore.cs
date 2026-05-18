namespace LifeSim;

public class Herbivore : Animal
{
    public Herbivore(World world, Point2 pos, Gender? gender = null)
        : base(world, pos, gender)
    {
    }

    protected override int VisionRange => 8;

    protected override int MovementEnergyCost => 2;

    protected override int FoodEnergyGain => 18;

    protected override int ReproductionEnergyThreshold => 60;

    protected override int InitialEnergy => 30;

    protected override char AnimalGlyph => 'h';

    public override System.ConsoleColor? Color => System.ConsoleColor.Yellow;

    protected override Organism? FindPrey() => World.FindNearest<Plant>(Pos, VisionRange);

    protected override Animal MakeChild(Point2 position) => new Herbivore(World, position);
}