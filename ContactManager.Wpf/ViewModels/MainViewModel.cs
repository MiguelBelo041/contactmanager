using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ContactManager.Wpf.Models;
using ContactManager.Wpf.Services;

namespace ContactManager.Wpf.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ContactApiService _api;

    public MainViewModel()
    {
        _api = new ContactApiService();
        Contatos = new ObservableCollection<Contact>();

        SalvarCommand = new RelayCommand(async _ => await SalvarAsync(), _ => !IsBusy && ContatoSelecionado == null && PodeSalvar());
        AtualizarCommand = new RelayCommand(async _ => await AtualizarAsync(), _ => !IsBusy && ContatoSelecionado != null && PodeSalvar());
        ExcluirCommand = new RelayCommand(async _ => await ExcluirAsync(), _ => !IsBusy && ContatoSelecionado != null);
        LimparCommand = new RelayCommand(_ => LimparFormulario(), _ => !IsBusy);
        RecarregarCommand = new RelayCommand(async _ => await CarregarContatosAsync(), _ => !IsBusy);

        _ = CarregarContatosAsync();
    }

    public ObservableCollection<Contact> Contatos { get; }

    private string _nome = string.Empty;
    public string Nome
    {
        get => _nome;
        set => SetProperty(ref _nome, value);
    }

    private string _sobrenome = string.Empty;
    public string Sobrenome
    {
        get => _sobrenome;
        set => SetProperty(ref _sobrenome, value);
    }

    private string _telefone = string.Empty;
    public string Telefone
    {
        get => _telefone;
        set => SetProperty(ref _telefone, value);
    }

    private Contact? _contatoSelecionado;
    public Contact? ContatoSelecionado
    {
        get => _contatoSelecionado;
        set
        {
            if (SetProperty(ref _contatoSelecionado, value))
            {
                if (value != null)
                {
                    Nome = value.Nome;
                    Sobrenome = value.Sobrenome;
                    Telefone = value.Telefone;
                }
                else
                {
                    Nome = string.Empty;
                    Sobrenome = string.Empty;
                    Telefone = string.Empty;
                }
            }
        }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private string _statusMensagem = "Pronto.";
    public string StatusMensagem
    {
        get => _statusMensagem;
        set => SetProperty(ref _statusMensagem, value);
    }

    public ICommand SalvarCommand { get; }
    public ICommand AtualizarCommand { get; }
    public ICommand ExcluirCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand RecarregarCommand { get; }

    private bool PodeSalvar()
    {
        return !string.IsNullOrWhiteSpace(Nome)
            && !string.IsNullOrWhiteSpace(Sobrenome)
            && !string.IsNullOrWhiteSpace(Telefone);
    }

    private async Task CarregarContatosAsync()
    {
        try
        {
            IsBusy = true;
            StatusMensagem = "Carregando contatos...";

            var lista = await _api.GetAllAsync();
            Contatos.Clear();
            foreach (var c in lista)
                Contatos.Add(c);

            StatusMensagem = $"{Contatos.Count} contato(s) carregado(s).";
        }
        catch (Exception ex)
        {
            StatusMensagem = "Erro ao carregar contatos.";
            MessageBox.Show($"Não foi possível carregar os contatos.\n\nVerifique se a API está em execução em http://localhost:5212.\n\nDetalhes: {ex.Message}",
                "Erro de comunicação", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SalvarAsync()
    {
        try
        {
            IsBusy = true;
            StatusMensagem = "Salvando...";

            var novo = new Contact
            {
                Nome = Nome,
                Sobrenome = Sobrenome,
                Telefone = Telefone
            };

            await _api.CreateAsync(novo);
            await CarregarContatosAsync();
            LimparFormulario();
            StatusMensagem = "Contato cadastrado com sucesso.";
        }
        catch (Exception ex)
        {
            StatusMensagem = "Erro ao salvar.";
            MessageBox.Show($"Não foi possível salvar o contato.\n\nDetalhes: {ex.Message}",
                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AtualizarAsync()
    {
        if (ContatoSelecionado is null) return;

        try
        {
            IsBusy = true;
            StatusMensagem = "Atualizando...";

            var atualizado = new Contact
            {
                Id = ContatoSelecionado.Id,
                Nome = Nome,
                Sobrenome = Sobrenome,
                Telefone = Telefone
            };

            await _api.UpdateAsync(atualizado);
            await CarregarContatosAsync();
            LimparFormulario();
            StatusMensagem = "Contato atualizado com sucesso.";
        }
        catch (Exception ex)
        {
            StatusMensagem = "Erro ao atualizar.";
            MessageBox.Show($"Não foi possível atualizar o contato.\n\nDetalhes: {ex.Message}",
                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExcluirAsync()
    {
        if (ContatoSelecionado is null) return;

        var confirma = MessageBox.Show(
            $"Deseja realmente excluir {ContatoSelecionado.Nome} {ContatoSelecionado.Sobrenome}?",
            "Confirmar exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirma != MessageBoxResult.Yes) return;

        try
        {
            IsBusy = true;
            StatusMensagem = "Excluindo...";

            await _api.DeleteAsync(ContatoSelecionado.Id);
            await CarregarContatosAsync();
            LimparFormulario();
            StatusMensagem = "Contato excluído com sucesso.";
        }
        catch (Exception ex)
        {
            StatusMensagem = "Erro ao excluir.";
            MessageBox.Show($"Não foi possível excluir o contato.\n\nDetalhes: {ex.Message}",
                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LimparFormulario()
    {
        ContatoSelecionado = null;
        Nome = string.Empty;
        Sobrenome = string.Empty;
        Telefone = string.Empty;
        StatusMensagem = "Formulário limpo.";
    }
}
