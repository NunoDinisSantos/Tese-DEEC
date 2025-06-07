using TeseAPIs.Services.Helper;

namespace TeseAPIs.Services
{
    public class CheckWinCondition(IChallengeService challengeService, IChallengeManagerService challengeManagerService, IChallengeProgress challengeProgressService) : ICheckWinCondition
    {
        public async Task<bool> Check()
        {
            var latestChallenge = await challengeService.GetChallengeAsync();

            if (latestChallenge.Id == -1)
            {
                return  false;
            }

            int challengeType = latestChallenge.EventType;

            var result = await challengeManagerService.ValidateForChallenges(latestChallenge, false);

            if (result == false)
            {
                return false;
            }

            await challengeService.EndChallengeById(latestChallenge);

            await challengeProgressService.ResetChallengeProgress();

            return true;
        }
    }
}
