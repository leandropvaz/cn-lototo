namespace CN.Lototo.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(Guid tenantId, string email, string perfil);

}
