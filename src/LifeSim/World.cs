using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeSim;

public class World
{
    private readonly Dictionary<Point2, Organism> _grid = new();
    private readonly List<Organism> _organisms = new();

    public World(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int Width { get; }

    public int Height { get; }

    public int Tick { get; private set; }

    public IEnumerable<Organism> All => _organisms.Where(o => o.IsAlive);

    public void Add(Organism org)
    {
        if (_grid.ContainsKey(org.Pos))
        {
            return;
        }

        _organisms.Add(org);
        _grid[org.Pos] = org;
    }

    public void Remove(Organism org)
    {
        if (!org.IsAlive)
        {
            return;
        }

        org.MarkDead();
        _grid.Remove(org.Pos);
    }

    public void MoveTo(Organism org, Point2 newPos)
    {
        if (!org.IsAlive)
        {
            return;
        }

        var wrappedPos = Wrap(newPos);
        if (_grid.ContainsKey(wrappedPos))
        {
            return;
        }

        _grid.Remove(org.Pos);
        org.Pos = wrappedPos;
        _grid[wrappedPos] = org;
    }

    public bool IsEmpty(Point2 p) => !_grid.ContainsKey(Wrap(p));

    public Point2 Wrap(Point2 p)
    {
        var x = ((p.X % Width) + Width) % Width;
        var y = ((p.Y % Height) + Height) % Height;
        return new Point2(x, y);
    }

    public void Step()
    {
        Tick++;
        var snapshot = All.OrderBy(_ => RandomProvider.Next(0, int.MaxValue)).ToList();
        foreach (var o in snapshot)
        {
            if (o.IsAlive)
            {
                o.Tick();
            }
        }

        _organisms.RemoveAll(o => !o.IsAlive);
    }

    public IEnumerable<Point2> Neighbors8(Point2 p)
    {
        for (var dy = -1; dy <= 1; dy++)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                if (dx != 0 || dy != 0)
                {
                    yield return Wrap(new Point2(p.X + dx, p.Y + dy));
                }
            }
        }
    }

    public IEnumerable<Point2> EmptyNeighbors8(Point2 p)
    {
        foreach (var n in Neighbors8(p))
        {
            if (IsEmpty(n))
            {
                yield return n;
            }
        }
    }

    public void Seed<T>(int count)
        where T : Organism
    {
        Seed(count, CreateOrganism<T>);
    }

    public void Seed(int count, Func<Point2, Organism> createOrganism)
    {
        for (var i = 0; i < count; i++)
        {
            var position = RandomEmptyCell();
            if (position == null)
            {
                break;
            }

            Add(createOrganism(position.Value));
        }
    }

    public Point2? RandomEmptyCell()
    {
        return TryFindRandomEmptyCellByAttempts() ?? PickEmptyCellByFullScan();
    }

    private Point2? TryFindRandomEmptyCellByAttempts()
    {
        const int maxAttempts = 500;

        for (var i = 0; i < maxAttempts; i++)
        {
            var position = new Point2(RandomProvider.Next(0, Width), RandomProvider.Next(0, Height));
            if (IsEmpty(position))
            {
                return position;
            }
        }

        return null;
    }

    private Point2? PickEmptyCellByFullScan()
    {
        var emptyCells = FindEmptyCells().ToList();

        return emptyCells.Count == 0 ? null : emptyCells.Pick();
    }

    private IEnumerable<Point2> FindEmptyCells()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var position = new Point2(x, y);
                if (IsEmpty(position))
                {
                    yield return position;
                }
            }
        }
    }

    public Organism? FindNearest<T>(Point2 from, int visionRange)
        where T : Organism
    {
        Organism? best = null;
        var bestDist = int.MaxValue;

        foreach (var o in All)
        {
            if (o is T)
            {
                var dx = ToroidalDistance(from.X, o.Pos.X, Width);
                var dy = ToroidalDistance(from.Y, o.Pos.Y, Height);
                var distance = dx + dy;
                if (distance <= visionRange && distance < bestDist)
                {
                    best = o;
                    bestDist = distance;
                }
            }
        }

        return best;
    }

    public string SerializeWorldSnapshot()
    {
        var items = All.Select(o => $"{o.GetType().Name}@{o.Pos.X},{o.Pos.Y}");
        return $"Tick={Tick} | {string.Join(";", items)}";
    }

    public IReadOnlyDictionary<Point2, Organism> GridSnapshot() => new Dictionary<Point2, Organism>(_grid);

    private Organism CreateOrganism<T>(Point2 position)
        where T : Organism
    {
        var organismType = typeof(T);

        var constructorWithGender = organismType.GetConstructor(
            new[] { typeof(World), typeof(Point2), typeof(Gender?) });

        if (constructorWithGender != null)
        {
            return (Organism)constructorWithGender.Invoke(new object?[] { this, position, null });
        }

        var constructor = organismType.GetConstructor(
            new[] { typeof(World), typeof(Point2) });

        if (constructor != null)
        {
            return (Organism)constructor.Invoke(new object[] { this, position });
        }

        throw new NotSupportedException($"Cannot create organism type: {organismType.Name}");
    }

    private static int ToroidalDistance(int a, int b, int size)
    {
        var diff = Math.Abs(a - b);
        return Math.Min(diff, size - diff);
    }
}