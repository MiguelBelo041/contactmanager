using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using ContactManager.Wpf.Models;

namespace ContactManager.Wpf.Services;

public class ContactApiService
{
    private const string FallbackBaseUrl = "http://localhost:5212/";

    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ContactApiService() : this(LoadBaseUrl())
    {
    }

    public ContactApiService(string baseUrl)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    private static string LoadBaseUrl()
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path))
                return FallbackBaseUrl;

            var json = File.ReadAllText(path);
            using var document = JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty("ApiBaseUrl", out var urlElement))
            {
                var url = urlElement.GetString();
                if (!string.IsNullOrWhiteSpace(url))
                    return url;
            }
        }
        catch
        {
        }

        return FallbackBaseUrl;
    }

    public async Task<List<Contact>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/contacts");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<Contact>>(_jsonOptions);
        return list ?? new List<Contact>();
    }

    public async Task<Contact?> GetByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"api/contacts/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Contact>(_jsonOptions);
    }

    public async Task<Contact> CreateAsync(Contact contact)
    {
        var payload = new
        {
            nome = contact.Nome,
            sobrenome = contact.Sobrenome,
            telefone = contact.Telefone
        };

        var response = await _http.PostAsJsonAsync("api/contacts", payload);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<Contact>(_jsonOptions);
        return created ?? throw new InvalidOperationException("Resposta vazia ao criar contato.");
    }

    public async Task UpdateAsync(Contact contact)
    {
        var payload = new
        {
            nome = contact.Nome,
            sobrenome = contact.Sobrenome,
            telefone = contact.Telefone
        };

        var response = await _http.PutAsJsonAsync($"api/contacts/{contact.Id}", payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/contacts/{id}");
        response.EnsureSuccessStatusCode();
    }
}
