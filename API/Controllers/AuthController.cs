using System.Security.Claims;
using Core.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("auth")]
[ApiController]
public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;

    // USERS
    [HttpGet("users"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var response = await _authService.GetUsers();
        return Ok(response);
    }

    [HttpGet("user/{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUser(int id)
    {
        var response = await _authService.FindUserDtoById(id);
        return Ok(response);
    }

    [HttpPost("user/register"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterAsync(RegisterDto request)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if(userIdClaim == null)
        {
            return Unauthorized("Invalid credentials");
        }
        var userId = userIdClaim.Value;
        var userRequest = await _authService.FindUserDtoById(int.Parse(userId));
        if(userRequest == null)
        {
            return Unauthorized("Invalid credentials");
        }

        request.CreatedById = userRequest.Id;
        try
        {
            User user = await _authService.Register(request);
            await _authService.SaveUser();
            return Ok(new { username = user.Username });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    [HttpPost("user/login")]
    public async Task<ActionResult<string>> Login(LoginDto request)
    {
        try
        {
            var tokens = await _authService.Login(request);
            return Ok(tokens);
        }
        catch (ArgumentException ex)
        {
            return Unauthorized(ex.Message);
            
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch(System.Security.SecurityException ex)
        {
            // return forbidden
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("user/refresh")]
    public async Task<ActionResult<string>> Refresh(RefreshTokenDto refreshToken)
    {
        try
        {
            var tokens = await _authService.RefreshTokens(refreshToken.RefreshToken);
            return Ok(tokens);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPatch("user/edit"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditUser(EditUserDto request)
    {
        try
        {
            var user = await _authService.EditUser(request);
            await _authService.UpdateUser(user);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut("user/restore_password"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> RestorePassword(RestorePasswordDto request)
    {
        try
        {
            var user = await _authService.RestorePassword(request.UserId, request.NewPassword);
            await _authService.UpdateUser(user);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    // permissions
    [HttpGet("permissions"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await _authService.GetPermissions();
        return Ok(permissions);
    }
}