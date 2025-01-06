using System.Collections;
using GeneticSharp;
using MattEland.MadDataScience.SquirrelSimulation.Brains;

namespace MattEland.MadDataScience.SquirrelSimulation.Genetics;

public class SquirrelChromosome() : BinaryChromosomeBase(NumVariables * BitsPerVariable)
{
    private const int NumVariables = 5;
    private const int BitsPerVariable = 8;

    public override IChromosome CreateNew()
    {
        SquirrelChromosome squirrelChromosome = new();
        for (int i = 0; i < NumVariables; i++)
        {
            squirrelChromosome.SetValue(i, RandomizationProvider.Current.GetFloat(-10f, 10f));
        }

        return squirrelChromosome;
    }

    private float GetValue(int index)
    {
        // Get the bits for the gene
        BitArray arr = new BitArray(BitsPerVariable);
        for (int i = 0; i < BitsPerVariable; i++)
        {
            Gene gene = GetGene((index * BitsPerVariable) + i);
            arr.Set(i, gene.Value is not 0);
        }

        // Convert the BitArray to a byte
        byte[] array = new byte[BitsPerVariable / 8];
        arr.CopyTo(array, 0);
        sbyte val = (sbyte)array[0];

        // Convert the byte value back to a float
        return Math.Clamp(val / 10.0f, -10f, 10f);
    }

    private void SetValue(int index, float value)
    {
        value = Math.Clamp(value, -10, 10);
        sbyte intVal = (sbyte)Math.Round(value * 10);

        // Get the binary representation of value
        BitArray arr = new BitArray([(byte) intVal]);

        // Copy the bits into the chromosome
        for (int i = 0; i < BitsPerVariable; i++)
        {
            ReplaceGene((index * BitsPerVariable) + i, new Gene(arr[i] ? 1 : 0));
        }
    }

    public void SetWeights(SmellWeights weights)
    {
        SetValue(0, weights.Acorn);
        SetValue(1, weights.Squirrel);
        SetValue(2, weights.Gorilla);
        SetValue(3, weights.Rabbit);
        SetValue(4, weights.Tree);
    }

    public SmellWeights GetWeights()
    {
        return new SmellWeights
        {
            Acorn = GetValue(0),
            Squirrel = GetValue(1),
            Gorilla = GetValue(2),
            Rabbit = GetValue(3),
            Tree = GetValue(4)
        };
    }
    
    public override string ToString() 
        => GetWeights().ToString();
}