namespace MattEland.MadDataScience.SquirrelSimulation;

public class Acorn : GameObject
{
    public override string Name => "Acorn";
    public override bool Blocks(IGameActor actor) => actor is not Squirrel;
}