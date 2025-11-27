using CN.Lototo.Domain.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CN.Lototo.Web.Services
{
    public class AuthService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly LototoAuthenticationStateProvider _authProvider;

        public AuthService(IUsuarioRepository usuarios,
                           AuthenticationStateProvider authProvider)
        {
            _usuarios = usuarios;
            _authProvider = (LototoAuthenticationStateProvider)authProvider;
        }

        public async Task<bool> LoginAsync(string login, string senha)
        {
            var usuario = await _usuarios.ObterPorLoginAsync(login);

            if (usuario == null)
                return false;

            // TODO: depois troca por hash de senha
            if (usuario.SenhaHash != GerarHash(senha))
                return false;

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Login),
                    new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),  // opcional se quiser usar Authorize(Roles)
                    new Claim("UserId", usuario.Id.ToString()),
                    new Claim("Perfil", ((int)usuario.Perfil).ToString()),
                    new Claim("PlantaId", usuario.PlantaId?.ToString() ?? "")
                };

            if (usuario.PlantaId.HasValue)
                claims.Add(new Claim("plantaId", usuario.PlantaId.Value.ToString()));

            var identity = new ClaimsIdentity(claims, "LototoAuth");
            var principal = new ClaimsPrincipal(identity);

            // avisa o Blazor que agora tem user logado
            _authProvider.SignIn(principal);

            return true;
        }

        public Task LogoutAsync()
        {
            _authProvider.SignOut();
            return Task.CompletedTask;
        }

        private string GerarHash(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
