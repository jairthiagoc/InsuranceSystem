using Microsoft.AspNetCore.Mvc;
using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;
using Microsoft.AspNetCore.Authorization;

namespace ContractService.Adapters.Inbound.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Temporariamente desabilitado para desenvolvimento
public class ContractsController : ControllerBase
{
    private readonly ICreateContractPort _createContractPort;
    private readonly IGetContractsPort _getContractsPort;
    private readonly IGetContractByIdPort _getContractByIdPort;
    private readonly IGetContractByProposalIdPort _getContractByProposalIdPort;

    public ContractsController(
        ICreateContractPort createContractPort,
        IGetContractsPort getContractsPort,
        IGetContractByIdPort getContractByIdPort,
        IGetContractByProposalIdPort getContractByProposalIdPort)
    {
        _createContractPort = createContractPort;
        _getContractsPort = getContractsPort;
        _getContractByIdPort = getContractByIdPort;
        _getContractByProposalIdPort = getContractByProposalIdPort;
    }

    [HttpPost]
    public async Task<ActionResult<ContractResult>> CreateContract([FromBody] CreateContractRequest request)
    {
        try
        {
            var result = await _createContractPort.ExecuteAsync(request);
            return CreatedAtAction(nameof(GetContract), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContractResult>>> GetContracts()
    {
        var contracts = await _getContractsPort.ExecuteAsync();
        return Ok(contracts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContractResult>> GetContract(Guid id)
    {
        var contract = await _getContractByIdPort.ExecuteAsync(id);
        
        if (contract == null)
            return NotFound();

        return Ok(contract);
    }

    [HttpGet("proposal/{proposalId}")]
    public async Task<ActionResult<ContractResult>> GetContractByProposalId(Guid proposalId)
    {
        var contract = await _getContractByProposalIdPort.ExecuteAsync(proposalId);
        
        if (contract == null)
            return NotFound();

        return Ok(contract);
    }
} 