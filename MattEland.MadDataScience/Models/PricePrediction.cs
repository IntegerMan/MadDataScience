using Microsoft.ML.Data;

namespace MattEland.MadDataScience.Models;

public class PricePrediction
{
    [ColumnName("Score")]
    public float Price;
}