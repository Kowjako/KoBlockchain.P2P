using Dapper;
using KoBlockchain.P2P.Entities;
using Microsoft.Data.Sqlite;

namespace KoBlockchain.P2P.Data;

public interface IBlockRepository
{
    Task<int> AddBlockAsync(BlockchainBlock block);
    Task UpdateLatestBlockAsync(BlockchainBlock block);
    Task<BlockchainBlock?> GetLatestBlockAsync();
    Task<IEnumerable<BlockchainBlock>> GetFullChainAsync();
}

public class BlockRepository : IBlockRepository
{
    private readonly IConfiguration _configuration;

    private readonly ITransactionRepository _transactionRepository;

    public BlockRepository(IConfiguration configuration, ITransactionRepository transactionRepository)
    {
        _configuration = configuration;
        _transactionRepository = transactionRepository;
    }

    public Task<int> AddBlockAsync(BlockchainBlock block)
    {
        var cmd = "INSERT INTO Blocks (Hash, Data, PreviousBlockHash, Timestamp) VALUES (@Hash, @Data, @PreviousBlockHash, @Timestamp); " +
                  "SELECT last_insert_rowid()";
        using var conn = new SqliteConnection(_configuration["Database"]);
        return conn.ExecuteScalarAsync<int>(cmd, block);
    }

    public Task<BlockchainBlock?> GetLatestBlockAsync()
    {
        var cmd = "SELECT * FROM LatestBlock LIMIT 1";
        using var conn = new SqliteConnection(_configuration["Database"]);
        return conn.QueryFirstOrDefaultAsync<BlockchainBlock>(cmd);
    }

    public Task<IEnumerable<BlockchainBlock>> GetFullChainAsync()
    {
        using var conn = new SqliteConnection(_configuration["Database"]);
        return conn.QueryAsync<BlockchainBlock>("SELECT * FROM Blocks");
    }

    public async Task UpdateLatestBlockAsync(BlockchainBlock block)
    {
        using var conn = new SqliteConnection(_configuration["Database"]);
        await conn.OpenAsync();
        var tran = await conn.BeginTransactionAsync();

        var cmd = "DELETE FROM LatestBlock";
        await conn.ExecuteAsync(cmd);

        var insertCmd = "INSERT INTO LatestBlock (Hash, Data, PreviousBlockHash, Timestamp) VALUES (@Hash, @Data, @PreviousBlockHash, @Timestamp)";
        await conn.ExecuteAsync(insertCmd, block);

        await tran.CommitAsync();
    }
}
