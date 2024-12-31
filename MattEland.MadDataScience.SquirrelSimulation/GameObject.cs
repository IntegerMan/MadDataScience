namespace MattEland.MadDataScience.SquirrelSimulation;

public abstract class GameObject
{
    public WorldPosition Position { get; set; }
    public abstract string Name { get; }
}