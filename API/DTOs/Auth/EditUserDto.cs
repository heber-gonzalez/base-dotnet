using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Auth;

public class EditUserDto
    {
        [Required]
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string? Names { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string? FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        [StringLength(20, MinimumLength = 4)]
        public string? Username { get; set; }
        public string? EmployeeId { get; set; }
        public List<int>? Permissions { get; set; }
        public bool? Status { get; set; }

    }