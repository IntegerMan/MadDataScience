using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.SquirrelSimulation.GameObjects;

public interface IGameActor
{
    public bool IsActive => true;
    public WorldPosition Position { get; set; }
    public string Name { get; }
    public int TurnOrder { get; }
    public IBrain Brain { get; }
    public void HandleCollision(GameObject otherObject, GameWorld world);
}