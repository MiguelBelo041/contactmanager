namespace ContactManager.Wpf.Models;

public class Contact
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
}
