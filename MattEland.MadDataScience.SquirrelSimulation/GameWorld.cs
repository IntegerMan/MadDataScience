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

        // Check collisions. We need to use ToList() here to create a copy of the collection since gorillas can kill
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
                // Skip the center tile - we want to encourage movement. Actors will remain stationary when out of options
                if (x == 0 && y == 0) continue; 
                
                WorldPosition newPosition = actor.Position.Move(x, y);
                if (!IsValidPosition(newPosition)) continue;
                    
                if (IsBlocked(actor, newPosition)) continue;

                TilePerceptions tilePerceptions = BuildTilePerceptions(actor, newPosition);

                // Squirrels should not smell acorns if they have one - this helps with multi-agent simulations since squirrels can only carry 1 acorn
                if (actor is Squirrel { HasAcorn: true })
                {
                    tilePerceptions.SmellOfAcorn = 0;
                }
                
                perceptions.Add(tilePerceptions);
            }
        }

        return perceptions;
    }

    private TilePerceptions BuildTilePerceptions(IGameActor actor, WorldPosition newPosition) 
        => new()
        {
            Position = newPosition,
            SmellOfGorilla = CalculateTileSmell(actor, newPosition, typeof(Gorilla)),
            SmellOfAcorn = CalculateTileSmell(actor, newPosition, typeof(Acorn)),
            SmellOfSquirrel = CalculateTileSmell(actor, newPosition, typeof(Squirrel)),
            SmellOfTree = CalculateTileSmell(actor, newPosition, typeof(Tree)),
            SmellOfRabbit = CalculateTileSmell(actor, newPosition, typeof(Rabbit))
        };

    private float CalculateTileSmell(IGameActor? self, WorldPosition pos, Type type)
    {
        float smell = 0;
        foreach (var obj in Objects.Where(o => o.GetType() == type && o != self))
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

    public IEnumerable<TileVisualization> GetTileVisualizations(VisualizationKind visualization, SmellWeights? weights = null, IGameActor? actor = null)
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
                        VisualizationKind.Acorn => CalculateTileSmell(actor, pos, typeof(Acorn)),
                        VisualizationKind.Squirrel => CalculateTileSmell(actor, pos, typeof(Squirrel)),
                        VisualizationKind.Rabbit => CalculateTileSmell(actor, pos, typeof(Rabbit)),
                        VisualizationKind.Gorilla => CalculateTileSmell(actor, pos, typeof(Gorilla)),
                        VisualizationKind.Tree => CalculateTileSmell(actor, pos, typeof(Tree)),
                        VisualizationKind.Attractiveness => weights is not null 
                            ? CalculateTileAttractiveness(BuildTilePerceptions(actor, pos), weights) 
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
        (choice.SmellOfGorilla * weights.Gorilla) +
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