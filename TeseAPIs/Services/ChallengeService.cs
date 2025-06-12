using Dapper;
using System.Globalization;
using TeseAPIs.Data;
using TeseAPIs.Models;
using TeseAPIs.Services.Helper;

namespace TeseAPIs.Services
{
    public class ChallengeService(IDbConnectionFactory connectionFactory, IChallengeManagerService challengeManagerService, IChallengeProgress challengeProgressService) : IChallengeService
    {
        public async Task<Challenge> CreateChallenge(ChallengeDTO challengeDTO)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var startDate = DateTime.Parse(challengeDTO.StartDate,CultureInfo.InvariantCulture);
            var endDate = DateTime.Parse(challengeDTO.EndDate, CultureInfo.InvariantCulture);

            var challenge = new Challenge()
            {
                Description = challengeDTO.Description,
                StartDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Started = challengeDTO.Started,
                Ended = false,
                EventType = challengeDTO.EventType,
                QuantityX = challengeDTO.QuantityX,
                QuantityY = challengeDTO.QuantityY,
                QuantityZ = challengeDTO.QuantityZ
            };

            var query = $@"
                INSERT INTO Challenge (startdate, enddate, description, ended, eventtype, quantityx, quantityy, quantityz, started) 
                VALUES (
                    '{challenge.StartDate}',
                    '{challenge.EndDate}',
                    '{challenge.Description}',
                    {Convert.ToInt32(challenge.Ended)},
                    {challenge.EventType},
                    {challenge.QuantityX},
                    {challenge.QuantityY},
                    {challenge.QuantityZ},
                    {Convert.ToInt32(challenge.Started)}
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
                    Started = challenge.Started,
                    Ended = true,
                    EventType = challenge.EventType,
                    QuantityX = challenge.QuantityX,
                    QuantityY = challenge.QuantityY,
                    QuantityZ = challenge.QuantityZ
                };

                return returnChallenge;
            }

            return new Challenge() { Id = -1 };
        }

        public async Task<Challenge> GetChallengeAsync()
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"UPDATE Challenge SET ended = 1 WHERE DATETIME('now') > DATETIME(enddate) and ended = 0"; // closes challenges
            int closed = await connection.ExecuteAsync(query);

            query = $"UPDATE Challenge SET started = 1 WHERE DATETIME('now') > DATETIME(startdate) and ended = 0"; // updates the challenges startdate

            await connection.ExecuteAsync(query);

            query = $"SELECT * FROM Challenge WHERE ended = 0 and started = 1 order by startdate";

            var result = await connection.QueryAsync<Challenge>(query);

            if (result.Any())
            {
                var challenge = result.FirstOrDefault();
                return challenge!;
            }

            else
            {
                return new Challenge() { Id = -1, EventType = -1 };
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

            var endDate = DateTime.Parse(challenge.EndDate, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            var query = $"UPDATE Challenge SET description = '{challenge.Description}', enddate = '{endDate}' WHERE id = {challenge.Id}";

            var result = await connection.ExecuteAsync(query);

            if (result > 0)
            {
                var returnChallenge = new Challenge()
                {
                    Id = challenge.Id,
                    Description = challenge.Description,
                    EndDate = challenge.EndDate,
                    EventType = challenge.EventType,
                    Started = challenge.Started,
                    QuantityX = challenge.QuantityX,
                    QuantityY = challenge.QuantityY,
                    QuantityZ = challenge.QuantityZ
                };

                return returnChallenge;
            }

            return new Challenge();
        }

        public async Task<Challenge> EndChallengeByIdAPP(Challenge challenge)
        {
            if (challenge.Ended)
            {
                return challenge;
            }

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
                    Started = challenge.Started,
                    Ended = true,
                    EventType = challenge.EventType,
                    QuantityX = challenge.QuantityX,
                    QuantityY = challenge.QuantityY,
                    QuantityZ = challenge.QuantityZ
                };

                //var startDate = DateTime.Parse(challenge.StartDate, CultureInfo.InvariantCulture);
                //if (DateTime.Compare(DateTime.Now, startDate) > 0)
                //{
                //}

                result = await challengeManagerService.ValidateForChallenges(challenge, true); // get winner
                await challengeProgressService.ResetChallengeProgress();


                return returnChallenge;
            }

            return new Challenge();
        }

        public async Task<bool> AbortLatestChallenge()
        {
            var latestChallenge = await GetChallengeAsync();
            latestChallenge.Ended = true;

            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"UPDATE Challenge SET ended = 1 WHERE id = {latestChallenge.Id}";

            var resultQuery = await connection.ExecuteAsync(query) > 0;

            bool result = await challengeProgressService.ResetChallengeProgress();

            return result && resultQuery;
        }

        public async Task<bool> HasConflictsDates(ChallengeDTO challenge)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var newStart = DateTime.Parse(challenge.StartDate, CultureInfo.InvariantCulture);
            var newEnd = DateTime.Parse(challenge.EndDate, CultureInfo.InvariantCulture);

            var existingChallenges = await connection.QueryAsync<Challenge>(
                "SELECT * FROM Challenge WHERE Ended = 0");

            foreach (var existing in existingChallenges)
            {
                var existingStart = DateTime.Parse(existing.StartDate, CultureInfo.InvariantCulture);
                var existingEnd = DateTime.Parse(existing.EndDate, CultureInfo.InvariantCulture);

                if (existingStart < newEnd && existingEnd > newStart)
                {
                    return true; 
                }
            }

            return false;
        }
    }
}
