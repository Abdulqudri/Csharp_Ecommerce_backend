
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}[controller]")]
[Produces("application/json")]
[Authorize]
public class BaseApiController : ControllerBase
{
    protected string? UserId => User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    protected string? UserEmail => User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
}