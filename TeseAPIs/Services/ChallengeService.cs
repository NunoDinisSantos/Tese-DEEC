using Dapper;
using TeseAPIs.Data;
using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public class ChallengeService(IDbConnectionFactory connectionFactory) : IChallengeService
    {
        public async Task<Challenge> CreateChallenge(ChallengeDTO challengeDTO)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"SELECT COUNT(*) FROM Challenge";
            var result = connection.ExecuteScalar<int>(query)!;

            var challenge = new Challenge()
            {
                Id = result++,
                Description = challengeDTO.Description,
                StartDate = DateTime.Parse(challengeDTO.StartDate).ToString("dd-MM-yyy"),
                EndDate = DateTime.Parse(challengeDTO.EndDate).ToString("dd-MM-yyy"),
                Ended = false,
            };

            query = $"INSERT INTO Challenge (id,startdate,enddate,description,ended) VALUES ({challenge.Id},'{DateTime.Parse(challenge.StartDate).ToString("dd-MM-yyy")}','{DateTime.Parse(challenge.EndDate).ToString("dd-MM-yyy")}','{challenge.Description}',{challenge.Ended})";
            result = await connection.ExecuteAsync(query);
            return challenge;
        }
        public async Task<Challenge> EndChallengeById(Challenge challenge)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"UPDATE Challenge SET ended = 1 WHERE id = {challenge.Id}";

            var result = await connection.ExecuteAsync(query);

            if (result > 0)
            {
                var returnChallenge = new Challenge()
                {
                    Id = challenge.Id,
                    Description = challenge.Description,
                    EndDate = challenge.EndDate,
                    Ended = true
                };

                return returnChallenge;
            }

            return new Challenge();
        }

        public async Task<Challenge> GetChallengeAsync()
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"SELECT * FROM Challenge WHERE ended = 0 order by startdate";

            var result = await connection.QueryAsync<Challenge>(query);

            var challenge = result.FirstOrDefault();

            if (challenge != null)
            {
                return challenge;
            }

            else
            {
                return new Challenge();
            }
        }

        public async Task<IEnumerable<Challenge>> GetChallengesAsync()
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"SELECT * FROM Challenge ORDER BY enddate desc, ended";

            var result = await connection.QueryAsync<Challenge>(query);

            return result;
        }

        public async Task<Challenge> UpdateChallengeById(Challenge challenge)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"UPDATE Challenge SET description = '{challenge.Description}', enddate = '{challenge.EndDate}' WHERE id = {challenge.Id}";

            var result = await connection.ExecuteAsync(query);

            if (result > 0)
            {
                var returnChallenge = new Challenge()
                {
                    Id = challenge.Id,
                    Description = challenge.Description,
                    EndDate = challenge.EndDate
                };

                return returnChallenge;
            }

            return new Challenge();
        }
    }
}
