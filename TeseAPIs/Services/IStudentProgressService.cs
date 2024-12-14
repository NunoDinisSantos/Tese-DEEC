using TeseAPIs.Mapping.PlayerProgress;
using TeseAPIs.Mapping.PlayersProgress;

namespace TeseAPIs.Services
{
    public interface IStudentProgressService
    {
        Task<PlayerProgressResponse?> GetByIdAsync(string studentId);
        Task<IEnumerable<PlayerProgressResponse>?> GetAllAsync();
        Task<bool?> UpdateCreditsByIdAsync(string studentId, int sumCredits);
    }
}