namespace KoBlockchain.P2P.Entities;

public record BlockchainBlock
{
    public int Id { get; init; }
    public byte[] Data { get; init; } = null!;
    public string Hash { get; set; } = null!;
    public string? PreviousBlockHash { get; init; }
    public DateTime Timestamp { get; init; }
}