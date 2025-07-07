using Core.AuditLogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Api.Controllers;

[Route("api/audit")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetAuditLogsResponse>> GetUsers()
    {
        var response = await _mediator.Send(new GetAuditLogsRequest());

        return Ok(response);
    }
}
