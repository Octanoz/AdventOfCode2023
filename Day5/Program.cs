using System.Diagnostics;
using Day5;

// string filePath = @"..\Day5\example1.txt";
string filePath = @"..\Day5\input.txt";
// ReadOnlySpan<string> input = File.ReadAllLines(filePath);

long locationOne = PartOne(filePath);
Console.WriteLine($"Lowest location number in part one is: {locationOne}");

long locationOneSeed = PartOneSeed(filePath);
Console.WriteLine($"Lowest location number in part one using Class is: {locationOneSeed}");

// long locationTwo = PartTwo(filePath);
// Console.WriteLine($"Lowest location number in part two is: {locationTwo}");

long PartOneSeed(string filePath)
{
    List<Seed> seeds = [];
    using (StreamReader sr = new(filePath))
    {
        while (!sr.EndOfStream)
        {
            string currentLine = sr.ReadLine()!;

            if (currentLine.StartsWith("seeds"))
            {
                var seedNumbers = currentLine.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                                                .Skip(1)
                                                                .Select(long.Parse);

                foreach (var seed in seedNumbers)
                    seeds.Add(new(seed));
            }

            if (currentLine.StartsWith("seed-"))
            {
                currentLine = sr.ReadLine()!;

                while (!String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long soilNumber = parts[0];
                    long seedFirst = parts[1];
                    long amount = parts[2];

                    long difference = seedFirst - soilNumber;
                    long seedLast = seedFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.SeedNumber >= seedFirst && seed.SeedNumber <= seedLast)
                            seed.Soil = seed.SeedNumber - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("soil-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Fertilizer = seed.Soil;

                while (!String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long fertilizerNumber = parts[0];
                    long soilFirst = parts[1];
                    long amount = parts[2];

                    long difference = soilFirst - fertilizerNumber;
                    long soilLast = soilFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Soil >= soilFirst && seed.Soil <= soilLast)
                            seed.Fertilizer = seed.Soil - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("fertilizer-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Water = seed.Fertilizer;

                while (!String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long waterNumber = parts[0];
                    long fertilizerFirst = parts[1];
                    long amount = parts[2];

                    long difference = fertilizerFirst - waterNumber;
                    long fertilizerLast = fertilizerFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Fertilizer >= fertilizerFirst && seed.Fertilizer <= fertilizerLast)
                            seed.Water = seed.Fertilizer - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("water-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Light = seed.Water;

                while (!String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long lightNumber = parts[0];
                    long waterFirst = parts[1];
                    long amount = parts[2];

                    long difference = waterFirst - lightNumber;
                    long waterLast = waterFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Water >= waterFirst && seed.Water <= waterLast)
                            seed.Light = seed.Water - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("light-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Temperature = seed.Light;

                while (!String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long temperatureNumber = parts[0];
                    long lightFirst = parts[1];
                    long amount = parts[2];

                    long difference = lightFirst - temperatureNumber;
                    long lightLast = lightFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Light >= lightFirst && seed.Light <= lightLast)
                            seed.Temperature = seed.Light - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("temperature-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Humidity = seed.Temperature;

                while (!String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long humidityNumber = parts[0];
                    long temperatureFirst = parts[1];
                    long amount = parts[2];

                    long difference = temperatureFirst - humidityNumber;
                    long temperatureLast = temperatureFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Temperature >= temperatureFirst && seed.Temperature <= temperatureLast)
                            seed.Humidity = seed.Temperature - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("humidity-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Location = seed.Humidity;

                while (!String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long locationNumber = parts[0];
                    long humidityFirst = parts[1];
                    long amount = parts[2];

                    long difference = humidityFirst - locationNumber;
                    long humidityLast = humidityFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Humidity >= humidityFirst && seed.Humidity <= humidityLast)
                            seed.Location = seed.Humidity - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }
        }
    }

    return seeds.OrderBy(s => s.Location).First().Location;
}

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
                List<long> changed = [];

                while (!sr.EndOfStream && !String.IsNullOrEmpty(currentLine))
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long newNumber = parts[0];
                    long originalFirst = parts[1];
                    long amount = parts[2];

                    long difference = originalFirst - newNumber;
                    long originalLast = originalFirst + amount - 1;

                    for (long i = 0; i < seedNumbers.Length; i++)
                    {
                        if (changed.Contains(i))
                            continue;

                        if (seedNumbers[i] >= originalFirst && seedNumbers[i] <= originalLast)
                        {
                            seedNumbers[i] -= difference;
                            changed.Add(i);
                        }
                    }

                    currentLine = sr.ReadLine()!;
                }
            }
        }
    }

    return seedNumbers.Min();
}

long PartTwo(string filePath)
{
    List<Seed> seeds = [];
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

                long firstNumber = 0;
                long numberRange = 0;
                HashSet<long> seedNumbers = [];
                for (int i = 0; i < parts.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        firstNumber = parts[i];
                    }
                    else
                    {
                        numberRange = parts[i];
                        for (long j = 0; j < numberRange; j++)
                        {
                            seedNumbers.Add(firstNumber + j);
                        }
                    }
                }

                foreach (var seed in seedNumbers)
                    seeds.Add(new(seed));
            }

            if (currentLine.StartsWith("seed-"))
            {
                currentLine = sr.ReadLine()!;

                while (currentLine != "")
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long soilNumber = parts[0];
                    long seedFirst = parts[1];
                    long amount = parts[2];

                    long difference = seedFirst - soilNumber;
                    long seedLast = seedFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.SeedNumber >= seedFirst && seed.SeedNumber <= seedLast)
                            seed.Soil = seed.SeedNumber - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("soil-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Fertilizer = seed.Soil;

                while (currentLine != "")
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long fertilizerNumber = parts[0];
                    long soilFirst = parts[1];
                    long amount = parts[2];

                    long difference = soilFirst - fertilizerNumber;
                    long soilLast = soilFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Soil >= soilFirst && seed.Soil <= soilLast)
                            seed.Fertilizer = seed.Soil - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("fertilizer-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Water = seed.Fertilizer;

                while (currentLine != "")
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long waterNumber = parts[0];
                    long fertilizerFirst = parts[1];
                    long amount = parts[2];

                    long difference = fertilizerFirst - waterNumber;
                    long fertilizerLast = fertilizerFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Fertilizer >= fertilizerFirst && seed.Fertilizer <= fertilizerLast)
                            seed.Water = seed.Fertilizer - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("water-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Light = seed.Water;

                while (currentLine != "")
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long lightNumber = parts[0];
                    long waterFirst = parts[1];
                    long amount = parts[2];

                    long difference = waterFirst - lightNumber;
                    long waterLast = waterFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Water >= waterFirst && seed.Water <= waterLast)
                            seed.Light = seed.Water - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("light-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Temperature = seed.Light;

                while (currentLine != "")
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long temperatureNumber = parts[0];
                    long lightFirst = parts[1];
                    long amount = parts[2];

                    long difference = lightFirst - temperatureNumber;
                    long lightLast = lightFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Light >= lightFirst && seed.Light <= lightLast)
                            seed.Temperature = seed.Light - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("temperature-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Humidity = seed.Temperature;

                while (currentLine != "")
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long humidityNumber = parts[0];
                    long temperatureFirst = parts[1];
                    long amount = parts[2];

                    long difference = temperatureFirst - humidityNumber;
                    long temperatureLast = temperatureFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Temperature >= temperatureFirst && seed.Temperature <= temperatureLast)
                            seed.Humidity = seed.Temperature - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }

            if (currentLine.StartsWith("humidity-"))
            {
                currentLine = sr.ReadLine()!;

                foreach (var seed in seeds)
                    seed.Location = seed.Humidity;

                while (!sr.EndOfStream)
                {
                    long[] parts = currentLine.Split(' ').Select(long.Parse).ToArray();
                    long locationNumber = parts[0];
                    long humidityFirst = parts[1];
                    long amount = parts[2];

                    long difference = humidityFirst - locationNumber;
                    long humidityLast = humidityFirst + amount - 1;

                    foreach (var seed in seeds)
                    {
                        if (seed.Humidity >= humidityFirst && seed.Humidity <= humidityLast)
                            seed.Location = seed.Humidity - difference;
                    }

                    currentLine = sr.ReadLine()!;
                }
            }
        }
    }

    return seeds.OrderBy(s => s.Location).First().Location;
}