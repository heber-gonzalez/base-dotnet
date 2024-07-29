namespace API.DTOs.Auth;
public class UserDto
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Names { get; set; }
    public string? FirstLastName { get; set; }
    public string? SecondLastName { get; set; }
    public string? EmployeeId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? CreatedById { get; set; }
    public bool Status { get; set; }
    public List<Permission>? Permissions { get; set; }
}