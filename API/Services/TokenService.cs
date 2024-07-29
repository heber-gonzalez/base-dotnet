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

public partial class TokenService : ITokenService
{
    readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IConfiguration _configuration;
    private readonly IPermissionService _permissionsService;
    private readonly string? _secret;
    private IMapper _mapper;
    private IUserService _userService;


    public TokenService(
        IRepositoryWrapper repositoryWrapper, 
        IConfiguration configuration, 
        IPermissionService permissionService, 
        IMapper mapper,
        IUserService userService
        )
    {
        var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
        _secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? config["JWT_SECRET"];
        _repositoryWrapper = repositoryWrapper;
        _configuration = configuration;
        _permissionsService = permissionService;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<TokensDto> GetTokens(User user)
    {
        var accessToken = await CreateAccessToken(user);
        var refreshToken = CreateRefreshToken(user);
        TokensDto tokens = new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        await RenovateRefreshToken(user, refreshToken);   
        return tokens;
    }

    public async Task<User> VerifyRefreshToken(string refreshToken)
    {
        var user = await _userService.FindUserByRefreshToken(refreshToken) ?? throw new InvalidOperationException("Token inválido");
        if (user.RefreshToken.ExpirationDate < DateTime.Now || user.RefreshToken.Revoked)
        {
            throw new InvalidOperationException("Token inválido");
        }
        
        return user;
    }

    public async Task<User> FindUserByRefreshToken(string refreshToken)
    {
        var claims = GetTokenClaims(refreshToken);
        var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; 
        var user = await _userService.FindDetailedById(Convert.ToInt32(userId)) ?? throw new InvalidOperationException("Token inválido"); 
        
        return user;
    }

    public List<Claim> GetTokenClaims(string token)
    {
        var securityToken = ConvertJwtStringToJwtSecurityToken(token);

        return securityToken.Claims.ToList();
    }

    public static JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        
        return token;
    }

    private async Task<string> CreateAccessToken(User user)
    {
        

        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FullName ?? ""),
        ];


        var permissions = await _permissionsService.GetPermissionsByUser(user);

        claims.AddRange(permissions.Where(permission => permission.Nombre != null).Select(permission => new Claim(ClaimTypes.Role, permission.Nombre ?? "")));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret ?? throw new Exception("Secret not found")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds
        );
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return accessToken;
    }

    private static string CreateRefreshToken(User user)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FullName ?? "")
        ];

        var key = new RsaSecurityKey(RSA.Create(2048));
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );
        var refreshToken = new JwtSecurityTokenHandler().WriteToken(token);

        return refreshToken;
    }

    public async Task RenovateRefreshToken(User user, string refreshToken)
    {
        var token = await GetTokenByUserIdAsync(user.Id);
        if(token != null)
        {
            await DeleteToken(token);
        }
        await AddRefreshToken(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpirationDate = DateTime.Now.AddDays(1)
        });
    }


    private async Task<RefreshToken> GetTokenByUserIdAsync(int userId)
    {
        return await _repositoryWrapper.RefreshToken.FindByUserId(userId);
    }
    private async Task DeleteToken(RefreshToken refreshToken)
    {
        _repositoryWrapper.RefreshToken.DeleteToken(refreshToken);
        await SaveChanges();
    }
    private async Task AddRefreshToken(RefreshToken refreshToken)
    {
        _repositoryWrapper.RefreshToken.AddToken(refreshToken);
        await SaveChanges();
    }
    private async Task<RefreshToken> GetToken(string token)
    {
        return await _repositoryWrapper.RefreshToken.FindByToken(token);
    }

    public async Task SaveChanges()
    {
        await _repositoryWrapper.Save();
    }
    

}