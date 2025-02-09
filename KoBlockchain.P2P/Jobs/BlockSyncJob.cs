using Hangfire;
using KoBlockchain.P2P.Data;
using KoBlockchain.P2P.Entities;
using Newtonsoft.Json;

namespace KoBlockchain.P2P.Jobs;

// For p2p communication, so our service expose endpoint to get own block list
// and also our service has job which is calling another nodes to sync blocks between each other

[DisableConcurrentExecution(100)]
public class BlockSyncJob
{
    public const string HangfireJobId = "block-sync-job";

    private readonly IPeerRepository _peerRepository;
    private readonly IBlockRepository _blockRepository;

    public BlockSyncJob(IPeerRepository peerRepository, IBlockRepository blockRepository)
    {
        _peerRepository = peerRepository;
        _blockRepository = blockRepository;
    }

    public async Task SyncBlocks()
    {
        var latestBlock = await _blockRepository.GetLatestBlockAsync();
        var newLatestBlock = default(BlockchainBlock);

        var peers = await _peerRepository.GetPeersAsync();
        using var client = new HttpClient();

        foreach (var peer in peers)
        {
            client.BaseAddress = new Uri(peer.PeerUrl);
            var result = await client.GetAsync("/api/v1/host/chain");

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                var blockchain = JsonConvert.DeserializeObject<IEnumerable<BlockchainBlock>>(response);

                foreach (var block in blockchain)
                {
                    if (block.Id > latestBlock.Id)
                    {
                        await _blockRepository.AddBlockAsync(block);
                        newLatestBlock = block;
                    }
                }
            }
        }

        if (newLatestBlock is not null)
        {
            await _blockRepository.UpdateLatestBlockAsync(newLatestBlock);
        }      
    }
}
