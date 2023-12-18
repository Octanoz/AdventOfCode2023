namespace Day10;

class GridMethods
{
    public static string[,] PopulateStringGrid(ReadOnlySpan<string> input)
    {
        string sReplace = "╬";
        string fReplace = "╔";
        string lReplace = "╚";
        string sevenReplace = "╗";
        string jReplace = "╝";
        string verticalPipe = "║";
        string horizontalPipe = "═";

        int rows = input.Length;
        int cols = input[0].Length;

        string[,] grid = new string[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j] = input[i][j] switch
                {
                    'S' => sReplace,
                    'F' => fReplace,
                    'L' => lReplace,
                    '7' => sevenReplace,
                    'J' => jReplace,
                    '|' => verticalPipe,
                    '-' => horizontalPipe,
                    _ => " "
                };
            }
        }

        return grid;
    }

    public static string[,] PopulateOriginalGrid(ReadOnlySpan<string> input)
    {
        int rows = input.Length;
        int cols = input[0].Length;

        string[,] grid = new string[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (input[i][j] == '.')
                {
                    grid[i, j] = " ";
                }
                else grid[i, j] = input[i][j].ToString();
            }
        }

        return grid;
    }

    public static void DrawStringGrid(string[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write(grid[i, j]);
            }
            Console.WriteLine();
        }
    }

    public static void CleanGrid(string[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        string[] symbols = ["╚", "╔", "═", "╬", "║", "╝", "╗"];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (!symbols.Contains(grid[i, j]))
                    grid[i, j] = " ";
            }
        }
    }

    public static (int, int) FindSPipe(string[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] == "╬")
                    return (i, j);
            }
        }

        throw new InvalidDataException("No S-pipe in this grid");
    }

    public static (int, int) FindOriginalSPipe(string[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] == "S")
                    return (i, j);
            }
        }

        throw new InvalidDataException("No S-pipe in this grid");
    }

    public static bool InsideLoop(string[,] grid, int currentRow, int currentCol)
    {
        int pipesLeft = 0;
        bool anyPipes = false;
        int maxRow = grid.GetLength(0);
        int maxCol = grid.GetLength(1);

        string[] symbols = ["╚", "╔", "═", "╬", "║", "╝", "╗"];

        for (int leftCol = 0; leftCol < currentCol; leftCol++)
        {
            if (grid[currentRow, leftCol] == "║" || grid[currentRow, leftCol] == "╚" || grid[currentRow, leftCol] == "╝")
                pipesLeft++;
        }

        for (int rightCol = currentCol + 1; rightCol < maxCol; rightCol++)
        {
            if (symbols.Contains(grid[currentRow, rightCol]))
            {
                anyPipes = true;
                break;
            }
        }

        if (anyPipes)
        {
            return pipesLeft % 2 != 0;
        }
        else return anyPipes;
    }
}