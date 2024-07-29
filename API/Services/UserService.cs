using System.Globalization;

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Core.Interfaces.Users;

namespace API.Services;


public partial class UserService(IRepositoryWrapper repositoryWrapper, IConfiguration configuration, IPermissionService permissionService, IMapper mapper) : IUserService
{
    readonly IRepositoryWrapper _repositoryWrapper = repositoryWrapper;
    private readonly IConfiguration _configuration = configuration;
    private readonly IPermissionService _permissionsService = permissionService;
    private readonly string? _secret;
    private readonly IMapper _mapper = mapper;

    public async Task<User?> FindDetailedById(int id)
    {
        return await _repositoryWrapper.User.FindDetailedById(id);
    }

    public async Task<User> Register(RegisterDto registerDto)
    {
        var user = CreateUser(registerDto);
        _repositoryWrapper.User.Create(user);
        if(registerDto.Permissions != null)
        {
            await AddPermissionsToUser(user, registerDto.Permissions);
        }
        return user;
    }

    public User CreateUser(RegisterDto registerDto)
    {
        var (salt, hash) = CreatePasswordHash(registerDto.Password);

        User user = new()
        {
            Names = registerDto.Names,
            FirstLastName = registerDto.FirstLastName,
            SecondLastName = registerDto.SecondLastName,
            EmployeeId = registerDto.EmployeeId,
            Username = GenerateUsername(registerDto.Names, registerDto.FirstLastName, registerDto.SecondLastName),
            PasswordSalt = salt,
            PasswordHash = hash,
            CreatedById = registerDto.CreatedById,
            CreatedAt = DateTime.Now,
            Status = true
        };
        return user;
    }

    public void Create(User user)
    {
        _repositoryWrapper.User.Create(user);
    }

