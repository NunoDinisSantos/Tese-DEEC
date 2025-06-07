using Dapper;
using TeseAPIs.Data;
using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public class ChallengeWinner(IDbConnectionFactory connectionFactory) : IChallengeWinner
    {
        public async Task<IEnumerable<ChallengeWinnerData>> GetWinners()
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"Select * FROM ChallengeWinners ORDER BY id desc";

            var result = await connection.QueryAsync<ChallengeWinnerData>(query);

            return result;
        }

        public async Task<ChallengeWinnerData> GetLastWinner()
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"Select * FROM ChallengeWinners ORDER BY id desc LIMIT 1";

            var result = await connection.QueryAsync<ChallengeWinnerData>(query);

            return result.FirstOrDefault()!;
        }

        public async Task<bool> PostWinner(ChallengeWinnerDataDTO dto)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $@"
                INSERT INTO ChallengeWinners (nick_name, player_id, challengeId) 
                VALUES (
                    '{dto.Nick_Name}',
                    '{dto.Player_Id}',
                    '{dto.ChallengeId}'
                );";

            await connection.ExecuteAsync(query);

            return true;
        }
    }
}
