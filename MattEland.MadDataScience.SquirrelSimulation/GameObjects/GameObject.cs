using System.Diagnostics.CodeAnalysis;

namespace MattEland.MadDataScience.SquirrelSimulation.GameObjects;

public abstract class GameObject
{
    private WorldPosition _position;

    public required WorldPosition Position
    {
        get => _position;
        [MemberNotNull(nameof(_position))]
        set
        {
            _position = value;
            OnPositionChanged(value);
        }
    }

    public Queue<WorldPosition> PastPositions { get; } = new(3);

    protected void OnPositionChanged(WorldPosition newPos)
    {
        PastPositions.Enqueue(newPos);
        if (PastPositions.Count > 3)
        {
            PastPositions.Dequeue();
        }
    }

    public abstract string Name { get; }
    public abstract bool Blocks(IGameActor actor);
}