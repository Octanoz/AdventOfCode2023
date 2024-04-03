using System.Collections.Concurrent;
using StandardDirections;

namespace AStar;

public struct AStarNode(int Row, int Col)
{
    public int Row { get; } = Row;
    public int Col { get; } = Col;
    public Direction Direction { get; set; }
    public int DirectionSteps { get; set; }
}

public class AStarPathfinder(int[,] grid)
{
    private readonly int[,] grid = grid;
    private readonly int rows = grid.GetLength(0);
    private readonly int cols = grid.GetLength(1);


    public List<AStarNode> OptimalPath()
    {
        AStarNode startNode = new(0, 0) { Direction = Direction.Down };
        AStarNode endNode = new(rows - 1, cols - 1);
        PriorityQueue<AStarNode, int> openQueue = new();
        openQueue.Enqueue(startNode, 0);

        Dictionary<AStarNode, AStarNode> cameFrom = [];
        Dictionary<AStarNode, int> gScore = new() { [startNode] = 0 }; // Accumulated heat loss
        Dictionary<AStarNode, int> fScore = new() { [endNode] = Heuristic(startNode, endNode) }; // Accumulated heat loss + manhattan distance (heuristic)

        while (openQueue.TryDequeue(out AStarNode currentNode, out int currentF))
        {
            //Check if current node is the goal
            if (currentNode.Row.Equals(endNode.Row) && currentNode.Col.Equals(endNode.Col))
            {
                return ReconstructPath(currentNode, cameFrom);
            }

            //Explore neighbours
            foreach (var dir in PossibleDirections(currentNode))
            {
                (int newRow, int newCol) = dir switch
                {
                    Direction.Up => (currentNode.Row - 1, currentNode.Col),
                    Direction.Right => (currentNode.Row, currentNode.Col + 1),
                    Direction.Down => (currentNode.Row + 1, currentNode.Col),
                    _ => (currentNode.Row, currentNode.Col - 1)
                };

                //Make sure we're within bounds
                if (WithinBounds(newRow, rows, newCol, cols))
                {
                    AStarNode neighbour = new(newRow, newCol)
                    {
                        Direction = dir,
                        DirectionSteps = dir == currentNode.Direction ? currentNode.DirectionSteps + 1 : 1
                    };

                    //Apply tentative G cost
                    int tentativeG = gScore[currentNode] + grid[newRow, newCol];

                    //Check if neighbour already exists in open queue or if tentativeG is lower than the storedG in the dictionary
                    if (!gScore.TryGetValue(neighbour, out int neighbourG) || tentativeG < neighbourG)
                    {
                        cameFrom[neighbour] = currentNode;
                        gScore[neighbour] = tentativeG;
                        fScore[neighbour] = tentativeG + Heuristic(neighbour, endNode);

                        if (!openQueue.UnorderedItems.Any(item => item.Element.Equals(neighbour)))
                        {
                            openQueue.Enqueue(neighbour, fScore[neighbour]);
                        }
                    }
                }
            }
        }

        return [];
    }

    public List<AStarNode> OptimalPath2()
    {
        AStarNode startNodeDown = new(0, 0) { Direction = Direction.Down };
        AStarNode startNodeRight = new(0, 0) { Direction = Direction.Right };
        AStarNode endNode = new(rows - 1, cols - 1);
        PriorityQueue<AStarNode, int> openQueue = new();
        openQueue.Enqueue(startNodeDown, 0);
        openQueue.Enqueue(startNodeRight, 0);

        Dictionary<AStarNode, AStarNode> cameFrom = [];
        Dictionary<AStarNode, int> gScore = new()
        {
            [startNodeDown] = 0,
            [startNodeRight] = 0
        };

        Dictionary<AStarNode, int> fScore = new() { [endNode] = Heuristic(startNodeDown, endNode) }; // Accumulated heat loss + manhattan distance (heuristic)

        while (openQueue.TryDequeue(out AStarNode currentNode, out int currentF))
        {
            //Check if current node is the goal
            if (currentNode.Row.Equals(endNode.Row) && currentNode.Col.Equals(endNode.Col) && currentNode.DirectionSteps >= 4)
            {
                return ReconstructPath(currentNode, cameFrom);
            }

            //Explore neighbours
            foreach (var dir in PossibleDirections2(currentNode))
            {
                (int newRow, int newCol) = dir switch
                {
                    Direction.Up => (currentNode.Row - 1, currentNode.Col),
                    Direction.Right => (currentNode.Row, currentNode.Col + 1),
                    Direction.Down => (currentNode.Row + 1, currentNode.Col),
                    _ => (currentNode.Row, currentNode.Col - 1)
                };

                //Make sure we're within bounds
                if (WithinBounds(newRow, rows, newCol, cols))
                {
                    AStarNode neighbour = new(newRow, newCol)
                    {
                        Direction = dir,
                        DirectionSteps = dir == currentNode.Direction ? currentNode.DirectionSteps + 1 : 1
                    };

                    //Apply tentative G cost
                    int tentativeG = gScore[currentNode] + grid[newRow, newCol];

                    //Check if neighbour already exists in open queue or has lower G cost in the dictionary
                    if (!gScore.TryGetValue(neighbour, out int neighbourG) || tentativeG < neighbourG)
                    {
                        cameFrom[neighbour] = currentNode;
                        gScore[neighbour] = tentativeG;
                        fScore[neighbour] = tentativeG + Heuristic(neighbour, endNode);

                        if (!openQueue.UnorderedItems.Any(item => item.Element.Equals(neighbour)))
                        {
                            openQueue.Enqueue(neighbour, fScore[neighbour]);
                        }
                    }
                }
            }
        }

        return [];
    }

