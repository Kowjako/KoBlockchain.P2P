using Dapper;
using KoBlockchain.P2P.Entities;
using Microsoft.Data.Sqlite;

namespace KoBlockchain.P2P.Data;

public interface ITransactionRepository
{
    Task AddTransactionsAsync(IEnumerable<BlockchainTransaction> transactions);
    Task<IEnumerable<BlockchainTransaction>> GetNewTransactionsAsync();
    Task AssignTransactionsBlockId(IEnumerable<int> transactionIds, int blockId);
}

public class TransactionRepository : ITransactionRepository
{
	private readonly IConfiguration _configuration;

	public TransactionRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task AddTransactionsAsync(IEnumerable<BlockchainTransaction> transactions)
    {
        using var conn = new SqliteConnection(_configuration["Database"]);
        var cmd = @"INSERT INTO Transactions (BlockId, SenderAddress, ReceiverAddress, Amount, CreatedAt) VALUES " +
                  @"(@BlockId, @SenderAddress, @ReceiverAddress, @Amount, @CreatedAt)";

        foreach (var tran in transactions)
        {
            await conn.ExecuteAsync(cmd, tran);
        }
    }

    public Task AssignTransactionsBlockId(IEnumerable<int> transactionIds, int blockId)
    {
        using var conn = new SqliteConnection(_configuration["Database"]);
        var cmd = @"UPDATE Transactions SET BlockId = @BlockId WHERE Id IN @Ids";
        return conn.ExecuteAsync(cmd, new { BlockId = blockId, Ids = transactionIds});
    }

    public Task<IEnumerable<BlockchainTransaction>> GetNewTransactionsAsync()
    {
        using var conn = new SqliteConnection(_configuration["Database"]);
        return conn.QueryAsync<BlockchainTransaction>("SELECT * FROM Transactions WHERE BlockId IS NULL");
    }
}
