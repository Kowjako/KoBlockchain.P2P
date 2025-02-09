using KoBlockchain.P2P.Data;
using KoBlockchain.P2P.Entities;
using Microsoft.AspNetCore.Mvc;

namespace KoBlockchain.P2P.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class HostController : ControllerBase
{
    private readonly IPeerRepository _peerRepository;
    private readonly IBlockRepository _blockRepository;

    public HostController(IPeerRepository peerRepository, IBlockRepository blockRepository)
    {
        _blockRepository = blockRepository;
        _peerRepository = peerRepository;
    }

    [HttpGet("chain")]
    public async Task<ActionResult<IEnumerable<BlockchainBlock>>> GetChain()
        => Ok(await _blockRepository.GetFullChainAsync());

    [HttpPost("peer")]
    public async Task<ActionResult> AddPeer([FromQuery]string peerUrl)
    {
        await _peerRepository.AddPeerAsync(peerUrl);
        return Created();
    }
}
