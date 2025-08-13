using Microsoft.AspNetCore.Mvc;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ProposalService.Adapters.Inbound.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Temporariamente desabilitado para desenvolvimento
public class ProposalsController : ControllerBase
{
    private readonly ICreateProposalPort _createProposalPort;
    private readonly IGetProposalsPort _getProposalsPort;
    private readonly IGetProposalByIdPort _getProposalByIdPort;
    private readonly IGetProposalsByStatusPort _getProposalsByStatusPort;
    private readonly IUpdateProposalStatusPort _updateProposalStatusPort;

    public ProposalsController(
        ICreateProposalPort createProposalPort,
        IGetProposalsPort getProposalsPort,
        IGetProposalByIdPort getProposalByIdPort,
        IGetProposalsByStatusPort getProposalsByStatusPort,
        IUpdateProposalStatusPort updateProposalStatusPort)
    {
        _createProposalPort = createProposalPort;
        _getProposalsPort = getProposalsPort;
        _getProposalByIdPort = getProposalByIdPort;
        _getProposalsByStatusPort = getProposalsByStatusPort;
        _updateProposalStatusPort = updateProposalStatusPort;
    }

    [HttpPost]
    public async Task<ActionResult<ProposalResult>> CreateProposal([FromBody] CreateProposalRequest request)
    {
        try
        {
            var result = await _createProposalPort.ExecuteAsync(request);
            return CreatedAtAction(nameof(GetProposal), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProposalResult>>> GetProposals(
        [FromQuery] string? statusFilter = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<ProposalStatus>(statusFilter, out var status))
            {
                var proposals = await _getProposalsByStatusPort.ExecuteAsync(status);
                return Ok(proposals);
            }
            
            var allProposals = await _getProposalsPort.ExecuteAsync();
            return Ok(allProposals);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProposalResult>> GetProposal(Guid id)
    {
        var proposal = await _getProposalByIdPort.ExecuteAsync(id);
        
        if (proposal == null)
            return NotFound();

        return Ok(proposal);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ProposalResult>> UpdateProposalStatus(Guid id, [FromBody] UpdateProposalStatusRequest request)
    {
        try
        {
            if (id != request.Id)
                return BadRequest("ID in URL must match ID in request body");

            var result = await _updateProposalStatusPort.ExecuteAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
} 