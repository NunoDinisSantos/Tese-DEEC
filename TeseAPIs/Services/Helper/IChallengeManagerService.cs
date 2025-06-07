using TeseAPIs.Models;

namespace TeseAPIs.Services.Helper
{
    public interface IChallengeManagerService
    {
        public Task<bool> ValidateForChallenges(Challenge id, bool endedByApp);
    }
}
