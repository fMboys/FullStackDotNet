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

    public AuthService(
        HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
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