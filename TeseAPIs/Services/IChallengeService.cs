﻿using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public interface IChallengeService
    {
        Task<IEnumerable<Challenge>> GetChallengesAsync();

        Task<Challenge> GetChallengeAsync();

        Task<Challenge> UpdateChallengeById(Challenge challenge);

        Task<Challenge> EndChallengeById(Challenge challenge);

        Task<Challenge> EndChallengeByIdAPP(Challenge challenge);

        Task<Challenge> CreateChallenge(ChallengeDTO challengeDTO);

        Task<bool> AbortLatestChallenge();

        Task<bool> HasConflictsDates(ChallengeDTO challenge);
    }
}