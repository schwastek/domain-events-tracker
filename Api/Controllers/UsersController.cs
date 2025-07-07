using Api.Filters;
using Core.Dto;
using Core.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Api.Controllers;

[Route("api/users")]
[ApiController]
[UseTransaction]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetUsersResponse>> GetUsers()
    {
        var response = await _mediator.Send(new GetUsersRequest());

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UserDtoForRead>> CreateUser()
    {
        var response = await _mediator.Send(new CreateUserRequest());

        return Ok(response);
    }

    /// <summary>
    /// Useful as a playground for trying out changes during development. Logic is handled entirely in the handler.
    /// </summary>
    /// <param name="userObjectId">Finds a User by <c>UserObjectId</c></param>
    /// <returns></returns>
    [HttpPut("{userObjectId}")]
    public async Task<ActionResult<UserDtoForRead>> UpdateUser([FromRoute] Guid userObjectId)
    {
        var request = new UpdateUserRequest { UserObjectId = userObjectId };
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}
