using Day5;

// string filePath = @"..\Day5\example1.txt";
string filePath = @"..\Day5\input.txt";
// ReadOnlySpan<string> input = File.ReadAllLines(filePath);

long locationOne = PartOne(filePath);
Console.WriteLine($"Lowest location number in part one is: {locationOne}");

long locationTwo = PartTwo(filePath);
Console.WriteLine($"Lowest location number in part two is: {locationTwo}");

//? 100165128 was too low

long PartOne(string filePath)
{
    long[] seedNumbers = [];
    using (StreamReader sr = new(filePath))
    {
        while (!sr.EndOfStream)
        {
            string currentLine = sr.ReadLine()!;

            if (currentLine.StartsWith("seeds"))
            {
                seedNumbers = currentLine.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                            .Skip(1)
                                            .Select(long.Parse)
                                            .ToArray();

                currentLine = sr.ReadLine()!;
                continue;
            }

            if (Char.IsLetter(currentLine[0]))
            {
                currentLine = sr.ReadLine()!;
                List<NumberRange> ranges = [];

                while (!String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long offset = parts[0];
                    long start = parts[1];
                    long range = parts[2];

                    ranges.Add(new(start, offset, range));

                    currentLine = sr.ReadLine()!;
                }

                for (int i = 0; i < seedNumbers.Length; i++)
                {
                    foreach (var range in ranges)
                    {
                        if (seedNumbers[i] >= range.Start && seedNumbers[i] <= range.End)
                        {
                            seedNumbers[i] -= range.Difference;
                            break;
                        }
                    }
                }
            }
        }
    }

    return seedNumbers.Min();
}

long PartTwo(string filePath)
{
    List<SeedRange> seedRanges = [];
    using (StreamReader sr = new(filePath))
    {
        while (!sr.EndOfStream)
        {
            string currentLine = sr.ReadLine()!;

            if (currentLine.StartsWith("seeds"))
            {
                long[] parts = currentLine.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                            .Skip(1)
                                            .Select(long.Parse)
                                            .ToArray();

                long start = 0;
                for (int i = 0; i < parts.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        start = parts[i];
                    }
                    else
                    {
                        seedRanges.Add(new(start, parts[i]));
                    }
                }

                seedRanges = [.. seedRanges.OrderBy(s => s.Start)];

                currentLine = sr.ReadLine()!;
                continue;
            }

            if (Char.IsLetter(currentLine[0]))
            {
                currentLine = sr.ReadLine()!;
                List<NumberRange> ranges = [];

                while (!String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long offset = parts[0];
                    long start = parts[1];
                    long range = parts[2];

                    ranges.Add(new(start, offset, range));

                    currentLine = sr.ReadLine()!;
                }

                ranges = [.. ranges.OrderBy(r => r.Start)];

                List<SeedRange> newRanges = [];
                for (int i = 0; i < seedRanges.Count; i++)
                {
                    var seedRange = seedRanges[i];
                    foreach (var numberRange in ranges)
                    {
                        //Entire seedrange comes before current numberrange
                        if (seedRange.End < numberRange.Start)
                        {
                            newRanges.Add(seedRange);
                            break;
                        }
                        //Entire seedrange comes after current numberrange
                        else if (seedRange.Start > numberRange.End)
                        {
                            if (numberRange == ranges[^1])
                            {
                                newRanges.Add(seedRange);
                            }
                            else continue;
                        }
                        else if (seedRange.Start < numberRange.Start)
                        {
                            //seedrange starts before numberrange and ends after numberrange
                            if (seedRange.End > numberRange.End)
                            {
                                newRanges.Add(new(seedRange.Start, numberRange.Start - seedRange.Start));
                                seedRange.Start = numberRange.Start;

                                seedRanges.Insert(i + 1, new(numberRange.End + 1, seedRange.End - numberRange.End));
                                seedRange.End = numberRange.End;

                                seedRange.Start -= numberRange.Difference;
                                seedRange.End -= numberRange.Difference;
                                newRanges.Add(seedRange);

                                break;
                            }
                            //seedrange end is within the numberrange
                            else if (seedRange.End > numberRange.Start)
                            {
                                newRanges.Add(new(seedRange.Start, numberRange.Start - seedRange.Start));
                                seedRange.Start = numberRange.Start;

                                seedRange.Start -= numberRange.Difference;
                                seedRange.End -= numberRange.Difference;
                                newRanges.Add(seedRange);

                                break;
                            }
                        }
                        //seedrange start is within the numberrange
                        else if (seedRange.Start >= numberRange.Start)
                        {
                            //seedrange end is outside the numberrange
                            if (seedRange.End > numberRange.End)
                            {
                                seedRanges.Insert(i + 1, new(numberRange.End + 1, seedRange.End - numberRange.End));
                                seedRange.End = numberRange.End;

                                seedRange.Start -= numberRange.Difference;
                                seedRange.End -= numberRange.Difference;

                                newRanges.Add(seedRange);

                                break;
                            }
                            //seedrange end is within the numberrange
                            else if (seedRange.End <= numberRange.End)
                            {
                                seedRange.Start -= numberRange.Difference;
                                seedRange.End -= numberRange.Difference;

                                newRanges.Add(seedRange);

                                break;
                            }
                        }
                    }
                }

                seedRanges = newRanges;
            }
        }
    }

    return seedRanges.OrderBy(s => s.Start).First().Start;
}
