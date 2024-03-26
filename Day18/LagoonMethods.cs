using CoordXL;
using CustomCoord;
using StandardDirections;

namespace Lagoon;

public static class LagoonMethods
{
    #region Part1 Methods

    public static Coord FindFillerStart(char[,] grid, Coord minRow)
    {
        Queue<Coord> coordQueue = [];
        coordQueue.Enqueue(minRow);
        while (true)
        {
            Coord current = coordQueue.Dequeue();

            if (grid[current.Row, current.Col] is '.')
                return current;
            else
            {
                coordQueue.Enqueue(current.Right);
                coordQueue.Enqueue(current.Down);
            }
        }
    }

    public static void FillGrid(char[,] grid, Coord fillStart, int rows, int cols)
    {
        Queue<Coord> coordQueue = [];
        coordQueue.Enqueue(fillStart);
        while (coordQueue.Count is not 0)
        {
            Coord current = coordQueue.Dequeue();
            grid[current.Row, current.Col] = '#';

            var emptySpaces = current.Neighbours.Where(c => IsValidCoordinate(c, rows, cols) && grid[c.Row, c.Col] is '.');
            foreach (var emptySpace in emptySpaces)
            {
                if (!coordQueue.Contains(emptySpace))
                    coordQueue.Enqueue(emptySpace);
            }
        }
    }

    public static bool IsValidCoordinate(Coord coord, int rows, int cols) => coord.Row >= 0 && coord.Row < rows && coord.Col >= 0 && coord.Col < cols;

