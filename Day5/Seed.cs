namespace Day5;

public class Seed(long seedNumber)
{
    public long SeedNumber { get; } = seedNumber;
    public long Soil { get; set; } = seedNumber;
    public long Fertilizer { get; set; } = seedNumber;
    public long Water { get; set; } = seedNumber;
    public long Light { get; set; } = seedNumber;
    public long Temperature { get; set; } = seedNumber;
    public long Humidity { get; set; } = seedNumber;
    public long Location { get; set; } = seedNumber;
}