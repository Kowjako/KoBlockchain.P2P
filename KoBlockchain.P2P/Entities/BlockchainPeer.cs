namespace KoBlockchain.P2P.Entities;

public sealed record BlockchainPeer
{
    public int Id { get; init; }
    public string PeerUrl { get; init; } = null!;
}
