using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace CN.Lototo.Web.Services
{
    public class LototoAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _session;
        private const string SessionKey = "lototo.auth.user";

        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public LototoAuthenticationStateProvider(ProtectedSessionStorage session)
        {
            _session = session;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Se já temos em memória, devolve
            if (_currentUser.Identity?.IsAuthenticated == true)
                return new AuthenticationState(_currentUser);

            try
            {
                // Tenta carregar do SessionStorage
                var result = await _session.GetAsync<SerializableUser>(SessionKey);

                if (result.Success && result.Value != null)
                {
                    _currentUser = CreatePrincipal(result.Value);
                }
                else
                {
                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            catch (InvalidOperationException)
            {
                // Durante pré-render, JS ainda não está disponível
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return new AuthenticationState(_currentUser);
        }

        public async Task SignInAsync(SerializableUser user)
        {
            _currentUser = CreatePrincipal(user);
            await _session.SetAsync(SessionKey, user);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public async Task SignOutAsync()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            await _session.DeleteAsync(SessionKey);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        private ClaimsPrincipal CreatePrincipal(SerializableUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Nome),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim("UserId", user.UserId.ToString()),
        new Claim("Perfil", user.Perfil.ToString()),
        new Claim(ClaimTypes.Role, user.PerfilNome) // ✅ Usar o nome do perfil
    };

            if (user.PlantaId.HasValue)
            {
                claims.Add(new Claim("plantaId", user.PlantaId.Value.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(user.PlantaNome))
            {
                claims.Add(new Claim("PlantaNome", user.PlantaNome));
            }

            var identity = new ClaimsIdentity(claims, "LototoAuth");
            return new ClaimsPrincipal(identity);
        }
    }

    // ✅ Classe serializável para guardar no SessionStorage
    public class SerializableUser
    {
        public string Nome { get; set; } = "";
        public int Perfil { get; set; }
        public string PerfilNome { get; set; } = ""; // ✅ Adicione isto
        public int UserId { get; set; }
        public int? PlantaId { get; set; }
        public string? PlantaNome { get; set; }
    }
}