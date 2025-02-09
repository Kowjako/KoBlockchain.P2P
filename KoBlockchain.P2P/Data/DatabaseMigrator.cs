using System.Security.Cryptography;
using System.Text;
using Dapper;
using KoBlockchain.P2P.Entities;
using Microsoft.Data.Sqlite;

namespace KoBlockchain.P2P.Data;

public interface IDatabaseMigrator
{
    Task MigrateAsync();
}

public class DatabaseMigrator : IDatabaseMigrator
{
    private readonly IConfiguration _configuration;
    private readonly IBlockRepository _blockRepository;

    public DatabaseMigrator(IConfiguration configuration, IBlockRepository blockRepository)
    {
        _configuration = configuration;
        _blockRepository = blockRepository;
    }

    public async Task MigrateAsync()
    {
        var dbFilePath = Path.Combine(Directory.GetCurrentDirectory(), "KoBlockchain.P2P.Data.db");
        var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "init.sql");

        if (!File.Exists(dbFilePath))
        {
            var connString = _configuration["Database"];

            using var conn = new SqliteConnection(connString);
            await conn.OpenAsync();

            var createBlocksTableCmd = await File.ReadAllTextAsync(scriptPath);
            await conn.ExecuteAsync(createBlocksTableCmd);

            // Each node, when joining to blockchain network, should create genesis block
            // this genesis block is identical for each node, meaning hash and data are the same
            // for genesis blocks on each node

            var genesisBlockObject = new BlockchainBlock()
            {
                Data = Encoding.UTF8.GetBytes("GenesisBlock by Wlodziu"),
                Timestamp = DateTime.UtcNow,
                PreviousBlockHash = null, // because genesis block its first block
            };

            using var sha512 = SHA512.Create();
            var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes("GenesisBlock by Wlodziu"));
            genesisBlockObject.Hash = Convert.ToBase64String(hashBytes);

            await _blockRepository.AddBlockAsync(genesisBlockObject);
            await _blockRepository.UpdateLatestBlockAsync(genesisBlockObject);
        }
    }
}
