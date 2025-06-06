﻿using Dapper;
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

            var startDate = DateTime.ParseExact(challengeDTO.StartDate, "dd-MM-yyyy", null);
            var endDate = DateTime.ParseExact(challengeDTO.EndDate, "dd-MM-yyyy", null);

            var challenge = new Challenge()
            {
                Description = challengeDTO.Description,
                StartDate = startDate.ToString("dd-MM-yyyy"),
                EndDate = endDate.ToString("dd-MM-yyyy"),
                Started = challengeDTO.Started,
                Ended = false,
                EventType = challengeDTO.EventType,
                QuantityXXX = challengeDTO.QuantityXXX,
                QuantityYYY = challengeDTO.QuantityYYY,
                QuantityZZZ = challengeDTO.QuantityZZZ
            };

            var query = $@"
                INSERT INTO Challenge (startdate, enddate, description, ended, eventtype, quantityx, quantityy, quantityz, started) 
                VALUES (
                    '{challenge.StartDate}',
                    '{challenge.EndDate}',
                    '{challenge.Description}',
                    {Convert.ToInt32(challenge.Ended)},
                    {challenge.EventType},
                    {challenge.QuantityXXX},
                    {challenge.QuantityYYY},
                    {challenge.QuantityZZZ},
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
                    QuantityXXX = challenge.QuantityXXX,
                    QuantityYYY = challenge.QuantityYYY,
                    QuantityZZZ = challenge.QuantityZZZ
                };

                return returnChallenge;
            }

            return new Challenge() { Id = -1 };
        }

        public async Task<Challenge> GetChallengeAsync()
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"SELECT * FROM Challenge WHERE ended = 0 and started = 1 order by startdate";

            var result = await connection.QueryAsync<Challenge>(query);

            if (result.Any())
            {
                var challenge = result.FirstOrDefault();
                return challenge!;
            }

            else
            {
                return new Challenge() { Id = -1 };
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

            var endDate = DateTime.ParseExact(challenge.EndDate, "dd-MM-yyyy", null).ToString("dd-MM-yyyy");

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
                    QuantityXXX = challenge.QuantityXXX,
                    QuantityYYY = challenge.QuantityYYY,
                    QuantityZZZ = challenge.QuantityZZZ
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
                    QuantityXXX = challenge.QuantityXXX,
                    QuantityYYY = challenge.QuantityYYY,
                    QuantityZZZ = challenge.QuantityZZZ
                };

                var startDate = DateTime.ParseExact(challenge.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);


                if (DateTime.Compare(DateTime.Now, startDate) > 0)
                {
                    await challengeManagerService.ValidateForChallenges(challenge, true);
                }

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

            var newStart = DateTime.ParseExact(challenge.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var newEnd = DateTime.ParseExact(challenge.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var existingChallenges = await connection.QueryAsync<Challenge>(
                "SELECT * FROM Challenge WHERE Ended = 0");

            foreach (var existing in existingChallenges)
            {
                var existingStart = DateTime.ParseExact(existing.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var existingEnd = DateTime.ParseExact(existing.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                if (existingStart < newEnd && existingEnd > newStart)
                {
                    return true; 
                }
            }

            return false;
        }
    }
}
