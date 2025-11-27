using CN.Lototo.Client.Dto;
using CN.Lototo.Domain.Interfaces;
using CN.Lototo.Infrastructure.Storage;

namespace CN.Lototo.Client.Services
{
    public class ServicoEquipamentoApp
    {
        private readonly IEquipamentoRepository _equipamentos;
        private readonly StorageService _armazenamento;

        public ServicoEquipamentoApp(
            IEquipamentoRepository equipamentos,
            StorageService armazenamento)
        {
            _equipamentos = equipamentos;
            _armazenamento = armazenamento;
        }

        public async Task<IReadOnlyList<EquipamentoDto>> BuscarAsync(
            int? plantaId, bool superGestor, string? termo)
        {
            var lista = await _equipamentos.BuscarPorTermoAsync(plantaId, superGestor, termo);

            return lista.Select(e => new EquipamentoDto
            {
                Id = e.Id,
                Tag = e.Tag,
                Nome = e.Nome,
                Descricao = e.Descricao,
                NomePlanta = e.Planta.Nome,
                UrlArquivoProcedimento = string.IsNullOrEmpty(e.CaminhoArquivoProcedimento)
                    ? null
                    : _armazenamento.ObterUrlPublica(e.CaminhoArquivoProcedimento),
                UrlImagem = string.IsNullOrEmpty(e.CaminhoImagem)
                    ? null
                    : _armazenamento.ObterUrlPublica(e.CaminhoImagem)
            }).ToList();
        }
    }
}
