#define TEST
// #define PART1
#define PART2

using Gearspace;
using Workflows;

Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day19\example1.txt",
    ["challenge"] = @"..\Day19\input.txt"
};

#if PART1
Console.WriteLine($"Grand total of all gear ratings in part one is {PartOne(filePaths)}");
#endif

#if PART2
Console.WriteLine($"Grand total of all gear ratings in part two is {PartTwo(filePaths)}");
#endif

int PartOne(Dictionary<string, string> filePaths)
{
    Dictionary<string, Workflow> storedWorkflows = [];
    List<Gear> acceptedGears = [];

    using StreamReader sr = new(filePaths["challenge"]);
    string currentString = sr.ReadLine() ?? throw new InvalidDataException("Unable to read the input file.");

    while (!String.IsNullOrEmpty(currentString))
    {
        //Workflows line format: px{a<2006:qkq,m>2090:A,rfg}
        int openBrace = currentString.IndexOf('{');
        int lastComma = currentString.LastIndexOf(',');
        Workflow baseWorkflow = new(currentString[..openBrace], currentString[(lastComma + 1)..^1]);

        string[] comparisons = currentString[(openBrace + 1)..lastComma].Split(',');
        Workflow fullWorkflow = Workflow.SetupWorkflowRules(baseWorkflow, comparisons);
        storedWorkflows.Add(fullWorkflow.Name, fullWorkflow);

        currentString = sr.ReadLine() ??
                        throw new InvalidDataException($"Unexpected end of file after {storedWorkflows.Count} workflows: processing {storedWorkflows.Keys.Last()}");
    }

    while (!sr.EndOfStream)
    {
        currentString = sr.ReadLine() ?? throw new InvalidDataException($"Unexpected early exit in second while loop after processing {acceptedGears.Count} gears. Last accepted gear: [{acceptedGears[^1]}]");

        //Gear line format: {x=787,m=2655,a=1222,s=2876}
        int[] gearAttributes = currentString[2..^1].Split(new[] { '=', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                .Where(s => Char.IsDigit(s[0]))
                                                                .Select(Int32.Parse)
                                                                .ToArray();

        Gear currentGear = new(gearAttributes[0], gearAttributes[1], gearAttributes[2], gearAttributes[3]);

        string currentWorkflow = "in";
        while (currentWorkflow is not "R" and not "A") //R = rejected, A = accepted
        {
            if (storedWorkflows.TryGetValue(currentWorkflow, out Workflow? activeWorkflow))
                currentWorkflow = activeWorkflow.ProcessGear(currentGear);
        }

        if (currentWorkflow is "A")
        {
            acceptedGears.Add(currentGear);
        }
    }

    return acceptedGears.Select(g => g).Aggregate(0, (acc, g) => acc + g.x + g.m + g.a + g.s);
}

long PartTwo(Dictionary<string, string> filePaths)
{
    Dictionary<string, WorkflowBlueprint> workflowBlueprints = [];
    List<long> acceptedRatings = [];

    using StreamReader sr = new(filePaths["challenge"]);
    string currentString = sr.ReadLine() ?? throw new InvalidDataException("Unable to read the input file.");

    while (!String.IsNullOrEmpty(currentString))
    {
        //Workflows line format: px{a<2006:qkq,m>2090:A,rfg}
        int openBrace = currentString.IndexOf('{');
        int lastComma = currentString.LastIndexOf(',');
        string[] comparisons = currentString[(openBrace + 1)..lastComma].Split(',');
        WorkflowBlueprint baseWorkflow = new(currentString[..openBrace], currentString[(lastComma + 1)..^1], comparisons);
        workflowBlueprints.Add(baseWorkflow.Name, baseWorkflow);

        currentString = sr.ReadLine() ??
                        throw new InvalidDataException($"Unexpected end of file after {workflowBlueprints.Count} workflows: processing {workflowBlueprints.Keys.Last()}");
    }

    Queue<WorkflowNode> workflowQueue = [];
    workflowQueue.Enqueue(new("in", null!));
    while (workflowQueue.Count is not 0)
    {
        WorkflowNode currentNode = workflowQueue.Dequeue();

        if (currentNode.WorkflowName is "R")
            continue;

        if (currentNode.WorkflowName is "A")
        {
            acceptedRatings.Add(currentNode.AvailableRanges['x'].Count * currentNode.AvailableRanges['m'].Count *
                                        currentNode.AvailableRanges['a'].Count * currentNode.AvailableRanges['s'].Count);

            continue;
        }

        WorkflowBlueprint currentBlueprint = workflowBlueprints[currentNode.WorkflowName];
        WorkflowBlueprint.GenerateChildren(currentNode, currentBlueprint);

        foreach (var childNode in currentNode.Children)
        {
            workflowQueue.Enqueue(childNode);
        }
    }

    return acceptedRatings.Sum();
}