    public static int CalculateVolumeFilled(char[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        int volume = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] is '#')
                    volume++;
            }
        }

        return volume;
    }

    public static int CalculateVolume(List<Coord> inputCoordinates, int rows)
    {
        List<Coord> coordinates = inputCoordinates.OrderBy(c => c.Row).ThenBy(c => c.Col).ToList();

        bool insideLagoon = false;
        var (firstNeighbour, lastNeighbour) = (0, 0);
        Coord? prevCoord = null;

        int volume = 0;
        int rowNumber = 0;
        for (int i = 0; i < coordinates.Count; i++)
        {
            volume++;
            Coord coord = coordinates[i];

            //Top and bottom row coordinates don't need further check 
            //since the state before and after will always be the same
            if (coord.Row is 0 || coord.Row == rows - 1)
            {
                continue;
            }

            //Reset bool and previous coordinate when scanning a new row
            if (coord.Row != rowNumber)
            {
                rowNumber = coord.Row;
                insideLagoon = false;
                prevCoord = null;
            }
            else if (prevCoord is not null && insideLagoon)
            {
                volume += coord.Col - prevCoord.Col;
            }

            //If the first neighbour is not set and the current coordinate has a horizontal neighbour
            if (coordinates.Contains(coord.Right) && firstNeighbour is 0)
            {
                firstNeighbour = coordinates.Contains(coord.Up) ? -1 : 1;
                while (coordinates.Contains(coord.Right))
                {
                    coord = coordinates[++i];
                    volume++;
                }

                lastNeighbour = coordinates.Contains(coord.Up) ? -1 : 1;
                if (firstNeighbour != lastNeighbour)
                    insideLagoon = !insideLagoon;

                (firstNeighbour, lastNeighbour) = (0, 0);
                prevCoord = new(coord.Row, coord.Col + 1);
            }
            else if (!coordinates.Contains(coord.Right) && firstNeighbour is 0)
            {
                insideLagoon = !insideLagoon;
                prevCoord = new(coord.Row, coord.Col + 1);
            }
        }

        return volume;
    }

    #endregion

    #region Part2 Methods

    public static (CoordL startCoord, CoordL endCoord) CalculateRange(CoordL baseCoord, Direction direction, long steps, out CoordL newBaseCoord)
    {
        CoordL startCoord;
        CoordL endCoord;

        switch (direction)
        {
            case Direction.Up:
                startCoord = new(baseCoord.Row - 1, baseCoord.Col);
                endCoord = new(baseCoord.Row - steps + 1, baseCoord.Col);
                newBaseCoord = new(endCoord.Row - 1, endCoord.Col);
                break;
            case Direction.Right:
                startCoord = new(baseCoord.Row, baseCoord.Col);
                endCoord = new(baseCoord.Row, baseCoord.Col + steps);
                newBaseCoord = new(endCoord.Row, endCoord.Col);
                break;
            case Direction.Down:
                startCoord = new(baseCoord.Row + 1, baseCoord.Col);
                endCoord = new(baseCoord.Row + steps - 1, baseCoord.Col);
                newBaseCoord = new(endCoord.Row + 1, endCoord.Col);
                break;
            case Direction.Left:
                startCoord = new(baseCoord.Row, baseCoord.Col);
                endCoord = new(baseCoord.Row, baseCoord.Col - steps);
                newBaseCoord = new(endCoord.Row, endCoord.Col);
                break;
            default:
                throw new ArgumentException($"Invalid direction used in CalculateRange [{direction}]");
        }

        if (startCoord.Row < endCoord.Row || startCoord.Col < endCoord.Col)
            return (startCoord, endCoord);
        else return (endCoord, startCoord);
    }

    public static long CalculateVolume2(List<(CoordL, CoordL)> inputHorizontalCoordinates, List<(CoordL, CoordL)> verticalRanges)
    {
        // Sort coordinates by row to ensure we are scanning row by row
        List<(CoordL, CoordL)> horizontalRanges = inputHorizontalCoordinates.OrderBy(c => c.Item1.Row).ToList();
        long rows = horizontalRanges.Max(c => Math.Max(c.Item1.Row, c.Item2.Row)) + 1;

        var (insideLagoon, firstNeighbourUp, lastNeighbourUp) = (false, false, false);
        CoordL? prevCoord = null;
        long volume = 0;

        for (long i = 0; i < rows; i++)
        {
            //Limit ranges to current row to reduce calculations further down the method
            List<(CoordL, CoordL)> currentRowRanges = CalculateRowRanges(horizontalRanges, verticalRanges, i);

            insideLagoon = false;
            prevCoord = null;
            foreach (var (start, end) in currentRowRanges)
            {
                //Top and bottom row coordinates don't need further check, state before and after will always be the same
                if (start.Row is 0 || start.Row == rows - 1)
                {
                    if (start.Col != end.Col)
                    {
                        volume += end.Col - start.Col + 1;
                    }
                    else volume++;

                    continue;
                }

                if (prevCoord is not null && insideLagoon)
                {
                    volume += start.Col - prevCoord.Col;
                }

                //If the range consists of more than 1 coordinate
                if (start.Col != end.Col)
                {
                    volume += end.Col - start.Col + 1;

                    firstNeighbourUp = verticalRanges.Exists(c => c.Item1 == start.Up || c.Item2 == start.Up);
                    lastNeighbourUp = verticalRanges.Exists(c => c.Item1 == end.Up || c.Item2 == end.Up);

                    //Flip state if orientation of first neighbour is different from orientation of last neighbour
                    if (firstNeighbourUp != lastNeighbourUp)
                        insideLagoon = !insideLagoon;

                    //Reset bools
                    (firstNeighbourUp, lastNeighbourUp) = (false, false);
                }
                else
                {
                    volume++;
                    insideLagoon = !insideLagoon;
                }

                //One to the right so the previous coordinate is not counted twice
                prevCoord = new(end.Row, end.Col + 1);
            }
        }

        return volume;
    }

    private static List<(CoordL, CoordL)> CalculateRowRanges(List<(CoordL, CoordL)> horizontalRanges, List<(CoordL start, CoordL end)> verticalRanges, long row)
    {
        List<(CoordL, CoordL)> result = horizontalRanges.Where(range => range.Item1.Row == row).ToList();

        foreach (var (start, end) in verticalRanges)
        {
            if (start.Row <= row && end.Row >= row)
            {
                result.Add((new(row, start.Col), new(row, start.Col)));
            }
        }

        return result.OrderBy(c => c.Item2.Col).ToList();
    }

    public static List<(CoordL, CoordL)> CorrectCoordinates(List<(CoordL, CoordL)> rawRanges, CoordL correctiveCoord)
    {
        return rawRanges.Select(c => (new CoordL(c.Item1.Row + correctiveCoord.Row, c.Item1.Col + correctiveCoord.Col),
                                    new CoordL(c.Item2.Row + correctiveCoord.Row, c.Item2.Col + correctiveCoord.Col)))
                        .ToList();
    }

    #endregion

}