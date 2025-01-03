namespace MattEland.MadDataScience.SquirrelSimulation;

public record WorldPosition(int X, int Y)
{
    public override string ToString() => $"{{{X},{Y}}}";

    public WorldPosition Move(GameDirection direction) 
        => direction switch
        {
            GameDirection.Up => new WorldPosition(X, Y - 1),
            GameDirection.Down => new WorldPosition(X, Y + 1),
            GameDirection.Left => new WorldPosition(X - 1, Y),
            GameDirection.Right => new WorldPosition(X + 1, Y),
            GameDirection.UpLeft => new WorldPosition(X - 1, Y - 1),
            GameDirection.UpRight => new WorldPosition(X + 1, Y - 1),
            GameDirection.DownLeft => new WorldPosition(X - 1, Y + 1),
            GameDirection.DownRight => new WorldPosition(X + 1, Y + 1),
            _ => this
        };
    
    public WorldPosition Move(int x, int y) => new(X + x, Y + y);

    public float DistanceTo(WorldPosition other)
    {
        int xDiff = X - other.X;
        int yDiff = Y - other.Y;
        return MathF.Sqrt((xDiff * xDiff) + (yDiff * yDiff)); // Pythagorean theorem
    }
}