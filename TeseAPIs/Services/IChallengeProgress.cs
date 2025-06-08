using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public interface IChallengeProgress
    {
        Task<ChallengeProgressData> CreateUpdateChallengeProgressById(ChallengeProgressData progressData);
        Task<IEnumerable<ChallengeProgressData>> GetChallengeProgress(int eventType);
        Task<bool> ResetChallengeProgress();
    }
}
