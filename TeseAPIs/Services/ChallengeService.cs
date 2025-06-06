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

            var startDate = DateTime.ParseExact(challengeDTO.StartDate, "dd-MM-yyyy", null);
            var endDate = DateTime.ParseExact(challengeDTO.EndDate, "dd-MM-yyyy", null);

            var challenge = new Challenge()
            {
                Description = challengeDTO.Description,
                StartDate = startDate.ToString("dd-MM-yyyy"),
                EndDate = endDate.ToString("dd-MM-yyyy"),
                Ended = false,
            };

            var query = $@"
                INSERT INTO Challenge (startdate, enddate, description, ended) 
                VALUES (
                    '{challenge.StartDate}',
                    '{challenge.StartDate}',
                    '{challenge.Description}',
                    {Convert.ToInt32(challenge.Ended)}
                );";
            await connection.ExecuteAsync(query);

            var id = connection.ExecuteScalar<long>("SELECT last_insert_rowid();");
            challenge.Id = (int)id;

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

            var query = $"SELECT * FROM Challenge ORDER BY ended, enddate desc";

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
