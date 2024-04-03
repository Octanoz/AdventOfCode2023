using Day10;

///May refactor this using the CustomCoord class that I didn't have implemented at the time
///That will likely make this a lot cleaner but also require the whole thing to be re-written nearly from scratch
///There are a lot of possible moves and out of bounds calculations that can probably be avoided

Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day10\example1.txt",
    ["example2"] = @"..\Day10\example2.txt",
    ["example3"] = @"..\Day10\example3.txt",
    ["example4"] = @"..\Day10\example4.txt",
    ["challenge"] = @"..\Day10\input.txt"
};

ReadOnlySpan<string> input = File.ReadAllLines(filePaths["challenge"]);

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
            if (!symbols.Contains(pipeGrid[row, col]) && GridMethods.InsideLoop(pipeGrid, row, col))
            {
                pipeGrid[row, col] = "X";
                insideLoop++;
            }
        }
    }

    return insideLoop;
}

Queue<(int, int)> StartingNodes(SPipe sPipe, string[,] pipeGrid, (int row, int col)[] possibleMoves)
{
    int maxRow = pipeGrid.GetLength(0);
    int maxCol = pipeGrid.GetLength(1);
    Queue<(int, int)> pipeNodes = [];

    for (int i = 0; i < possibleMoves.Length; i++)
    {
        if (possibleMoves[i].row < 0 && possibleMoves[i].row >= maxRow &&
            possibleMoves[i].col < 0 && possibleMoves[i].col >= maxCol)
        {
            continue;
        }

        switch (i)
        {
            case 0 when sPipe.MoveLeft.Contains(pipeGrid[possibleMoves[i].row, possibleMoves[i].col]):
            case 1 when sPipe.MoveUp.Contains(pipeGrid[possibleMoves[i].row, possibleMoves[i].col]):
            case 2 when sPipe.MoveRight.Contains(pipeGrid[possibleMoves[i].row, possibleMoves[i].col]):
            case 3 when sPipe.MoveDown.Contains(pipeGrid[possibleMoves[i].row, possibleMoves[i].col]):
                pipeNodes.Enqueue((possibleMoves[i].row, possibleMoves[i].col));
                break;
        }
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