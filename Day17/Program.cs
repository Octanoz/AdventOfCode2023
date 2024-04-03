// #define VISUALIZATION

using AdventUtilities;
using StandardDirections;
using AStar;

Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day17\example1.txt",
    ["example2"] = @"..\Day17\example2.txt",
    ["challenge"] = @"..\Day17\input.txt"
};

Span<string> input = File.ReadAllLines(filePaths["example1"]);

//TLDR: The parallelism approach is badly implemented

///<summary>
///Method using parallelism can't use PriorityQueue since it is not thread-safe afaik.
///because there is no elimination of any possible points, memory just gets flooded quicker due to the parallelism.
///Perhaps sending a single tracer out from start location to end location before going parallel and then 
///eliminating any crucible going over those initial values can improve this method
///or some way to keep a parallel loop going while adding and yield returning from a priorityqueue...
///</summary>
Console.WriteLine($"Optimal heat loss: {SolutionAStar(input, 1)}"); // 1 = part 1, 2 = part 2, 3 = part 2 with parallelism

int SolutionAStar(Span<string> input, int solution)
{
    int[,] grid = GridExtensions.New2DGridWithDimensions<int>(input, out int rows, out int cols);
    AStarPathfinder pathfinder = new(grid);
    List<AStarNode> optimalPath = solution == 1 ? pathfinder.OptimalPath() : solution == 2 ? pathfinder.OptimalPath2() : pathfinder.OptimalPath2Parallel();


    if (optimalPath is not null)
    {
#if VISUALIZATION

        Dictionary<(int, int), Direction> coordinateDirections = optimalPath.Select(node => (node.Row, node.Col, node.Direction))
                                                                            .ToDictionary(tuple => (tuple.Row, tuple.Col), tuple => tuple.Direction);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if ((i, j) is not (0, 0) && coordinateDirections.TryGetValue((i, j), out Direction direction))
                {
                    char arrow = direction switch
                    {
                        Direction.Up => '^',
                        Direction.Right => '>',
                        Direction.Down => 'v',
                        Direction.Left => '<',
                        _ => '?'
                    };

                    Thread.Sleep(100);
                    Console.Write($"{arrow} ");
                }

                else Console.Write($"  ");
            }

            Console.WriteLine();
        }
        Console.ResetColor();

        Console.WriteLine($"\nThe optimal path is {optimalPath.Count} steps long.\n");

#endif

        return optimalPath.Select(node => grid[node.Row, node.Col]).Skip(1).Sum();
    }

    return -1;
}

