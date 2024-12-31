namespace MattEland.MadDataScience.SquirrelSimulation;

public record WorldPosition(int X, int Y)
{
    public override string ToString() => $"{{{X},{Y}}}";
}