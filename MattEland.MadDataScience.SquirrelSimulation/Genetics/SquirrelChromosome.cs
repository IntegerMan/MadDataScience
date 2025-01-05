using System.Collections;
using GeneticSharp;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelChromosome : BinaryChromosomeBase
{
    private const int NumVariables = 5;
    private const int BitsPerVariable = 16;

    public SquirrelChromosome() : base(NumVariables * BitsPerVariable)
    {
    }

    public override IChromosome CreateNew()
    {
        SquirrelChromosome squirrelChromosome = new();
        for (int i = 0; i < NumVariables; i++)
        {
            squirrelChromosome.SetValue(i, RandomizationProvider.Current.GetFloat(-10f, 10f));
        }

        return squirrelChromosome;
    }

    public float GetValue(int index)
    {
        // Get the bits for the gene
        BitArray arr = new BitArray(16);
        for (int i = 0; i < BitsPerVariable; i++)
        {
            Gene gene = GetGene((index * 16) + i);
            arr.Set(i, gene.Value is not 0);
        }

        // Convert the BitArray to a short
        byte[] array = new byte[2];
        arr.CopyTo(array, 0);
        short intVal = BitConverter.ToInt16(array, 0);

        // Convert the short value back to a float
        return intVal / 100.0f;
    }

    public void SetValue(int index, float value)
    {
        value = Math.Clamp(value, -10, 10);
        
        short intVal = (short)Math.Round(value * 100);

        // Get the binary representation of value
        BitArray arr = new BitArray(BitConverter.GetBytes(intVal));

        // Copy the bits into the chromosome
        for (int i = 0; i < BitsPerVariable; i++)
        {
            ReplaceGene((index * BitsPerVariable) + i, new Gene(arr[i] ? 1 : 0));
        }
    }

    public override string ToString()
        => $"Acorn: {GetValue(0)}, Squirrel: {GetValue(1)}, Doggo: {GetValue(2)}, Rabbit: {GetValue(3)}, Tree: {GetValue(4)}";
}