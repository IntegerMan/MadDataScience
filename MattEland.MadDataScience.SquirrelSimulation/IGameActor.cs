namespace MattEland.MadDataScience.SquirrelSimulation;

public interface IGameActor
{
    public WorldPosition Position { get; set; }
    public string Name { get; }
    public int TurnOrder { get; }
    
    public WorldPosition GetGameMove(IEnumerable<TilePerceptions> choices, Random random);
    public void HandleCollision(GameObject otherObject, GameWorld world);
}