using CN.Lototo.Application.Dto;
using CN.Lototo.Domain.Entities;
using CN.Lototo.Domain.Interfaces;

namespace CN.Lototo.Application.Services
{
    public class PlantasService
    {
        private readonly IPlantaRepository _repo;
        private readonly IUnitOfWork _uow;

        public PlantasService(IPlantaRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<IReadOnlyList<PlantasDto>> ListarAsync()
        {
            var list = await _repo.ListarAtivasAsync();

            return list.Select(x => new PlantasDto
            {
                Id = x.Id,
                Nome = x.Nome,
                Codigo = x.Codigo,
                Localizacao = x.Localizacao,
                Ativa = x.Ativa
            }).ToList();
        }

        public async Task<PlantasDto?> ObterAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || !entity.Ativa)
                return null;

            return new PlantasDto
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Codigo = entity.Codigo,
                Localizacao = entity.Localizacao,
                Ativa = entity.Ativa
            };
        }

        public async Task<int> CriarAsync(PlantasDto dto)
        {
            var entity = new Plantas
            {
                Nome = dto.Nome,
                Codigo = dto.Codigo,
                Localizacao = dto.Localizacao,
                Ativa = true,
                DataCriacao = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _uow.CommitAsync();

            return entity.Id;
        }

        public async Task AtualizarAsync(PlantasDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) return;

            entity.Nome = dto.Nome;
            entity.Codigo = dto.Codigo;
            entity.Localizacao = dto.Localizacao;
            entity.Ativa = dto.Ativa;
            entity.DataAtualizacao = DateTime.UtcNow;

            _repo.Update(entity);
            await _uow.CommitAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return;

            entity.Ativa = false;
            entity.DataAtualizacao = DateTime.UtcNow;

            _repo.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
