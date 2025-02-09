using Hangfire;
using KoBlockchain.P2P.Data;
using KoBlockchain.P2P.Entities;

namespace KoBlockchain.P2P.Jobs;

// This job is responsible for generating random transactions and add them to mempool
// mempool transactions are used by mining job to create new block for the blockchain
// while transaction is not part of block, blockId is null

[DisableConcurrentExecution(100)]
public class TransactionPusherJob
{
    public const string HangfireJobId = "transaction-pusher-job";

    private readonly ITransactionRepository _transactionRepository;
    private readonly Random _random = new Random();

    public TransactionPusherJob(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task PushNewTransactions()
    {
        var transactionsToAdd = Enumerable.Range(0, 10).Select(_ => new BlockchainTransaction
        {
            Amount = (decimal)(_random.NextDouble() * (double)(100m - 10m) + (double)10m),
            BlockId = null,
            SenderAddress = "0x" + GenerateRandomAddress(15),
            ReceiverAddress = "0x" + GenerateRandomAddress(15)
        });

        await _transactionRepository.AddTransactionsAsync(transactionsToAdd);
    }

    private string GenerateRandomAddress(int length)
        => new string(Enumerable.Range(0, length)
                                .Select(_ => _random.Next(1, 9).ToString()[0])
                                .ToArray());
}
