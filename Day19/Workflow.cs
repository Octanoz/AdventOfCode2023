using Gearspace;

namespace Workflows;

public class Workflow(string Name, string DefaultRule)
{
    public string Name { get; } = Name;
    public string DefaultRule { get; } = DefaultRule;
    public List<Func<Gear, string>> Rules { get; set; } = [];

    public static int GetGearAttribute(Gear gear, char gearAttribute)
    {
        return gearAttribute switch
        {
            'x' => gear.x,
            'm' => gear.m,
            'a' => gear.a,
            's' => gear.s,
            _ => throw new ArgumentException($"Unknown gear attribute: {gearAttribute}")
        };
    }

    public static Workflow SetupWorkflowRules(Workflow workflow, string[] comparisons)
    {
        for (int i = 0; i < comparisons.Length; i++)
        {
            char gearAttribute = comparisons[i][0];
            string[] valueAndDestination = comparisons[i][2..].Split(':');
            int comparisonValue = int.Parse(valueAndDestination[0]);
            string destination = valueAndDestination[1];

            //Assign workflow rules, depending on how many were present in the comparisons array
            Func<Gear, bool> comparisonFunction;
            if (comparisons[i][1] is '<')
            {
                comparisonFunction = gear => GetGearAttribute(gear, gearAttribute) < comparisonValue;
            }
            else comparisonFunction = gear => GetGearAttribute(gear, gearAttribute) > comparisonValue;

            workflow.AddRule(comparisonFunction, destination);
        }

        return workflow;
    }
}

public class WorkflowBlueprint(string Name, string DefaultRule, string[] Rules)
{
    public string Name { get; } = Name;
    public string DefaultRule { get; } = DefaultRule;
    public string[] Rules { get; set; } = Rules;

    public static void GenerateChildren(WorkflowNode currentNode, WorkflowBlueprint currentBlueprint)
    {
        string[] comparisons = currentBlueprint.Rules;
        for (int i = 0; i < comparisons.Length; i++)
        {
            //{a<2000:kdq}
            char gearAttribute = comparisons[i][0]; //a
            string[] valueAndDestination = comparisons[i][2..].Split(':'); //2000 kdq
            int comparisonValue = int.Parse(valueAndDestination[0]); //2000
            string destination = valueAndDestination[1]; //kdq

            //Assign workflow rules, depending on how many were present in the comparisons array
            if (currentNode.AvailableRanges.TryGetValue(gearAttribute, out List<int>? storedRange))
            {
                if (comparisons[i][1] is '<')
                {
                    List<int> childRanges = storedRange.Where(x => x < comparisonValue).ToList();
                    WorkflowNode childNode = GenerateChildNode(currentNode, gearAttribute, destination, childRanges);

                    currentNode.Children.Add(childNode);
                    currentNode.AvailableRanges[gearAttribute] = currentNode.AvailableRanges[gearAttribute].Where(c => !childRanges.Contains(c)).ToList();
                }
                else
                {
                    List<int> childRanges = [.. storedRange.Where(x => x > comparisonValue)];
                    WorkflowNode childNode = GenerateChildNode(currentNode, gearAttribute, destination, childRanges);

                    currentNode.Children.Add(childNode);
                    currentNode.AvailableRanges[gearAttribute] = currentNode.AvailableRanges[gearAttribute].Where(c => !childRanges.Contains(c)).ToList();
                }
            }
        }

        WorkflowNode defaultChild = new(currentBlueprint.DefaultRule, currentNode.AvailableRanges);
        currentNode.Children.Add(defaultChild);
    }

    private static WorkflowNode GenerateChildNode(WorkflowNode currentNode, char gearAttribute, string destination, List<int> childRanges)
    {
        WorkflowNode childNode = new(destination, currentNode.AvailableRanges);
        childNode.AvailableRanges[gearAttribute] = childRanges; //a: 0-1999
        return childNode;
    }
}

public static class WorkflowExtensions
{
    public static void AddRule(this Workflow currentWorkflow, Func<Gear, bool> comparisonFunction, string destination)
    {
        if (currentWorkflow.Rules.Count is not 3)
        {
            currentWorkflow.Rules.Add(gear => comparisonFunction(gear) ? destination : "");
        }
        else throw new InvalidOperationException($"Workflow {currentWorkflow.Name} already has 3 rules but AddRule is prompted a fourth time.");
    }

    public static string ProcessGear(this Workflow activeWorkflow, Gear gear)
    {
        foreach (var rule in activeWorkflow.Rules)
        {
            string result = rule(gear);

            if (result is not "")
                return result;
        }

        return activeWorkflow.DefaultRule;
    }
}

public class WorkflowNode(string WorkflowName, Dictionary<char, List<int>> AvailableRanges)
{
    public string WorkflowName { get; } = WorkflowName;
    public Dictionary<char, List<int>> AvailableRanges { get; } = AvailableRanges ?? new()
    {
        ['x'] = Enumerable.Range(0, 4000).ToList(),
        ['m'] = Enumerable.Range(0, 4000).ToList(),
        ['a'] = Enumerable.Range(0, 4000).ToList(),
        ['s'] = Enumerable.Range(0, 4000).ToList()
    };
    public List<WorkflowNode> Children { get; set; } = [];

}
