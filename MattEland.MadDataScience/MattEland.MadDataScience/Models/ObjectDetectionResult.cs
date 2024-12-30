using System.Drawing;

namespace MattEland.MadDataScience.Models;

public class ObjectDetectionResult
{
    public string Name { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public Rectangle BoundingBox { get; set; } = new();
    
    public override string ToString() => $"{Name} ({Confidence:P2} Confidence) at {BoundingBox}";
}