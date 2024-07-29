using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Auth;

public class TokensDto
{
    [Required]
    public string AccessToken { get; set; }
    [Required]
    public string RefreshToken { get; set; }
}