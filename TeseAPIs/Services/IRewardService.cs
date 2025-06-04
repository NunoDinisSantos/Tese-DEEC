using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public interface IRewardService
    {
        Task<IEnumerable<Reward>> GetRewardsAsync();

        Task<Reward> UpdateRewardById(int id, string description, int price);
    }
}
