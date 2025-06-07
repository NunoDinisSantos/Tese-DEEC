using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public interface IChallengeWinner
    {
        public Task<IEnumerable<ChallengeWinnerData>> GetWinners();
        public Task<bool> PostWinner(ChallengeWinnerDataDTO dto);
    }
}
