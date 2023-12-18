using Day10;

// string filePath = @"..\Day10\example1.txt";
// string filePath = @"..\Day10\example2.txt";
// string filePath = @"..\Day10\example3.txt";
// string filePath = @"..\Day10\example4.txt";
string filePath = @"..\Day10\input.txt";
ReadOnlySpan<string> input = File.ReadAllLines(filePath);

string[,] pipeGrid1 = GridMethods.PopulateStringGrid(input);
string[,] pipeGrid2 = GridMethods.PopulateOriginalGrid(input);

SPipe sPipe = new("╬", ["╚", "╔", "═"], ["╔", "║", "╗"], ["═", "╝", "╗"], ["╚", "║", "╝"], [(0, -1), (-1, 0), (0, 1), (1, 0)]);
SPipe sPipe2 = new("S", ["L", "F", "-"], ["F", "|", "7"], ["-", "J", "7"], ["L", "|", "J"], [(0, -1), (-1, 0), (0, 1), (1, 0)]);

List<Pipe> pipes =
[
    new("╔", ["╚", "╬", "║", "╝"], ["╝", "╗", "═", "╬"], [(1,0), (0,1)]), //F-pipe
    new("╚", ["╔", "╬", "║", "╗"], ["═", "╬", "╝", "╗"], [(-1,0), (0,1)]), //L-pipe
    new("╗", ["╚", "╬", "║", "╝"], ["╚", "╔", "═", "╬"], [(1,0), (0,-1)]), //7-pipe
    new("╝", ["╔", "╬", "║", "╗"], ["╚", "╔", "═", "╬"], [(-1,0), (0,-1)]), //J-pipe
    new("═", ["╚", "╔", "═", "╬"], ["═", "╬", "╝", "╗"], [(0,-1), (0,1)]), //horizontal pipe
    new("║", ["╔", "╬", "║", "╗"], ["╚", "╬", "║", "╝"], [(-1,0), (1,0)]) //vertical pipe
];

List<Pipe> originalPipes =
[
    new("F", ["L", "|", "J"], ["J", "7", "-"], [(1,0), (0,1)]), //F-pipe
    new("L", ["F", "|", "7"], ["-", "J", "7"], [(-1,0), (0,1)]), //L-pipe
    new("7", ["L", "|", "J"], ["L", "F", "-"], [(1,0), (0,-1)]), //7-pipe
    new("J", ["F", "|", "7"], ["L", "F", "-"], [(-1,0), (0,-1)]), //J-pipe
    new("-", ["L", "F", "-"], ["-", "J", "7"], [(0,-1), (0,1)]), //horizontal pipe
    new("|", ["F", "|", "7"], ["L", "|", "J"], [(-1,0), (1,0)]) //vertical pipe
];

int resultOne = PartOne(pipeGrid1, sPipe, pipes);
Console.WriteLine($"Furthest position in part one was {resultOne} steps away");

int resultTwo = PartTwo(pipeGrid2, sPipe2, originalPipes);
Console.WriteLine($"There were {resultTwo} tiles within the loop in part two.");

int PartOne(string[,] pipeGrid, SPipe sPipe, List<Pipe> pipes)
{
    int maxRow = pipeGrid.GetLength(0);
    int maxCol = pipeGrid.GetLength(1);
    (int sRow, int sCol) = GridMethods.FindSPipe(pipeGrid);
    (int row, int col)[] possibleMoves = SPipe.PossibleCoordinates(sPipe, (sRow, sCol));
    Queue<(int qRow, int qCol)> pipeNodes = StartingNodes(sPipe, pipeGrid, possibleMoves);

    int furthestPosition = 0;
    pipeGrid[sRow, sCol] = "0";
    while (pipeNodes.Count > 0)
    {
        (int cRow, int cCol) = pipeNodes.Dequeue();

        if (int.TryParse(pipeGrid[cRow, cCol], out _))
            continue;

        Pipe currentPipe = pipes.First(p => p.Symbol == pipeGrid[cRow, cCol]);
        (int rowA, int colA, int rowB, int colB) = Pipe.PossibleCoordinates(currentPipe, (cRow, cCol));

        bool isValidA = rowA >= 0 && rowA < maxRow && colA >= 0 && colA < maxCol;
        bool isValidB = rowB >= 0 && rowB < maxRow && colB >= 0 && colB < maxCol;

        int prevValue = 0;
        if (isValidA && isValidB)
        {
            if (int.TryParse(pipeGrid[rowA, colA], out prevValue))
            {
                pipeGrid[cRow, cCol] = (prevValue + 1).ToString();

                if (currentPipe.MoveB.Contains(pipeGrid[rowB, colB]))
                    pipeNodes.Enqueue((rowB, colB));
            }
            else
            {
                prevValue = int.Parse(pipeGrid[rowB, colB]);
                pipeGrid[cRow, cCol] = (prevValue + 1).ToString();

                if (currentPipe.MoveA.Contains(pipeGrid[rowA, colA]))
                    pipeNodes.Enqueue((rowA, colA));
            }
        }
        else if (!isValidA)
        {
            prevValue = int.Parse(pipeGrid[rowB, colB]);
            pipeGrid[cRow, cCol] = (prevValue + 1).ToString();
        }
        else
        {
            prevValue = int.Parse(pipeGrid[rowB, colB]);
            pipeGrid[cRow, cCol] = (prevValue + 1).ToString();
        }

        furthestPosition = Math.Max(furthestPosition, prevValue + 1);
    }

    return furthestPosition;
}

