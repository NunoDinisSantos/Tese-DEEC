using Dapper;
using TeseAPIs.Data;
using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public class RewardService(IDbConnectionFactory connectionFactory) : IRewardService
    {
        public async Task<IEnumerable<Reward>> GetRewardsAsync()
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"SELECT * FROM Rewards";

            var result = await connection.QueryAsync<Reward>(query);

            return result;
        }

        public async Task<Reward> UpdateRewardById(int id, string description, int price)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"UPDATE Rewards SET name = '{description}', price = {price} WHERE id = {id}";

            var result = await connection.ExecuteAsync(query);

            if (result > 0)
            {
                var reward = new Reward()
                {
                    Id = id,
                    Name = description,
                    Price = price
                };

                return reward;
            }

            return new Reward();
        }
    }
}
