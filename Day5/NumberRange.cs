namespace Day5;

record NumberRange(long Start, long Offset, long Range)
{
    public long End { get; } = Start + Range - 1;
    public long Difference { get; } = Start - Offset;
}
