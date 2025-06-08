using Dapper;
using TeseAPIs.Data;
using TeseAPIs.Models;
using TeseAPIs.Services.Helper;

namespace TeseAPIs.Services
{
    public class ChallengeProgress(IDbConnectionFactory connectionFactory) : IChallengeProgress
    {
        public async Task<ChallengeProgressData> CreateUpdateChallengeProgressById(ChallengeProgressData progressData)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $@"Select * FROM ChallengeProgress 
                        WHERE playerId = '{progressData.PlayerId}'";

            var result = await connection.QueryAsync<ChallengeProgressData>(query);

            if(result.Count() == 0)
            {
                query = $@"INSERT INTO ChallengeProgress (playerId, nick_name,coins, credits, fishcaught) VALUES ('{progressData.PlayerId}', '{progressData.Nick_Name}',{progressData.Coins}, {progressData.Credits},{progressData.FishCaught})";
                await connection.ExecuteAsync(query);

                return new ChallengeProgressData()
                {
                    Nick_Name = progressData.Nick_Name,
                    PlayerId = progressData.PlayerId,
                    Coins = progressData.Coins,
                    Credits = progressData.Credits,
                    FishCaught = progressData.FishCaught,
                };
            }

            else
            {
                result.First().Coins +=  progressData.Coins;
                result.First().Credits += progressData.Credits;
                result.First().FishCaught += progressData.FishCaught;

                query = $@"UPDATE ChallengeProgress SET coins = {result.First().Coins}, credits = {result.First().Credits}, fishcaught = {result.First().FishCaught} WHERE playerId = '{progressData.PlayerId}'";

                await connection.ExecuteAsync(query);
            }

            return result.First();
        }

        public async Task<IEnumerable<ChallengeProgressData>> GetChallengeProgress(int eventType)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var queryHelper = new GetQueryForChallenges();

            var query = queryHelper.GetProgressQueryForChallenge(eventType);

            var result = await connection.QueryAsync<ChallengeProgressData>(query);          

            return result;
        }

        public async Task<bool> ResetChallengeProgress()
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = "UPDATE ChallengeProgress SET fishcaught = 0, coins = 0, credits = 0";

            var result = await connection.ExecuteAsync(query);

            return true;
        }
    }
}