int PartTwo(string[,] pipeGrid, SPipe sPipe, List<Pipe> pipes)
{
    int maxRow = pipeGrid.GetLength(0);
    int maxCol = pipeGrid.GetLength(1);
    (int sRow, int sCol) = GridMethods.FindOriginalSPipe(pipeGrid);
    (int row, int col)[] possibleMoves = SPipe.PossibleCoordinates(sPipe, (sRow, sCol));
    Queue<(int qRow, int qCol)> pipeNodes = StartingNodes(sPipe, pipeGrid, possibleMoves);

    string[] symbols = ["╚", "╔", "═", "╬", "║", "╝", "╗"];
    string[] originals = ["L", "F", "-", "S", "|", "J", "7"];

    pipeGrid[sRow, sCol] = "╬";
    while (pipeNodes.Count > 0)
    {
        (int cRow, int cCol) = pipeNodes.Dequeue();

        if (symbols.Contains(pipeGrid[cRow, cCol]))
            continue;

        Pipe currentPipe = pipes.First(p => p.Symbol == pipeGrid[cRow, cCol]);
        (int rowA, int colA, int rowB, int colB) = Pipe.PossibleCoordinates(currentPipe, (cRow, cCol));

        bool isValidA = rowA >= 0 && rowA < maxRow && colA >= 0 && colA < maxCol;
        bool isValidB = rowB >= 0 && rowB < maxRow && colB >= 0 && colB < maxCol;

        if (isValidA && isValidB)
        {
            if (currentPipe.MoveB.Contains(pipeGrid[rowB, colB]))
            {
                pipeNodes.Enqueue((rowB, colB));
            }
            else pipeNodes.Enqueue((rowA, colA));

            for (int i = 0; i < 7; i++)
            {
                if (pipeGrid[cRow, cCol] == originals[i])
                    pipeGrid[cRow, cCol] = symbols[i];
            }
        }
    }

    GridMethods.CleanGrid(pipeGrid);

    int gridRows = pipeGrid.GetLength(0);
    int gridCols = pipeGrid.GetLength(1);

    int insideLoop = 0;
    for (int row = 1; row < gridRows - 1; row++)
    {
        for (int col = 1; col < gridCols - 1; col++)
        {
            if (!symbols.Contains(pipeGrid[row, col]))
            {
                if (GridMethods.InsideLoop(pipeGrid, row, col))
                {
                    pipeGrid[row, col] = "X";
                    insideLoop++;
                }
            }
        }
    }
    // GridMethods.DrawStringGrid(pipeGrid);

    return insideLoop;
}

Queue<(int, int)> StartingNodes(SPipe sPipe, string[,] pipeGrid, (int row, int col)[] possibleMoves)
{
    int maxRow = pipeGrid.GetLength(0);
    int maxCol = pipeGrid.GetLength(1);
    Queue<(int, int)> pipeNodes = [];

    if (possibleMoves[0].row >= 0 && possibleMoves[0].row < maxRow && possibleMoves[0].col >= 0 && possibleMoves[0].col < maxCol)
    {
        if (sPipe.MoveLeft.Contains(pipeGrid[possibleMoves[0].row, possibleMoves[0].col]))
            pipeNodes.Enqueue((possibleMoves[0].row, possibleMoves[0].col));
    }

    if (possibleMoves[1].row >= 0 && possibleMoves[1].row < maxRow && possibleMoves[1].col >= 0 && possibleMoves[1].col < maxCol)
    {
        if (sPipe.MoveUp.Contains(pipeGrid[possibleMoves[1].row, possibleMoves[1].col]))
            pipeNodes.Enqueue((possibleMoves[1].row, possibleMoves[1].col));
    }

    if (possibleMoves[2].row >= 0 && possibleMoves[2].row < maxRow && possibleMoves[2].col >= 0 && possibleMoves[2].col < maxCol)
    {
        if (sPipe.MoveRight.Contains(pipeGrid[possibleMoves[2].row, possibleMoves[2].col]))
            pipeNodes.Enqueue((possibleMoves[2].row, possibleMoves[2].col));
    }

    if (possibleMoves[3].row >= 0 && possibleMoves[3].row < maxRow && possibleMoves[3].col >= 0 && possibleMoves[3].col < maxCol)
    {
        if (sPipe.MoveDown.Contains(pipeGrid[possibleMoves[3].row, possibleMoves[3].col]))
            pipeNodes.Enqueue((possibleMoves[3].row, possibleMoves[3].col));
    }

    return pipeNodes;
}

record Pipe(string Symbol, string[] MoveA, string[] MoveB, List<(int, int)> Moves)
{
    public static (int, int, int, int) PossibleCoordinates(Pipe pipe, (int, int) currentCoordinate)
    {
        return
            (currentCoordinate.Item1 + pipe.Moves[0].Item1, currentCoordinate.Item2 + pipe.Moves[0].Item2,
            currentCoordinate.Item1 + pipe.Moves[1].Item1, currentCoordinate.Item2 + pipe.Moves[1].Item2);
    }
}

record SPipe(string Symbol, string[] MoveLeft, string[] MoveUp, string[] MoveRight, string[] MoveDown, List<(int, int)> Moves)
{
    public static (int, int)[] PossibleCoordinates(SPipe sPipe, (int, int) currentCoordinate)
    {
        return
        [
            (currentCoordinate.Item1 + sPipe.Moves[0].Item1, currentCoordinate.Item2 + sPipe.Moves[0].Item2),
            (currentCoordinate.Item1 + sPipe.Moves[1].Item1, currentCoordinate.Item2 + sPipe.Moves[1].Item2),
            (currentCoordinate.Item1 + sPipe.Moves[2].Item1, currentCoordinate.Item2 + sPipe.Moves[2].Item2),
            (currentCoordinate.Item1 + sPipe.Moves[3].Item1, currentCoordinate.Item2 + sPipe.Moves[3].Item2)
        ];
    }
}