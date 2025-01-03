namespace MattEland.MadDataScience.Models;

using Microsoft.ML.Data;

public class VideoGame
{
    [LoadColumn(0)]
    public string Title { get; set; }

    [LoadColumn(1)]
    public float Rating { get; set; }

    [LoadColumn(2)]
    public string TargetAgeGroup { get; set; }

    [LoadColumn(3)]
    public float Price { get; set; }

    [LoadColumn(4)]
    public string Platform { get; set; }

    [LoadColumn(5)]
    public bool RequiresSpecialDevice { get; set; }

    [LoadColumn(6)]
    public string Developer { get; set; }

    [LoadColumn(7)]
    public string Publisher { get; set; }

    [LoadColumn(8)]
    public float Year { get; set; }

    [LoadColumn(9)]
    public string Genre { get; set; }

    [LoadColumn(10)]
    public bool Multiplayer { get; set; }

    [LoadColumn(11)]
    public float HoursLong { get; set; }

    [LoadColumn(12)]
    public string Graphics { get; set; }

    [LoadColumn(13)]
    public string Music { get; set; }

    [LoadColumn(14)]
    public string Story { get; set; }

    [LoadColumn(15)]
    public string ReviewText { get; set; }

    [LoadColumn(16)]
    public string GameMode { get; set; }

    [LoadColumn(17)]
    public float MinPlayers { get; set; }
}