namespace MattEland.MadDataScience.SquirrelSimulation.GameObjects;

public class Tree : GameObject
{
    public override string Name => "Tree";
    public override bool Blocks(IGameActor actor) 
        => actor is not Squirrel { HasAcorn: true};
}