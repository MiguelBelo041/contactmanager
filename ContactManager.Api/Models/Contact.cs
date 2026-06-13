using System.ComponentModel.DataAnnotations;

namespace ContactManager.Api.Models;

public class Contact
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O sobrenome é obrigatório.")]
    [MaxLength(100)]
    public string Sobrenome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [MaxLength(30)]
    public string Telefone { get; set; } = string.Empty;
}
