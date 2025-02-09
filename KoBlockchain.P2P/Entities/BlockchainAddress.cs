namespace KoBlockchain.P2P.Entities;

public sealed record BlockchainAddress
{
    public int Id { get; init; }
    public string Address { get; init; } = null!;
    public decimal Balance { get; init; }
}
