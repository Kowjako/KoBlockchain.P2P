using System.Security.Cryptography;
using System.Text;
using Hangfire;
using KoBlockchain.P2P.Data;
using KoBlockchain.P2P.Entities;
using Newtonsoft.Json;

namespace KoBlockchain.P2P.Jobs;

// So here we will produce new block, then another nodes (peers) which are also running BlockSyncJob
// will request full chain from node which generated a new block (storing it inside database)
// and then append this new block to themselves

[DisableConcurrentExecution(100)]
public class BlockMiningJob
{
    public const string HangfireJobId = "block-mining-job";

    private readonly ITransactionRepository _transactionRepository;
    private readonly IBlockRepository _blockRepository;

    public BlockMiningJob(ITransactionRepository transactionRepository, IBlockRepository blockRepository)
    {
        _transactionRepository = transactionRepository;
        _blockRepository = blockRepository;
    }

    public async Task CreateBlock()
    {
        var latestBlock = await _blockRepository.GetLatestBlockAsync();
        var newTransactions = await _transactionRepository.GetNewTransactionsAsync();

        if (!newTransactions.Any()) return;

        // Here we can solve math task, to append only needed transactions, but in this simple version
        // there is no need for that, so we will take only 5 of them per block

        var selectedTransactions = newTransactions.Take(5).ToList();

        var block = new BlockchainBlock()
        {
            Timestamp = DateTime.UtcNow,
            Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(selectedTransactions)),
            PreviousBlockHash = latestBlock!.Hash,
        };

        var data = $"{JsonConvert.SerializeObject(selectedTransactions)}{block.PreviousBlockHash}{DateTime.UtcNow.Ticks}";

        using var sha512 = SHA512.Create();    
        var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(data));
        block.Hash = Convert.ToBase64String(hashBytes);

        var newBlockId = await _blockRepository.AddBlockAsync(block);
        await _transactionRepository.AssignTransactionsBlockId(selectedTransactions.Select(t => t.Id), newBlockId);
        await _blockRepository.UpdateLatestBlockAsync(block);
    }
}
