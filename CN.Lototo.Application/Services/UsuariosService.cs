using CN.Lototo.Application.Dto;
using CN.Lototo.Domain.Entities;
using CN.Lototo.Domain.Enums;
using CN.Lototo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CN.Lototo.Application.Services
{
    public class UsuariosService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IUnitOfWork _uow;

        public UsuariosService(IUsuarioRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<List<UsuariosDto>> ListarAsync()
        {
            var usuarios = await _repo.GetAsync();

            return usuarios.Select(u => new UsuariosDto
            {
                Id = u.Id,
                Login = u.Login,
                NomeCompleto = u.NomeCompleto,
                Perfil = (int)u.Perfil,
                Ativa = u.Ativa,
                PlantaId = u.PlantaId
            }).ToList();
        }

        public async Task CriarAsync(UsuariosDto dto)
        {
            var entity = new Usuarios
            {
                Login = dto.Login,
                NomeCompleto = dto.NomeCompleto,
                Perfil = (PerfilUsuario)dto.Perfil,
                Ativa = dto.Ativa,
                PlantaId = dto.PlantaId,
                SenhaHash = GerarHash(dto.Senha),
                DataCriacao = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _uow.CommitAsync();        // 👈 AQUI é que GRAVA
        }

        public async Task AtualizarAsync(UsuariosDto dto)
        {
            var usuario = await _repo.GetByIdAsync(dto.Id);

            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            usuario.Login = dto.Login;
            usuario.NomeCompleto = dto.NomeCompleto;
            usuario.Perfil = (PerfilUsuario)dto.Perfil;
            usuario.Ativa = dto.Ativa;
            usuario.PlantaId = dto.PlantaId;
            usuario.DataAtualizacao = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Senha))
                usuario.SenhaHash = GerarHash(dto.Senha);

            _repo.Update(usuario);
            await _uow.CommitAsync();        // 👈 COMMIT no update
        }

        public async Task RemoverAsync(int id)
        {
            var usuario = await _repo.GetByIdAsync(id);
            if (usuario == null)
                return;

            // soft delete
            usuario.Ativa = false;
            usuario.DataAtualizacao = DateTime.UtcNow;

            _repo.Update(usuario);
            await _uow.CommitAsync();        // 👈 COMMIT no delete lógico
        }

        private string GerarHash(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }

}
