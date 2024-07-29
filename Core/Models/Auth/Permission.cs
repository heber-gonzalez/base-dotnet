using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Models.Auth;
public class Permission
{
    [Key]
    public int Id { get; set; }
    public string? Nombre { get; set; }
    [JsonIgnore]
    public List<User> Users { get; } = new();
}