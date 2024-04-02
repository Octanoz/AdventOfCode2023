#define PART1
// #define TEST1
#define PART2
// #define TEST2

using System.Diagnostics;
using CommEnums;
using CommModules;

Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day20\example1.txt",
    ["example2"] = @"..\Day20\example2.txt",
    ["challenge"] = @"..\Day20\input.txt"
};

#if TEST1
long result1 = PartOne(filePaths["example1"]);
Debug.Assert(result1 == 32000000, $"Result for example1 was {result1} instead of 32000000");

long result2 = PartOne(filePaths["example2"]);
Debug.Assert(result2 == 11687500, $"Result for example2 was {result2} instead of 11687500");
#endif

#if PART1
Console.WriteLine($"Multiplication of low and high pulse counts in part one: {PartOne(filePaths["challenge"])}");
#endif

#if PART2
Console.WriteLine($"Button presses before single low pulse sent to 'rx': {PartTwo(filePaths["challenge"])}");
#endif

#if PART1

int PartOne(string filePath)
{
    List<CommModule> commModules = ParseModules(filePath);
    List<string> modulesWithoutOutput = FindModulesWithoutOutput(commModules);

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

#endif

#if PART2

long PartTwo(string filePath)
{
    List<CommModule> commModules = ParseModules(filePath);
    List<string> modulesWithoutOutput = FindModulesWithoutOutput(commModules);

    Queue<(string source, Pulse pulseFrequency, string target)> modules = [];
    CommModule? startingModule = commModules.First(cm => cm.Name is "broadcaster") ??
                                    throw new InvalidDataException($"Was not able to find the starting module named 'broadcaster'");

    //The low pulse for rx needs to come from conjunction module gf in this puzzle input
    Pulse[] gfHistory = Enumerable.Repeat(Pulse.Low, 4).ToArray();
    long[] highPulseLoops = new long[4];
    var (historyHighCount, index, loop) = (0, 0, 0);

    while (true)
    {
        loop++;
        modules.Enqueue(("button", Pulse.Low, startingModule.Name));
        while (modules.Count is not 0)
        {
            (string sourceName, Pulse pulse, string targetName) = modules.Dequeue();

            if (modulesWithoutOutput.Contains(targetName))
            {
                if (targetName is "rx")
                {
                    Conjunction gfCon = commModules.Find(cm => cm.Name == sourceName) as Conjunction ??
                                        throw new InvalidDataException("Could not cast 'rx' module as Conjunction");

                    //Copy any new high pulse inputs
                    if (gfCon.InputMemory.Values.Count(p => p == Pulse.High) is not 0)
                    {
                        gfHistory[0] = gfHistory[0] == Pulse.Low ? gfCon.InputMemory["kr"] : gfHistory[0];
                        gfHistory[1] = gfHistory[1] == Pulse.Low ? gfCon.InputMemory["zs"] : gfHistory[1];
                        gfHistory[2] = gfHistory[2] == Pulse.Low ? gfCon.InputMemory["kf"] : gfHistory[2];
                        gfHistory[3] = gfHistory[3] == Pulse.Low ? gfCon.InputMemory["qk"] : gfHistory[3];
                    }

                    if (gfHistory.Count(p => p == Pulse.High) > historyHighCount)
                    {
                        historyHighCount = gfHistory.Count(p => p == Pulse.High);
                        highPulseLoops[index++] = loop;

#if TEST2
                        Console.WriteLine($"\nIn loop {loop} the history array was able to add another high pulse\n");
                        Console.WriteLine($"Actual module memory: \n{gfCon}\n\nHistory: \n{string.Join(" | ", gfHistory)}\n============");
#endif
                    }

                    if (Array.TrueForAll(gfHistory, p => p == Pulse.High))
                    {
                        long lcm = highPulseLoops.Aggregate((a, b) => a * b / GCD(a, b));
                        return lcm;

                        long GCD(long a, long b) => b is 0 ? a : GCD(b, a % b);
                    }
                }

                continue;
            }

            var currentModule = commModules.Find(cm => cm.Name == targetName) ??
                                            throw new InvalidDataException($"Could not find module named {targetName}");

            ProcessModule(currentModule, modules, (sourceName, pulse, targetName));
        }
    }
}

static void ProcessModule(CommModule currentModule, Queue<(string source, Pulse pulseFrequency, string target)> modules, (string sourceName, Pulse pulse, string targetName) pulsePacket)
{
    if (currentModule is not null)
    {
        if (currentModule.Name is "broadcaster")
        {
            foreach (var dest in currentModule.Destinations)
            {
                modules.Enqueue((currentModule.Name, pulsePacket.pulse, dest));
            }
        }
        else if (currentModule is FlipFlop currentFF)
        {
            Pulse currentPulse = currentFF.ProcessPulse(pulsePacket.pulse);
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
            Pulse currentPulse = currentCon.ProcessPulse(pulsePacket.pulse, pulsePacket.sourceName);
            foreach (var dest in currentCon.Destinations)
            {
                modules.Enqueue((currentCon.Name, currentPulse, dest));
            }
        }
    }
}

#endif

static List<CommModule> ParseModules(string filePath)
{
    List<CommModule> commModules = [];
    List<Conjunction> conjunctions = [];

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

    return commModules;
}

static List<string> FindModulesWithoutOutput(List<CommModule> commModules)
{
    List<string> modulesWithoutOutput = [];
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

    return modulesWithoutOutput;
}

