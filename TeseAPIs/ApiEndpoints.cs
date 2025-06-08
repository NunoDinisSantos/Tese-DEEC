namespace TeseAPIs
{
    public class ApiEndpoints
    {
        private const string ApiBase = "api";

        public static class Tese
        {
            private const string Base = $"{ApiBase}/misteriosaquaticos";

            public const string Create = Base;
            public const string Get = $"{Base}/{{id:int}}";
            public const string UpdateCredits = $"{Base}/{{id:int}}/credits";
            public const string UpdateTutorial = $"{Base}/{{id:int}}/tutorial";
            public const string UpdateAchievements = $"{Base}/{{id:int}}/achievements";
            public const string UpdateDay = $"{Base}/{{id:int}}/day";
            public const string UpdateModules = $"{Base}/{{id:int}}/modules";
            public const string UpdateDayStreak = $"{Base}/{{id:int}}/daystreak";
            public const string Rewards = $"{Base}/rewards";
            public const string UpdateReward = $"{Base}/rewards/{{id:int}}";
            public const string Challenges = $"{Base}/challenges";
            public const string Challenge = $"{Base}/challenges/latest";
            public const string UpdateChallenge = $"{Base}/challenges/{{id:int}}";
            public const string EndChallenge = $"{Base}/challenges/end/{{id:int}}";
            public const string EndChallengeApp = $"{Base}/challenges/endapp/{{id:int}}";
            public const string ChallengeProgress = $"{Base}/challengeProgressEvent/{{eventType:int}}";
            public const string UpdateChallengeProgress = $"{Base}/challengeProgress/player";
            public const string ResetChallengeProgress = $"{Base}/challengeProgress/reset";
            public const string ChallengeWinners = $"{Base}/challengeWinners";
            public const string LastChallengeWinners = $"{Base}/lastChallengeWinners";
            public const string VerifyWin = $"{Base}/checkwin";
            public const string AbortChallenge = $"{Base}/challenges/abort";
            public const string CheckConflictChallengeDates = $"{Base}/challenges/conflicts";

            public const string GetAll = Base;
            
        }
    }
}