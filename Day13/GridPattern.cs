namespace Day13;

class GridPattern(string pattern) : IEquatable<GridPattern>
{
    public string Pattern { get; } = pattern;

    public bool Equals(GridPattern? other)
    {
        if (other == null)
            return false;

        return Pattern == other.Pattern;
    }

    public override int GetHashCode()
    {
        return Pattern.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is not null && Equals(obj as GridPattern);
    }
}