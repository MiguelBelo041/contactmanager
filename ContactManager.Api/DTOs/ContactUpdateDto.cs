using System.ComponentModel.DataAnnotations;

namespace ContactManager.Api.DTOs;

public class ContactUpdateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O sobrenome é obrigatório.")]
    [MaxLength(100)]
    public string Sobrenome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [MaxLength(20)]
    [RegularExpression(@"^\d+$", ErrorMessage = "O telefone deve conter apenas números.")]
    public string Telefone { get; set; } = string.Empty;
}
