using MattEland.MadDataScience.SquirrelSimulation.Brains;
using MattEland.MadDataScience.SquirrelSimulation.GameObjects;
using Microsoft.Extensions.Logging;

namespace MattEland.MadDataScience.SquirrelSimulation;

public class GameWorld(int width, int height, int maxTurns)
{
    private readonly List<GameObject> _objects = new();

    public IEnumerable<GameObject> Objects => _objects;
    public int Width => width;
    public int Height => height;
    
    public ILogger? Logger { get; set; }
    
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
    
    private bool HasObjectAtOrNear(WorldPosition position)
        => _objects.Any(o => o.Position.X >= position.X - 1 && o.Position.X <= position.X + 1 &&
                                        o.Position.Y >= position.Y - 1 && o.Position.Y <= position.Y + 1);

    public bool IsValidPosition(WorldPosition pos)
        => pos.X >= 0 && pos.X < width &&
           pos.Y >= 0 && pos.Y < height;

    private bool IsBlocked(IGameActor actor, WorldPosition newPosition) 
        => Objects.Any(o => o != actor && o.Position == newPosition && o.Blocks(actor));

    public void SimulateGameTurn()
    {
        // Ensure game is in progress
        if (State != GameStatus.InProgress)
        {
            Logger?.LogWarning("Game is not in progress");
            return;
        }
        
        // Have all actors take their turns in sequence (Squirrel, Rabbit, Doggo)
        foreach (var actor in Objects.OfType<IGameActor>()
                                     .Where(a => a.IsActive)
                                     .OrderBy(o => o.TurnOrder))
        {
            List<TilePerceptions> perceptions = BuildTilePerceptions(actor);

            WorldPosition desiredPos = actor.Brain.GetGameMove(actor, perceptions, Random);
 
            HandleActorMove(actor, desiredPos);
        }
        
        // Maintain game state
        TurnsLeft -= 1;
        if (!Objects.OfType<Squirrel>().Any(s => s.IsActive))
        {
            Logger?.LogInformation("Game ended: No active squirrels");
            State = Objects.OfType<Squirrel>().Any(s => s.IsInTree) 
                ? GameStatus.Won 
                : GameStatus.Killed;
            Result = CalculateResult();
        }
        else if (TurnsLeft <= 0)
        {
            Logger?.LogInformation("Game ended: Out of turns");
            State = Objects.OfType<Squirrel>().Any(s => s.IsInTree) 
                ? GameStatus.Won 
                : GameStatus.OutOfTime;
            Result = CalculateResult();
        }
    }

    private GameResult CalculateResult() => new()
        {
            TurnsLeft = TurnsLeft,
            AcornsOnBoard = Objects.OfType<Acorn>().Count(),
            SquirrelsOnBoard = Objects.OfType<Squirrel>().Count(),
            WinningSquirrels = Objects.OfType<Squirrel>().Count(s => s.IsInTree),
            RabbitsOnBoard = Objects.OfType<Rabbit>().Count(),
        };

    private void HandleActorMove(IGameActor actor, WorldPosition desiredPos)
    {
        // Evaluate what they want to do
        if (desiredPos == actor.Position)
        {
            Logger?.LogTrace("{Actor} is staying put", actor.Name);
            return;
        }
        Logger?.LogDebug("{Actor} wants to move to {Pos}", actor.Name, desiredPos);

        // Validate the move
        if (IsBlocked(actor, desiredPos))
        {
            Logger?.LogWarning("{Actor} was blocked trying to move to {Pos}", actor.Name, desiredPos);
            return;
        }
        if (!IsValidPosition(desiredPos))
        {
            Logger?.LogWarning("{Actor} tried to move to an invalid position {Pos}", actor.Name, desiredPos);
            return;
        }

        // Check collisions. We need to use ToList() here to create a copy of the collection since doggos can kill
        List<GameObject> collidedObjects = Objects.Where(o => o.Position == desiredPos).ToList();
        foreach (var obj in collidedObjects)
        {
            Logger?.LogDebug("{Actor} collided with {Obj}", actor.Name, obj.Name);
            actor.HandleCollision(obj, this);
        }
                    
        // Move the actor
        actor.Position = desiredPos;
        Logger?.LogDebug("{Actor} moved to {Pos}", actor.Name, desiredPos);
    }

    public Random Random { get; init; } = new();

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

                perceptions.Add(BuildTilePerceptions(newPosition));
            }
        }

        return perceptions;
    }

    private TilePerceptions BuildTilePerceptions(WorldPosition newPosition) 
        => new()
        {
            Position = newPosition,
            SmellOfDoggo = CalculateTileSmell(newPosition, typeof(Doggo)),
            SmellOfAcorn = CalculateTileSmell(newPosition, typeof(Acorn)),
            SmellOfSquirrel = CalculateTileSmell(newPosition, typeof(Squirrel)),
            SmellOfTree = CalculateTileSmell(newPosition, typeof(Tree)),
            SmellOfRabbit = CalculateTileSmell(newPosition, typeof(Rabbit))
        };

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

    public WorldPosition FindOpenPosition()
    {
        WorldPosition pos;
        do
        {
            int x = Random.Next(1, Width - 1);
            int y = Random.Next(1, Height - 1);
            pos = new WorldPosition(x, y);
        } 
        while (HasObjectAtOrNear(pos));

        return pos;
    }

    public IEnumerable<TileVisualization> GetTileVisualizations(VisualizationKind visualization, SmellWeights? weights = null)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                WorldPosition pos = new(x, y);
                yield return new TileVisualization
                {
                    Position = pos,
                    Value = visualization switch
                    {
                        VisualizationKind.Acorn => CalculateTileSmell(pos, typeof(Acorn)),
                        VisualizationKind.Squirrel => CalculateTileSmell(pos, typeof(Squirrel)),
                        VisualizationKind.Rabbit => CalculateTileSmell(pos, typeof(Rabbit)),
                        VisualizationKind.Doggo => CalculateTileSmell(pos, typeof(Doggo)),
                        VisualizationKind.Tree => CalculateTileSmell(pos, typeof(Tree)),
                        VisualizationKind.Attractiveness => weights is not null 
                            ? CalculateTileAttractiveness(BuildTilePerceptions(pos), weights) 
                            : 0f,
                        _ => 0f
                    }
                };
            }
        }
    }

    internal static float CalculateTileAttractiveness(TilePerceptions choice, SmellWeights weights) =>
        (choice.SmellOfSquirrel * weights.Squirrel) + 
        (choice.SmellOfRabbit * weights.Rabbit) +
        (choice.SmellOfDoggo * weights.Doggo) +
        (choice.SmellOfTree * weights.Tree) +
        (choice.SmellOfAcorn * weights.Acorn);

    public GameResult RunToCompletion()
    {
        while (State == GameStatus.InProgress)
        {
            SimulateGameTurn();
        }

        return Result!;
    }

    public GameResult? Result { get; private set; }
}