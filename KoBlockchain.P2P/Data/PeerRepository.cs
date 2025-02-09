using Dapper;
using KoBlockchain.P2P.Entities;
using Microsoft.Data.Sqlite;

namespace KoBlockchain.P2P.Data;

public interface IPeerRepository
{
    Task AddPeerAsync(string peerUrl);
    Task<IEnumerable<BlockchainPeer>> GetPeersAsync();
}

public class PeerRepository : IPeerRepository
{
    private readonly IConfiguration _configuration;

    public PeerRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task AddPeerAsync(string peerUrl)
    {
        var cmd = @"INSERT INTO Peers (PeerUrl) VALUES (@PeerUrl)";

        using var conn = new SqliteConnection(_configuration["Database"]);
        return conn.ExecuteAsync(cmd, new { PeerUrl = peerUrl });
    }

    public Task<IEnumerable<BlockchainPeer>> GetPeersAsync()
    {
        using var conn = new SqliteConnection(_configuration["Database"]);
        return conn.QueryAsync<BlockchainPeer>("SELECT * FROM Peers");
    }
}
