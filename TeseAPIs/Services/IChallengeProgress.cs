using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public interface IChallengeProgress
    {
        Task<ChallengeProgressData> CreateUpdateChallengeProgressById(ChallengeProgressData progressData);
        Task<IEnumerable<ChallengeProgressData>> GetChallengeProgress();
        Task<bool> ResetChallengeProgress();
    }
}
