using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Core.Interfaces.Users;
namespace API.Services;

public partial class AuthService(
        IPermissionService permissionService,
        IUserService userService,
        ITokenService tokenService
    ) : IAuthService
{
    private readonly IPermissionService _permissionsService = permissionService;
    private readonly ITokenService _tokenService = tokenService;
    private IUserService _userService = userService;

    // USERS

    public async Task<User> Register(RegisterDto registerDto)
    {
        User user = _userService.CreateUser(registerDto);

        _userService.Create(user);

        if(registerDto.Permissions != null)
        {
            await _userService.AddPermissionsToUser(user, registerDto.Permissions);
        }

        await _userService.SaveChanges();

        return user;
    }

    public async Task<TokensDto> Login(LoginDto loginDto)
    {
        User user = await _userService.VerifyUser(loginDto.Username, loginDto.Password) ?? throw new UnauthorizedAccessException("Invalid credentials");
        return await _tokenService.GetTokens(user);
    }

    public async Task<TokensDto> RefreshTokens(string refreshToken)
    {
        User user = await _tokenService.VerifyRefreshToken(refreshToken) ?? throw new UnauthorizedAccessException("Invalid token");
        return await _tokenService.GetTokens(user);
    }

    public async Task<User> EditUser(EditUserDto registerDto)
    {
        return await _userService.EditUser(registerDto);
    }

    public async Task<User> RestorePassword(int userID, string newPassword)
    {
        return await _userService.RestorePassword(userID, newPassword);
    }

    public async Task<List<UserDto>> GetUsers()
    {
        return await _userService.GetUsers();
    }

    public async Task<UserDto> FindUserDtoById(int id)
    {
        return await _userService.FindUserDtoById(id);
    }

    public async Task UpdateUser(User user)
    {
        await _userService.UpdateUser(user);
    }

    public async Task SaveUser()
    {
        await _userService.SaveChanges();
    }

    // PERMISSIONS
    public async Task<IEnumerable<Permission>> GetPermissions()
    {
        return await _permissionsService.GetPermissions();
    }
    

}