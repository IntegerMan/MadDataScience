using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation;

public class GameWorld(int width, int height, int maxTurns)
{
    private readonly List<GameObject> _objects = new();

    public IEnumerable<GameObject> Objects => _objects;
    public int Width => width;
    public int Height => height;
    
    public ILogger? Logger { get; set; }

    public bool UseManualSquirrelControl { get; set; }
    
    public int TurnsLeft { get; private set; } = maxTurns;
    public GameStatus State { get; internal set; } = GameStatus.InProgress;
    
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
        if (State != GameStatus.InProgress)
        {
            Logger?.LogWarning("Game is not in progress");
            return;
        }
        
        if (!IsValidPosition(newPosition))
        {
            Logger?.LogWarning("Invalid move attempted");
        }
        else if (IsBlocked(squirrel, newPosition))
        {
            Logger?.LogWarning("Squirrel was blocked at {Pos}", newPosition);
        }
        else
        {
            Logger?.LogDebug("Moving squirrel from {Old} to {New}", squirrel.Position, newPosition);
            squirrel.Position = newPosition;
            
            // TODO: Handle collisions
            GameObject? otherObject = Objects.FirstOrDefault(o => o.Position == newPosition);
            if (otherObject is not null)
            {
                squirrel.HandleCollision(otherObject, this);
            }
            
            if (otherObject is Acorn a)
            {
                Logger?.LogDebug("Squirrel found an acorn at {Pos}", newPosition);
                squirrel.Position = newPosition;
                squirrel.HasAcorn = true;

                // Remove the acorn from the world
                _objects.Remove(a);
            }
            else if (otherObject is Tree && squirrel.HasAcorn)
            {
                Logger?.LogInformation("Game ended: Squirrel reached the tree with an acorn");
                State = GameStatus.Won;
            }
            else if (otherObject is null)
            {

                squirrel.Position = newPosition;
            }
        }

        // Simulate other actors
        SimulateGameTurn();
    }

    private bool IsBlocked(IGameActor actor, WorldPosition newPosition) 
        => Objects.Any(o => o != actor && o.Position == newPosition && o.Blocks(actor));

    private void SimulateGameTurn()
    {
        if (State != GameStatus.InProgress)
        {
            Logger?.LogWarning("Game is not in progress");
            return;
        }
        
        // Have all actors take their turns in sequence (Squirrel, Rabbit, Doggo)
        foreach (var actor in Objects.OfType<IGameActor>().OrderBy(o => o.TurnOrder))
        {
            List<TilePerceptions> perceptions = BuildTilePerceptions(actor);

            WorldPosition desiredPos = actor.GetGameMove(perceptions, Random);
            Logger?.LogDebug("{Actor} wants to move to {Pos}", actor.Name, desiredPos);
        }
        
        // Maintain game state
        TurnsLeft -= 1;
        if (!Objects.OfType<Squirrel>().Any())
        {
            Logger?.LogInformation("Game ended: Squirrel is gone");
            State = GameStatus.Killed;
        }
        else if (TurnsLeft <= 0)
        {
            Logger?.LogInformation("Game ended: Out of turns");
            State = GameStatus.OutOfTime;
        }
    }

    public Random Random { get; set; } = new();

    private List<TilePerceptions> BuildTilePerceptions(IGameActor actor)
    {
        List<TilePerceptions> perceptions = new();
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                WorldPosition newPosition = actor.Position.Move(x, y);
                if (!IsValidPosition(newPosition)) continue;
                    
                if (IsBlocked(actor, newPosition)) continue;
                    
                TilePerceptions perception = new()
                {
                    Position = newPosition,
                    SmellOfDoggo = CalculateTileSmell(newPosition, typeof(Doggo)),
                    SmellOfAcorn = CalculateTileSmell(newPosition, typeof(Acorn)),
                    SmellOfSquirrel = CalculateTileSmell(newPosition, typeof(Squirrel)),
                    SmellOfTree = CalculateTileSmell(newPosition, typeof(Tree)),
                    SmellOfRabbit = CalculateTileSmell(newPosition, typeof(Rabbit))
                };
                perceptions.Add(perception);
            }
        }

        return perceptions;
    }

    private float CalculateTileSmell(WorldPosition pos, Type type)
    {
        float smell = 0;
        foreach (var obj in Objects.Where(o => o.GetType() == type))
        {
            smell += 1 / (obj.Position.DistanceTo(pos) + 1);
        }

        return smell;
    }

    internal void Remove(GameObject obj)
    {
        _objects.Remove(obj);
    }
}