namespace Gearspace;

public readonly struct Gear(int x, int m, int a, int s)
{
    public readonly int x = x;
    public readonly int m = m;
    public readonly int a = a;
    public readonly int s = s;

    public override string ToString() => $" x: {x} | m: {m} | a: {a} | s: {s} ";
}