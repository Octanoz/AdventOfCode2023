namespace AdventUtilities;

public static class GridExtensions
{
    public static T[,] New2DGrid<T>(string[] input)
    {
        int rows = input.Length;
        int cols = input[0].Length;
        T[,] grid = new T[rows, cols];

        Func<char, T> convertFunc = GetCharConvertFunc<T>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j] = convertFunc(input[i][j]);
            }
        }

        return grid;
    }

    public static T[,] New2DGridWithDimensions<T>(string[] input, out int rows, out int cols)
    {
        rows = input.Length;
        cols = input[0].Length;
        T[,] grid = new T[rows, cols];

        Func<char, T> convertFunc = GetCharConvertFunc<T>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j] = convertFunc(input[i][j]);
            }
        }

        return grid;
    }

    public static T[,] New2DGrid<T>(Span<string> input)
    {
        int rows = input.Length;
        int cols = input[0].Length;
        T[,] grid = new T[rows, cols];

        Func<char, T> convertFunc = GetCharConvertFunc<T>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j] = convertFunc(input[i][j]);
            }
        }

        return grid;
    }

    public static T[,] New2DGridWithDimensions<T>(Span<string> input, out int rows, out int cols)
    {
        rows = input.Length;
        cols = input[0].Length;
        T[,] grid = new T[rows, cols];

        Func<char, T> convertFunc = GetCharConvertFunc<T>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j] = convertFunc(input[i][j]);
            }
        }

        return grid;
    }


    public static T[,] New2DGrid<T>(string[] input, int elementLength)
    {
        string[][] chunks = input.Select(s => s
                                        .Select(c => c)
                                        .Chunk(elementLength)
                                        .Select(cArray => new string(cArray))
                                        .ToArray())
                                    .ToArray();

        int rows = chunks.Length;
        int cols = chunks[0].Length;
        T[,] grid = new T[rows, cols];

        Func<string, T> convertFunc = GetStringConvertFunc<T>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j] = convertFunc(chunks[i][j]);
            }
        }

        return grid;
    }

    private static Func<char, T> GetCharConvertFunc<T>()
    {
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Char => (c) => (T)(object)c,
            TypeCode.Int32 => (c) => (T)(object)(c - '0'),
            TypeCode.Int64 => (c) => (T)(object)(long)(c - '0'),
            _ => throw new NotSupportedException($"Type {typeof(T)} is not supported.")
        };
    }

    private static Func<string, T> GetStringConvertFunc<T>()
    {
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (s) => (T)(object)int.Parse(s),
            TypeCode.Int64 => (s) => (T)(object)long.Parse(s),
            _ => throw new NotSupportedException($"Type {typeof(T)} is not supported.")
        };
    }

    public static char[][] JaggedCharArray(string[] input) => input.Select(s => s.ToCharArray()).ToArray();

    public static int[][] JaggedIntArray(string[] input, char divider) => input.Select(s => s
                                                                                    .Split(divider))
                                                                                .Select(sArray => sArray
                                                                                    .Select(int.Parse)
                                                                                    .ToArray())
                                                                                .ToArray();

    public static void Draw2DGrid<T>(T[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{grid[i, j]} ");
            }
            Console.WriteLine();
        }
    }

    public static void Draw2DGridTight<T>(T[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{grid[i, j]}");
            }
            Console.WriteLine();
        }
    }

    public static void DrawJaggedGrid<T>(T[][] grid)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{grid[i][j]} ");
            }
            Console.WriteLine();
        }
    }

    public static void DrawJaggedGridTight<T>(T[][] grid)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{grid[i][j]}");
            }
            Console.WriteLine();
        }
    }

}
