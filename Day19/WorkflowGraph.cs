using Workflows;

namespace WorkflowGraph;

public class Graph
{
    private Dictionary<WorkflowNode, List<WorkflowNode>> adjacencyList;

    public Graph()
    {
        adjacencyList = [];
    }

    public void AddNode(WorkflowNode node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList[node] = [];
        }
    }

    public void AddEdge(WorkflowNode from, WorkflowNode to)
    {
        AddNode(from);
        AddNode(to);

        adjacencyList[from].Add(to);
    }

    public List<WorkflowNode> GetNeighbours(WorkflowNode node)
    {
        if (adjacencyList.TryGetValue(node, out List<WorkflowNode>? neighbours))
        {
            return neighbours;
        }
        else return [];
    }

}

