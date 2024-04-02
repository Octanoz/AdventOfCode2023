using CommEnums;

namespace CommModules;

public class CommModule(string Name, string[] Destinations)
{
    public string Name { get; } = Name;
    public string[] Destinations { get; } = Destinations;
}

public class FlipFlop(string Name, string[] Destinations) : CommModule(Name, Destinations)
{
    public ModuleState State { get; set; } = ModuleState.Off;
}

public static class FlipFlopExtensions
{
    public static Pulse ProcessPulse(this FlipFlop flipFlop, Pulse pulse)
    {
        if (pulse == Pulse.Low)
        {
            if (flipFlop.State == ModuleState.Off)
            {
                flipFlop.State = ModuleState.On;
                return Pulse.High;
            }
            else
            {
                flipFlop.State = ModuleState.Off;
                return Pulse.Low;
            }
        }

        return Pulse.None;
    }
}

public class Conjunction(string Name, string[] Destinations) : CommModule(Name, Destinations)
{
    public Dictionary<string, Pulse> InputMemory { get; } = [];

    public override string ToString()
    {
        return $"Con-{Name} [ {String.Join(" | ", InputMemory.Select(kvp => $"{kvp.Key}: {kvp.Value}"))} ]";
    }
}

public static class ConjunctionExtensions
{
    public static Pulse ProcessPulse(this Conjunction conjunction, Pulse pulse, string source)
    {
        conjunction.InputMemory[source] = pulse;

        if (conjunction.InputMemory.Values.All(p => p == Pulse.High))
            return Pulse.Low;

        return Pulse.High;
    }

    public static void SetMemory(this Conjunction conjunction, string[] sources)
    {
        if (conjunction.InputMemory.Count is not 0)
            throw new ArgumentException($"Conjunction module {conjunction.Name} already has memory set but SetMemory was called again");

        foreach (var source in sources)
        {
            if (!conjunction.InputMemory.TryAdd(source, Pulse.Low))
            {
                throw new ArgumentException($"Source {source} already exists in conjunction module {conjunction.Name}");
            }
        }
    }
}

