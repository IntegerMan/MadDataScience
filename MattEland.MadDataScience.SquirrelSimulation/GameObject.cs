namespace MattEland.MadDataScience.SquirrelSimulation;

public abstract class GameObject
{
    public required WorldPosition Position { get; set; }
    public abstract string Name { get; }
    public abstract bool Blocks(IGameActor actor);
}