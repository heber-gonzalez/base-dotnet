using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Auth;


public class User : BaseEntity
{
    [Required]
    public string Names { get; set; }
    [Required]
    public string FirstLastName { get; set; }
    public string? SecondLastName { get; set; }
    public string? FullName => $"{Names} {FirstLastName} {SecondLastName}";
    [Required]
    public string Username { get; set; }
    [Required]
    public byte[] PasswordHash { get; set; }
    [Required]
    public byte[] PasswordSalt { get; set; }
    public string? EmployeeId { get; set; }
    public RefreshToken? RefreshToken { get; set; }
    public int? CreatedById { get; set; }
    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }
    public List<Permission> Permissions { get; } = [];
}