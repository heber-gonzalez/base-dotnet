using System.ComponentModel.DataAnnotations;

namespace Core.Models.Auth;

public class ApiKey
{
    [Key]
    public int ApiKeyId { get; set; }
    [Required]
    public string HashedKey { get; set; }
    [Required]
    public string Name { get; set; }
    public bool Estatus { get; set; }
}