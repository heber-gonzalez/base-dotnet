using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Auth;



public class RegisterDto
    {
        public string? Username { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Names { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? EmployeeId { get; set; }
        [Required]
        public string Password { get; set; }
        public List<int>? Permissions { get; set; }
        public int? CreatedById { get; set; }

    }