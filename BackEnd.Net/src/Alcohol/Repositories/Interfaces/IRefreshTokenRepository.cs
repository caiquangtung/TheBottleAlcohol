using Alcohol.Models;
using System.Threading.Tasks;

namespace Alcohol.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateAsync(RefreshToken token);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<bool> RevokeAllUserTokensAsync(int userId);
        Task<int> GetUserTokenCountAsync(int userId);
        Task<bool> DeleteExpiredTokensAsync();
    }
} 