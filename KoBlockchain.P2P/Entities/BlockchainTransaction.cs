namespace KoBlockchain.P2P.Entities;

public sealed record BlockchainTransaction
{
    public int Id { get; init; }
    public int? BlockId { get; set; }
    public string SenderAddress { get; init; } = null!;
    public string ReceiverAddress { get; init; } = null!;
    public decimal Amount { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
