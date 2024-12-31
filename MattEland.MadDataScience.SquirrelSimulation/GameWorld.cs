namespace MattEland.MadDataScience.SquirrelSimulation;

public class GameWorld(int width, int height)
{
    private readonly List<GameObject> _objects = new();
    
    public IEnumerable<GameObject> Objects => _objects;
    public int Width => width;
    public int Height => height;

    public int TurnsLeft { get; private set; } = MaxTurns;
    public const int MaxTurns = 50;
    
    public void AddObject(GameObject gameObject)
    {
        // Ensure within bounds of the world
        if (!IsValidPosition(gameObject.Position))
        {
            throw new ArgumentOutOfRangeException(nameof(gameObject.Position), 
                $"Position {gameObject.Position} is outside the bounds of the world");
        }
        if (IsOccupied(gameObject.Position))
        {
            throw new InvalidOperationException($"Position {gameObject.Position} is already occupied");
        }
        
        _objects.Add(gameObject);
    }

    private bool IsOccupied(WorldPosition position) 
        => _objects.Any(o => o.Position == position);

    public bool IsValidPosition(WorldPosition pos) 
        => pos.X >= 0 && pos.X < width && 
           pos.Y >= 0 && pos.Y < height;
}