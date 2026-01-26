using Quiniela.Models;

namespace Quiniela.Services
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);
    }
}