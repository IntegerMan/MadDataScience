namespace MattEland.MadDataScience.SquirrelSimulation;

public record TileVisualization
{
    public required WorldPosition Position { get; init; }
    public required float Value { get; init; }
}