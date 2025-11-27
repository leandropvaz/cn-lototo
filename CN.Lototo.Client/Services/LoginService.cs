using Blazored.LocalStorage;
using CN.Lototo.Client.Dto;
using Microsoft.AspNetCore.Components;

namespace CN.Lototo.Client.Services
{
    public class LoginService
    {
        private readonly AuthService _authService;
        private readonly NavigationManager _nav;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthStateProvider _authStateProvider;

        public LoginService(AuthService authService, NavigationManager nav, ILocalStorageService localStorage, CustomAuthStateProvider authStateProvider)
        {
            _authService = authService;
            _nav = nav;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<LoginResponseDto> Login(LoginDto dto)
        {
            var response = await _authService.AutenticarAsync(dto);
            if (response == null) return null;

            await _localStorage.SetItemAsync("authToken", response.Token);
            await _localStorage.SetItemAsync("user", response.NomeUsuario);

            await _authStateProvider.NotifyUserAuthentication(response.Token);
            _nav.NavigateTo("/");
            return response;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("user");

            _authStateProvider.NotifyUserLogout();
            _nav.NavigateTo("/login");
        }
    }
}
