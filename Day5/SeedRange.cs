namespace Day5;

class SeedRange(long start, long length)
{
    public long Start { get; set; } = start;
    public long End { get; set; } = start + length - 1;
}