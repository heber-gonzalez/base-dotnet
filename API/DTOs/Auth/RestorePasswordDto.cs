using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Auth;



public class RestorePasswordDto
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public string NewPassword { get; set; }
}