using CN.Lototo.Client.Dto;
using CN.Lototo.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CN.Lototo.Client.Services
{
    public class AuthService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IConfiguration _configuracao;

        public AuthService(IUsuarioRepository usuarios, IConfiguration configuracao)
        {

            _usuarios = usuarios;
            _configuracao = configuracao;
        }

        public async Task<LoginResponseDto> AutenticarAsync(LoginDto loginDto)
        {
            var usuario = await _usuarios.ObterPorLoginAsync(loginDto.Login);
            if (usuario == null) return null;

            // TODO: aplicar hash de senha (BCrypt ou ASP.NET Identity)
            if (usuario.SenhaHash != loginDto.Senha) return null;

            if (!usuario.EhSuperGestor)
            {
                if (!loginDto.PlantaId.HasValue || usuario.PlantaId != loginDto.PlantaId)
                    return null;
            }

            var jwt = _configuracao.GetSection("Jwt");
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Chave"]!));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Login),
                new Claim("uid", usuario.Id.ToString()),
                new Claim("perfil", usuario.Perfil.ToString()),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
            };

            if (usuario.PlantaId.HasValue)
                claims.Add(new Claim("plantaId", usuario.PlantaId.Value.ToString()));

            var token = new JwtSecurityToken(
                issuer: jwt["Emissor"],
                audience: jwt["Publico"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiraMinutos"]!)),
                signingCredentials: credenciais);

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                NomeUsuario = usuario.NomeCompleto,
                Perfil = usuario.Perfil.ToString(),
                PlantaId = usuario.PlantaId,
                NomePlanta = usuario.Planta?.Nome
            };
        }

    }
}
