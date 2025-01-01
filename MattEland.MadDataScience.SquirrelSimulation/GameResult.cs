namespace MattEland.MadDataScience.SquirrelSimulation;

public record GameResult
{
    public required int TurnsLeft { get; init; }
    public required int AcornsOnBoard { get; init; }
    public required int SquirrelsOnBoard { get; init; }
    public required int RabbitsOnBoard { get; init; }
    public required int WinningSquirrels { get; init; }
}