    // returns salt and hashed password
    private static (byte[] salt, byte[] hash) CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        return (hmac.Key, hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    private string GenerateUsername(string nombre, string apellidoPaterno, string? apellidoMaterno)
    {
        // Normalize all strings to lowercase and remove accents
        nombre = NormalizeString(nombre);
        apellidoPaterno = NormalizeString(apellidoPaterno);
        apellidoMaterno = apellidoMaterno != null ? NormalizeString(apellidoMaterno) : null;

        // Get the first part of each name
        string primerNombre = nombre.Split(' ')[0];
        string primerApellido = apellidoPaterno.Split(' ')[0];
        string segundoApellido = apellidoMaterno != null ? apellidoMaterno.Split(' ')[0] : "";

        string username = $"{primerNombre}.{primerApellido}";
        bool apellidoMaternoUsed = false;

        // Check if the username already exists, if yes, append a number
        int i = 1;
        while (UsernameExists(username))
        {
            if (apellidoMaterno != null && apellidoMaterno.Length > 1 && !apellidoMaternoUsed)
            {
                string additional = string.Concat(".", segundoApellido[..2]);
                username += additional;
                apellidoMaternoUsed = true;
            }
            else
            {
                username = MyRegex().Replace(username, "") + i.ToString();
                i++;
            }
        }
        return username;
    }

    // Helper method to remove diacritics and convert to lowercase
    private static string NormalizeString(string input)
    {
        return RemoveDiacritics(input).ToLower();
    }


    static string RemoveDiacritics(string text)
    {
        return new string(text.Normalize(NormalizationForm.FormD)
            .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            .ToArray());
    }

    public async Task AddPermissionsToUser(User user, List<int> permissions)
    {
        foreach (var permission in permissions)
        {
            var permissionToAdd = await _permissionsService.GetPermissionById(permission);
            if(permissionToAdd != null)
            {
                //check if user already has permission
                var userPermissions = await _permissionsService.GetPermissionsByUser(user);
                if(!userPermissions.Contains(permissionToAdd))
                {
                    AddPermissionToUser(user, permissionToAdd);
                }
            }
        }
    }

    private async Task RemovePermissionsToUser(User user, List<int> permissions)
    {
        foreach (var permission in permissions)
        {
            try
            {
                var permissionToRemove = await _permissionsService.GetPermissionById(permission);
                RemovePermissionToUser(user, permissionToRemove);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    private void RemovePermissionToUser(User user, Permission permission)
    {
        _repositoryWrapper.User.RemovePermissionFromUser(user, permission);
    }

    private void AddPermissionToUser(User user, Permission permission)
    {
        _repositoryWrapper.User.AddPermissionToUser(user, permission);
    }



    public async Task<User> VerifyUser(string username, string password)
    {
        var user = await FindUserByUsername(username);
        if (user == null)
        {
            throw new InvalidOperationException("Usuario no encontrado");
        }
        if(!user.Status)
        {
            throw new InvalidOperationException("Usuario inactivo");
        }
        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            throw new InvalidOperationException("Credeciales incorrectas");
        }

        return user;

    }


    private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }


    public async Task<User> EditUser(EditUserDto request)
    {
        var user = await FindDetailedById(request.Id) ?? throw new InvalidOperationException("Usuario no encontrado");
        if (request.Status != null)
        {
            user.Status = (bool)request.Status;
        }
        if(request.Username != null)
        {
            var existingUser = await FindUserByUsername(request.Username);
            if(existingUser != null && existingUser.Id != user.Id)
            {
                throw new InvalidOperationException("El nombre de usuario ya está en uso");
            }
            user.Username = request.Username;
        }
        if(request.Permissions != null)
        {
            await EditPermissions(user, request.Permissions);
        }
        _mapper.Map(request, user);
        return user;
    }

    private async Task EditPermissions(User user, List<int> permissions)
    {
        var userPermissions = await _permissionsService.GetPermissionsByUser(user);
        var permissionsToAdd = permissions.Except(userPermissions.Select(p => p.Id)).ToList();
        var permissionsToRemove = userPermissions.Select(p => p.Id).Except(permissions).ToList();

        await RemovePermissionsToUser(user, permissionsToRemove);
        await AddPermissionsToUser(user, permissionsToAdd);
    }

    public async Task<User> RestorePassword(int userID, string newPassword)
    {
        var user = await FindUserById(userID) ?? throw new InvalidOperationException("Usuario no encontrado");
        using var hmac = new HMACSHA512();
        var PasswordSalt = hmac.Key;
        var PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
        user.PasswordSalt = PasswordSalt;
        user.PasswordHash = PasswordHash;
        return user;
    }

    public async Task<List<UserDto>> GetUsers()
    {
        var users = await _repositoryWrapper.User.GetAll();
        List<UserDto> response = users.Select(_mapper.Map<UserDto>).ToList();
        return response;

    }

    public async Task<UserDto> FindUserDtoById(int id)
    {
        var user = await FindDetailedById(id) ?? throw new InvalidOperationException("Usuario no encontrado");
        return _mapper.Map<UserDto>(user);
    }

    
    // data
    public async Task SaveChanges()
    {
        await _repositoryWrapper.Save();
    }

    // public async Task<IEnumerable<User>> GetUsers()
    // {
    //     return await _repositoryWrapper.User.GetAllUsersAsync();
    // }
    private bool UsernameExists(string username)
    {
        return _repositoryWrapper.User.GetByCondition(u => u.Username == username).Any();
    }

    private async Task<User?> FindUserByUsername(string username)
    {
        return await _repositoryWrapper.User.FindByUsername(username);
    }

    public async Task<User?> FindUserById(int id)
    {
        return await _repositoryWrapper.User.FindById(id);
    }

    public async Task<User?> FindUserByRefreshToken(string token)
    {
        return await _repositoryWrapper.User.FindByRefreshToken(token);
    }

    public async Task<IEnumerable<User?>> GetUsersByPermission(Permission permission)
    {
        return await _repositoryWrapper.User.GetByPermission(permission);
    }

    

    public async Task UpdateUser(User user)
    {
        _repositoryWrapper.User.UpdateUser(user);
        await _repositoryWrapper.Save();
    }

    [GeneratedRegex(@"\d+$")]
    private static partial Regex MyRegex();
}