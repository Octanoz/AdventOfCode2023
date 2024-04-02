#define TEST
// #define PART1
#define PART2

using System.Diagnostics;
using CommEnums;
using CommModules;

Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day20\example1.txt",
    ["example2"] = @"..\Day20\example2.txt",
    ["challenge"] = @"..\Day20\input.txt"
};

#if TEST
long result1 = PartOne(filePaths["example1"]);
Debug.Assert(result1 == 32000000, $"Result for example1 was {result1} instead of 32000000");

long result2 = PartOne(filePaths["example2"]);
Debug.Assert(result2 == 11687500, $"Result for example2 was {result2} instead of 11687500");
#endif

#if PART1
Console.WriteLine($"Multiplication of low and high pulse counts in part one: {PartOne(filePaths["challenge"])}");
#endif

#if PART2
Console.WriteLine($"Button presses before single pulse sent to 'rx': {PartTwo(filePaths["challenge"])}");
#endif

int PartOne(string filePath)
{
    List<Conjunction> conjunctions = [];
    List<CommModule> commModules = [];
    List<string> modulesWithoutOutput = [];

    using StreamReader sr = new(filePath);

    while (!sr.EndOfStream)
    {
        string currentString = sr.ReadLine() ?? throw new InvalidDataException("End of file reached unexpectedly");

        string[] parts = currentString.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
        string[] destinations = parts[1].Split(", ", StringSplitOptions.RemoveEmptyEntries);


        if (parts[0] is "broadcaster")
        {
            commModules.Add(new(parts[0], destinations));
        }
        else if (parts[0].StartsWith('%'))
        {
            commModules.Add(new FlipFlop(parts[0][1..], destinations));
        }
        else if (parts[0].StartsWith('&'))
        {
            conjunctions.Add(new Conjunction(parts[0][1..], destinations));
        }
    }

    foreach (var con in conjunctions)
    {
        string[] sources = commModules.Where(cm => cm.Destinations
                                            .Contains(con.Name))
                                        .Select(cm => cm.Name)
                                        .ToArray();

        con.SetMemory(sources);

        commModules.Add(con);
    }

    foreach (var module in commModules)
    {
        foreach (var dest in module.Destinations)
        {
            if (!commModules.Exists(cm => cm.Name == dest))
            {
                modulesWithoutOutput.Add(dest);
            }
        }
    }

    Queue<(string, Pulse, string)> modules = [];
    CommModule? startingModule = commModules.First(cm => cm.Name is "broadcaster") ??
                                    throw new InvalidDataException($"Was not able to find the starting module named 'broadcaster'");

    var (lowPulseCount, highPulseCount) = (0, 0);

    for (int i = 0; i < 1000; i++)
    {
        modules.Enqueue(("button", Pulse.Low, startingModule.Name));
        lowPulseCount++;

        while (modules.Count is not 0)
        {
            (string sourceName, Pulse pulse, string targetName) = modules.Dequeue();
            if (modulesWithoutOutput.Contains(targetName))
                continue;

            var currentModule = commModules.Find(cm => cm.Name == targetName) ??
                                            throw new InvalidDataException($"Could not find module named {targetName}");

            if (currentModule is not null)
            {
                if (currentModule.Name is "broadcaster")
                {
                    foreach (var dest in currentModule.Destinations)
                    {
                        modules.Enqueue((currentModule.Name, pulse, dest));

                        if (pulse is Pulse.Low)
                        {
                            lowPulseCount++;
                        }
                        else highPulseCount++;
                    }
                }
                else if (currentModule is FlipFlop currentFF)
                {
                    Pulse currentPulse = currentFF.ProcessPulse(pulse);
                    if (currentPulse is not Pulse.None)
                    {
                        foreach (var dest in currentFF.Destinations)
                        {
                            modules.Enqueue((currentFF.Name, currentPulse, dest));

                            if (currentPulse is Pulse.Low)
                            {
                                lowPulseCount++;
                            }
                            else highPulseCount++;
                        }
                    }
                }
                else if (currentModule is Conjunction currentCon)
                {
                    Pulse currentPulse = currentCon.ProcessPulse(pulse, sourceName);

                    foreach (var dest in currentCon.Destinations)
                    {

                        modules.Enqueue((currentCon.Name, currentPulse, dest));

                        if (currentPulse is Pulse.Low)
                        {
                            lowPulseCount++;
                        }
                        else highPulseCount++;
                    }
                }
            }
        }
    }

    return lowPulseCount * highPulseCount;
}

