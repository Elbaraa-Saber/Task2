namespace LifeSim;

public class Predator : Animal
{
    public Predator(World world, Point2 pos, Gender? gender = null)
        : base(world, pos, gender)
    {
    }

    protected override int VisionRange => 12;

    protected override int MovementEnergyCost => 3;

    protected override int FoodEnergyGain => 28;

    protected override int ReproductionEnergyThreshold => 80;

    protected override int InitialEnergy => 40;

    protected override char AnimalGlyph => 'W';

    public override System.ConsoleColor? Color => System.ConsoleColor.Red;

    protected override Organism? FindPrey() => World.FindNearest<Herbivore>(Pos, VisionRange);

    protected override Animal MakeChild(Point2 position) => new Predator(World, position);
}