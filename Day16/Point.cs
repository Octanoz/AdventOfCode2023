namespace Day16;

public struct Point(int Row, int Column, Direction direction)
{
    public int Row { get; } = Row;
    public int Column { get; } = Column;
    public Direction Direction { get; } = direction;
}
