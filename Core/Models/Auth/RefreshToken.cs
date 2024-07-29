using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Auth;


public class RefreshToken
{
    [Key]
    public int Id { get; set; } //the primary key of the refresh token

    [Required]
    public string Token { get; set; } //the token value

    [Required]
    public int UserId { get; set; } //the foreign key of the user associated with the refresh token

    [ForeignKey("UserId")]
    public User User { get; set; } //the navigation property to the user entity

    [Required]
    public DateTime ExpirationDate { get; set; } //the expiration date of the refresh token

    public bool Revoked { get; set; } //a flag that indicates whether the refresh token is revoked or not

    public string? ReplacedByToken { get; set; } //the token value of the new refresh token that replaces this one
}