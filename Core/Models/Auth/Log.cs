using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Auth;

public class Log
{
    [Key]
    public int Id { get; set; }
    public DateTimeOffset Fecha { get; set; }
    public string? IP { get; set; }
    public string? TipoConsulta { get; set; }
    public int? UsuarioID { get; set; }

    [ForeignKey("UsuarioID")]
    public User? Usuario { get; set; }
    public string? Peticion { get; set; }
    public string? Mensaje { get; set; }
    public string? Contenido { get; set; }
}