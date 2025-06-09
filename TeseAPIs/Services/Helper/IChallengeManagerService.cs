using TeseAPIs.Models;

namespace TeseAPIs.Services.Helper
{
    public interface IChallengeManagerService
    {
        public Task<int> ValidateForChallenges(Challenge id, bool endedByApp);
    }
}
