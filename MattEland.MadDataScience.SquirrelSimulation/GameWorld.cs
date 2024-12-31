using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation;

public class GameWorld(int width, int height, int maxTurns, ILogger logger)
{
    private readonly List<GameObject> _objects = new();

    public IEnumerable<GameObject> Objects => _objects;
    public int Width => width;
    public int Height => height;

    public int TurnsLeft { get; private set; } = maxTurns;
    public GameStatus State { get; private set; } = GameStatus.InProgress;
    
    public int MaxTurns { get; } = maxTurns;

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


    public void HandleSquirrelMove(Squirrel squirrel, WorldPosition newPosition)
    {
        if (!IsValidPosition(newPosition))
        {
            logger.LogWarning("Invalid move attempted");
        }
        else
        {
            // TODO: Handle collisions
            GameObject? otherObject = Objects.FirstOrDefault(o => o.Position == newPosition);
            if (otherObject is Acorn a)
            {
                logger.LogDebug("Squirrel found an acorn at {Pos}", newPosition);
                squirrel.Position = newPosition;
                squirrel.HasAcorn = true;

                // Remove the acorn from the world
                _objects.Remove(a);
            }
            else if (otherObject is Tree && squirrel.HasAcorn)
            {
                logger.LogInformation("Game ended: Squirrel reached the tree with an acorn");
                squirrel.Position = newPosition;
                State = GameStatus.Won;
            }
            else if (otherObject is Doggo)
            {
                logger.LogInformation("Game ended: Squirrel ran into the doggo");
                squirrel.Position = newPosition;
                State = GameStatus.Killed;
            }
            else if (otherObject is null)
            {
                logger.LogDebug("Moving squirrel from {Old} to {New}", squirrel.Position, newPosition);
                squirrel.Position = newPosition;
            }
            else
            {
                logger.LogDebug("Collision detected at {Pos} with {Other}", newPosition, otherObject.Name);
            }
        }

        // TODO: Simulate other actors
        if (State == GameStatus.InProgress)
        {
            TurnsLeft -= 1;
            if (TurnsLeft <= 0)
            {
                logger.LogInformation("Game ended: Out of turns");
                State = GameStatus.OutOfTime;
            }
        }
    }
}