    public List<AStarNode> OptimalPath2Parallel()
    {
        AStarNode startNodeDown = new(0, 0) { Direction = Direction.Down };
        AStarNode startNodeRight = new(0, 0) { Direction = Direction.Right };
        AStarNode endNode = new(rows - 1, cols - 1);
        ConcurrentBag<(AStarNode bagNode, int heatLost)> openBag = [(startNodeDown, 0), (startNodeRight, 0)];

        ConcurrentDictionary<AStarNode, AStarNode> cameFrom = [];
        ConcurrentDictionary<AStarNode, int> gScore = new()
        {
            [startNodeDown] = 0,
            [startNodeRight] = 0
        };

        ConcurrentDictionary<AStarNode, int> fScore = new() { [endNode] = Heuristic(startNodeDown, endNode) }; // Accumulated heat loss + manhattan distance (heuristic)

        List<AStarNode> optimalPath = [];

        // while (openBag.TryTake(out (AStarNode tupleNode, int currentF) currentTuple))
        while (openBag.Count is not 0)
        {
            List<AStarNode> crucibles = openBag.OrderBy(element => element.heatLost).Select(element => element.bagNode).ToList();
            openBag.Clear();

            Parallel.ForEach(crucibles, currentNode =>
            {
                //Check if current node is the goal
                if (currentNode.Row.Equals(endNode.Row) && currentNode.Col.Equals(endNode.Col) && currentNode.DirectionSteps >= 4)
                {
                    optimalPath = ReconstructPathParallel(currentNode, cameFrom);
                    openBag.Clear();
                    return;
                }

                //Explore neighbours
                foreach (var dir in PossibleDirections2(currentNode))
                {
                    (int newRow, int newCol) = dir switch
                    {
                        Direction.Up => (currentNode.Row - 1, currentNode.Col),
                        Direction.Right => (currentNode.Row, currentNode.Col + 1),
                        Direction.Down => (currentNode.Row + 1, currentNode.Col),
                        _ => (currentNode.Row, currentNode.Col - 1)
                    };

                    //Make sure we're within bounds
                    if (WithinBounds(newRow, rows, newCol, cols))
                    {
                        AStarNode neighbour = new(newRow, newCol)
                        {
                            Direction = dir,
                            DirectionSteps = dir == currentNode.Direction ? currentNode.DirectionSteps + 1 : 1
                        };

                        //Apply tentative G cost
                        int tentativeG = gScore[currentNode] + grid[newRow, newCol];

                        //Check if neighbour already exists in open queue or has lower G cost in the dictionary
                        if (!gScore.TryGetValue(neighbour, out int neighbourG) || tentativeG < neighbourG)
                        {
                            if (!gScore.TryAdd(neighbour, tentativeG))
                            {
                                gScore.TryUpdate(neighbour, tentativeG, neighbourG);
                            }

                            if (!cameFrom.TryGetValue(neighbour, out AStarNode storedParent))
                            {
                                cameFrom.TryAdd(neighbour, currentNode);
                            }
                            else cameFrom.TryUpdate(neighbour, currentNode, storedParent);

                            if (!fScore.TryGetValue(neighbour, out int storedF))
                            {
                                fScore.TryAdd(neighbour, tentativeG + Heuristic(neighbour, endNode));
                            }
                            else fScore.TryUpdate(neighbour, tentativeG + Heuristic(neighbour, endNode), storedF);

                            if (!openBag.Any(element => element.Item1.Equals(neighbour)))
                            {
                                openBag.Add((neighbour, fScore[neighbour]));
                            }
                        }
                    }
                }
            });
        }

        return optimalPath;
    }

    private List<Direction> PossibleDirections(AStarNode currentNode)
    {
        int currentDirection = (int)currentNode.Direction;

        if (currentNode.DirectionSteps is not 3)
        {
            return [(Direction)((currentDirection - 1 + 4) % 4), (Direction)((currentDirection + 1) % 4), (Direction)currentDirection];
        }
        else return [(Direction)((currentDirection - 1 + 4) % 4), (Direction)((currentDirection + 1) % 4)];
    }

    private List<Direction> PossibleDirections2(AStarNode currentNode)
    {
        int currentDirection = (int)currentNode.Direction;

        return currentNode.DirectionSteps switch
        {
            < 4 => [(Direction)currentDirection],
            >= 4 and < 10 => [(Direction)((currentDirection - 1 + 4) % 4), (Direction)((currentDirection + 1) % 4), (Direction)currentDirection],
            10 => [(Direction)((currentDirection - 1 + 4) % 4), (Direction)((currentDirection + 1) % 4)],
            _ => []
        };
    }

    private static bool WithinBounds(int row, int rows, int col, int cols) => row >= 0 && row < rows &&
                                                                              col >= 0 && col < cols;

    private static int Heuristic(AStarNode node, AStarNode endNode) => Math.Abs(node.Row - endNode.Row) +
                                                                        Math.Abs(node.Col - endNode.Col);

    private static List<AStarNode> ReconstructPath(AStarNode endNode, Dictionary<AStarNode, AStarNode> parentNodes)
    {
        List<AStarNode> path = [];
        AStarNode currentNode = endNode;
        path.Insert(0, (currentNode));
        while (parentNodes.TryGetValue(currentNode, out AStarNode parent))
        {
            currentNode = parent;
            path.Insert(0, (currentNode));
        }

        return path;
    }

    private static List<AStarNode> ReconstructPathParallel(AStarNode endNode, ConcurrentDictionary<AStarNode, AStarNode> parentNodes)
    {
        List<AStarNode> path = [];
        AStarNode currentNode = endNode;
        path.Insert(0, (currentNode));
        while (parentNodes.TryGetValue(currentNode, out AStarNode parent))
        {
            currentNode = parent;
            path.Insert(0, (currentNode));
        }

        return path;
    }

}

