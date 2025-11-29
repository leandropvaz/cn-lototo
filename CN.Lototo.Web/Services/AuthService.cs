using CN.Lototo.Domain.Enums;
using CN.Lototo.Domain.Interfaces;
using CN.Lototo.Web.Dto;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace CN.Lototo.Web.Services
{
    public class AuthService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly LototoAuthenticationStateProvider _authProvider;

        public AuthService(
            IUsuarioRepository usuarios,
            AuthenticationStateProvider authProvider)
        {
            _usuarios = usuarios;
            _authProvider = (LototoAuthenticationStateProvider)authProvider;
        }

        /// <summary>
        /// Login permitindo Super Gestor sem planta.
        /// Para Administrador/Usuário, planta continua obrigatória.
        /// </summary>
        public async Task<LoginResult> LoginAsync(string login, string senha, int? plantaId)
        {
            login = (login ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
                return LoginResult.Fail("Informe o login e a senha.");

            // 👉 Aqui buscamos só por login (sem filtrar por planta)
            //    PRECISA ter esse método no repositório: ObterPorLoginAsync(string login)
            var usuario = await _usuarios.ObterPorLoginAsync(login);

            if (usuario == null)
                return LoginResult.Fail("Credenciais inválidas.");

            if (usuario.SenhaHash != GerarHash(senha))
                return LoginResult.Fail("Credenciais inválidas.");

            var perfil = (PerfilUsuario)usuario.Perfil;

            // Se NÃO for SuperGestor, planta é obrigatória
            if (perfil != PerfilUsuario.SuperGestor)
            {
                if (plantaId is null || plantaId == 0)
                    return LoginResult.Fail("Selecione a planta.");

                // Se o usuário tiver planta fixa, valida se bate com a escolhida
                if (usuario.PlantaId.HasValue && usuario.PlantaId.Value != plantaId.Value)
                    return LoginResult.Fail("Você não tem acesso a esta planta.");
            }
            else
            {
                // Super Gestor → ignora planta, fica sem planta em sessão
                plantaId = null;
            }

            // ✅ Criar objeto serializável para o authentication state provider
            var userInfo = new SerializableUser
            {
                Nome = usuario.NomeCompleto,
                UserId = usuario.Id,
                Perfil = (int)usuario.Perfil,
                PerfilNome = usuario.Perfil.ToString(),
                PlantaId = plantaId,                          // aqui pode ser null para SuperGestor
                PlantaNome = usuario.Planta?.Nome             // se vier carregada
            };

            await _authProvider.SignInAsync(userInfo);

            return LoginResult.Ok();
        }

        public async Task LogoutAsync()
        {
            await _authProvider.SignOutAsync();
        }

        private string GerarHash(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
