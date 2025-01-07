namespace MattEland.MadDataScience.SquirrelSimulation;

public class TilePerceptions
{
    public required WorldPosition Position { get; set; }
    public float SmellOfAcorn { get; set; }
    public float SmellOfSquirrel { get; set; }
    public float SmellOfGorilla { get; set; }
    public float SmellOfRabbit { get; set; }
    public float SmellOfTree { get; set; }
    public bool IsEdgeTile { get; set; }
}