long PartTwo(string filePath)
{
    List<Conjunction> conjunctions = [];
    List<CommModule> commModules = [];
    List<string> modulesWithoutOutput = [];

    using StreamReader sr = new(filePath);

    while (!sr.EndOfStream)
    {
        string currentString = sr.ReadLine() ?? throw new InvalidDataException("End of file reached unexpectedly");

        string[] parts = currentString.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
        string[] destinations = parts[1].Split(", ", StringSplitOptions.RemoveEmptyEntries);


        if (parts[0] is "broadcaster")
        {
            commModules.Add(new(parts[0], destinations));
        }
        else if (parts[0].StartsWith('%'))
        {
            commModules.Add(new FlipFlop(parts[0][1..], destinations));
        }
        else if (parts[0].StartsWith('&'))
        {
            conjunctions.Add(new Conjunction(parts[0][1..], destinations));
        }
    }

    foreach (var con in conjunctions)
    {
        var regularInputs = commModules.Where(cm => cm.Destinations.Contains(con.Name))
                                        .Select(cm => cm.Name);

        var conjunctionInputs = conjunctions.Where(c => c.Destinations.Contains(con.Name))
                                            .Select(con => con.Name);

        string[] sources = regularInputs.Concat(conjunctionInputs).Distinct().ToArray();
        con.SetMemory(sources);

        commModules.Add(con);
    }

    foreach (var module in commModules)
    {
        foreach (var dest in module.Destinations)
        {
            if (!commModules.Exists(cm => cm.Name == dest))
            {
                modulesWithoutOutput.Add(dest);
            }
        }
    }

    Queue<(string source, Pulse pulseFrequency, string target)> modules = [];
    CommModule? startingModule = commModules.First(cm => cm.Name is "broadcaster") ??
                                    throw new InvalidDataException($"Was not able to find the starting module named 'broadcaster'");

#if TEST
    int highCount = 0;
#endif

    long loop = 0;
    while (true)
    {
        loop++;
        modules.Enqueue(("button", Pulse.Low, startingModule.Name));
        int lowPulseCount = 0;
        while (modules.Count is not 0)
        {
            (string sourceName, Pulse pulse, string targetName) = modules.Dequeue();
            if (modulesWithoutOutput.Contains(targetName))
            {
                if (targetName is "rx")
                {
#if TEST
                    Conjunction gfCon = commModules.Find(cm => cm.Name == sourceName) as Conjunction ??
                                        throw new InvalidDataException("Could not cast 'rx' module as Conjunction");

                    int gfHighCount = gfCon.InputMemory.Values.Count(v => v == Pulse.High);
                    if (gfHighCount != highCount && (gfHighCount is 0 && highCount > 1 || gfHighCount >= 2))
                    {
                        highCount = gfHighCount;
                        Console.WriteLine(gfCon);
                        Console.WriteLine($"More than one difference found in loop {loop}\n============\n");
                    }
#endif

                    if (pulse is Pulse.Low)
                        lowPulseCount++;
                }

                continue;
            }

            var currentModule = commModules.Find(cm => cm.Name == targetName) ??
                                            throw new InvalidDataException($"Could not find module named {targetName}");

            if (currentModule is not null)
            {
                if (currentModule.Name is "broadcaster")
                {
                    foreach (var dest in currentModule.Destinations)
                    {
                        modules.Enqueue((currentModule.Name, pulse, dest));
                    }
                }
                else if (currentModule is FlipFlop currentFF)
                {
                    Pulse currentPulse = currentFF.ProcessPulse(pulse);
                    if (currentPulse is not Pulse.None)
                    {
                        foreach (var dest in currentFF.Destinations)
                        {
                            modules.Enqueue((currentFF.Name, currentPulse, dest));
                        }
                    }
                }
                else if (currentModule is Conjunction currentCon)
                {
                    Pulse currentPulse = currentCon.ProcessPulse(pulse, sourceName);

                    foreach (var dest in currentCon.Destinations)
                    {
                        modules.Enqueue((currentCon.Name, currentPulse, dest));
                    }
                }
            }
        }

        if (loop % 1_000 is 0)
            Debug.WriteLine($"Loop {loop,10} completed");

        if (lowPulseCount is 1)
            return loop;
    }
}