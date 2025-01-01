namespace MattEland.MadDataScience.SquirrelSimulation.Brains;

public record SmellWeights
{
    public float Acorn { get; set; }
    public float Squirrel { get; set; }
    public float Doggo { get; set; }
    public float Rabbit { get; set; }
    public float Tree { get; set; }

    public override string ToString() 
        => $"Acorn: {Acorn}, Squirrel: {Squirrel}, Doggo: {Doggo}, Rabbit: {Rabbit}, Tree: {Tree}";
}