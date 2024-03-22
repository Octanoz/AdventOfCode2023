using AdventUtilities;
using StandardDirections;
using AStar;

Dictionary<string, string> inputData = new()
{
    ["example1"] = @"..\Day17\example1.txt",
    ["example2"] = @"..\Day17\example2.txt",
    ["challenge"] = @"..\Day17\input.txt"
};

Span<string> input = File.ReadAllLines(inputData["challenge"]);
// string[] input = File.ReadAllLines(filePath);

Console.WriteLine($"Optimal heat loss using ultra crucible and parallelism: {SolutionAStar(input, 1)}"); // 1 = part 1, 2 = part 2, 3 = part 2 with parallelism (still slow)

int SolutionAStar(Span<string> input, int solution)
{
    int[,] grid = GridExtensions.New2DGridWithDimensions<int>(input, out int rows, out int cols);
    AStarPathfinder pathfinder = new(grid);
    List<AStarNode> optimalPath = solution == 1 ? pathfinder.OptimalPath() : solution == 2 ? pathfinder.OptimalPath2() : pathfinder.OptimalPath2Parallel(); ;


    if (optimalPath is not null)
    {
        #region visualization

        Dictionary<(int, int), Direction> coordinateDirections = optimalPath.Select(node => (node.Row, node.Col, node.Direction))
                                                                            .ToDictionary(tuple => (tuple.Row, tuple.Col), tuple => tuple.Direction);

        Console.WriteLine();
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

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Thread.Sleep(100);
                    Console.Write($"{arrow} ");
                    Console.ResetColor();
                }
                else Console.Write($"  ");
                // else Console.Write($"{grid[i, j]} ");

            }

            Console.WriteLine();
        }

        Console.WriteLine($"\nThe optimal path is {optimalPath.Count} steps long.\n");

        #endregion

        return optimalPath.Select(node => grid[node.Row, node.Col]).Skip(1).Sum();
    }

    return -1;
}

