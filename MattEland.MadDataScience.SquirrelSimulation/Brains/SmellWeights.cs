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

    public void Randomize(Random random)
    {
        Acorn = (float)((random.NextDouble() * 10) - 5);
        Squirrel = (float)((random.NextDouble() * 10) - 5);
        Doggo = (float)((random.NextDouble() * 10) - 5);
        Rabbit = (float)((random.NextDouble() * 10) - 5);
        Tree = (float)((random.NextDouble() * 10) - 5);
    }
}