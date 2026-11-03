using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    private const string TokenKey = "authToken";

    public bool IsAuthenticated { get; private set; }

    public event Action? AuthenticationStateChanged;
    private readonly UserSessionService _session;

    public AuthService(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        UserSessionService session)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _session = session;
    }

    public async Task<bool> LoginAsync(
        string email,
        string password)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            new
            {
                Email = email,
                Password = password
            });

        if (!response.IsSuccessStatusCode)
            return false;

            

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        _session.UserId = result.Email;
            _session.Role = result.FullName;

        if (result is null)
            return false;

        await _localStorage.SetItemAsync(
            TokenKey,
            result.Token);

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                result.Token);

        IsAuthenticated = true;

        AuthenticationStateChanged?.Invoke();

        return true;
    }

    public async Task<bool> RegisterAsync(
        RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/register",
            request);

        return response.IsSuccessStatusCode;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);

        _httpClient.DefaultRequestHeaders.Authorization = null;

        IsAuthenticated = false;
         _session.Clear();

        AuthenticationStateChanged?.Invoke();
    }

    public async Task InitializeAsync()
    {
        var token = await _localStorage
            .GetItemAsync<string>(TokenKey);

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            IsAuthenticated = true;
        }
    }
}