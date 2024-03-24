namespace CoordXL;

public record CoordL(long Row, long Col)
{
    public static CoordL Zero => new(0, 0);

    public CoordL Up => new(Row - 1, Col);
    public CoordL Down => new(Row + 1, Col);
    public CoordL Left => new(Row, Col - 1);
    public CoordL Right => new(Row, Col + 1);

    public IEnumerable<CoordL> Neighbours => [Up, Down, Left, Right];

    public static CoordL operator +(CoordL a, CoordL b) => new(a.Row + b.Row, a.Col + b.Col);
    public static CoordL operator -(CoordL a, CoordL b) => new(a.Row - b.Row, a.Col - b.Col);

    public long Manhattan(CoordL other) => Math.Abs(Row - other.Row) + Math.Abs(Col - other.Col);

    public override string ToString() => $"[{Row}, {Col}]";
}

public record CoordUL(ulong Row, ulong Col)
{
    public static CoordUL Zero => new(0, 0);

    public CoordUL Up => new(Row - 1, Col);
    public CoordUL Down => new(Row + 1, Col);
    public CoordUL Left => new(Row, Col - 1);
    public CoordUL Right => new(Row, Col + 1);

    public IEnumerable<CoordUL> Neighbours => [Up, Down, Left, Right];

    public static CoordUL operator +(CoordUL a, CoordUL b) => new(a.Row + b.Row, a.Col + b.Col);
    public static CoordUL operator -(CoordUL a, CoordUL b) => new(a.Row - b.Row, a.Col - b.Col);

    public ulong Manhattan(CoordUL other) => Row - other.Row + Col - other.Col;

    public override string ToString() => $"[{Row}, {Col}]";
}