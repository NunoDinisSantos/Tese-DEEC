namespace TeseAPIs.Services.Helper
{
    public class GetQueryForChallenges()
    {
        public string GetProgressQueryForChallenge(int challengeType)
        {
            string query = challengeType switch
            {
                0 => "SELECT * FROM ChallengeProgress ORDER BY coins DESC LIMIT 3",

                1 => "SELECT * FROM ChallengeProgress ORDER BY credits DESC LIMIT 3",

                2 => "SELECT * FROM ChallengeProgress ORDER BY fishcaught DESC LIMIT 3",

                3 => "SELECT *, (coins + fishcaught) AS total FROM ChallengeProgress ORDER BY total DESC LIMIT 3",

                4 => "SELECT *, (coins + credits) AS total FROM ChallengeProgress ORDER BY total DESC LIMIT 3",

                5 => "SELECT *, (credits + fishcaught) AS total FROM ChallengeProgress ORDER BY total DESC LIMIT 3",

                6 => "SELECT *, (coins + credits + fishcaught) AS total FROM ChallengeProgress ORDER BY total DESC LIMIT 3",

                7 => $@"
                    SELECT * FROM ChallengeProgress 
                    ORDER BY coins DESC LIMIT 3",

                8 => $@"
                    SELECT * FROM ChallengeProgress 
                    ORDER BY credits DESC LIMIT 3",

                9 => $@"
                    SELECT * FROM ChallengeProgress 
                    ORDER BY fishcaught DESC LIMIT 3",

                10 => $@"
                    SELECT * FROM ChallengeProgress 
                    ORDER BY (coins + credits) DESC LIMIT 3",

                11 => $@"
                    SELECT * FROM ChallengeProgress 
                    ORDER BY (coins + fishcaught) DESC LIMIT 3",

                12 => $@"
                    SELECT * FROM ChallengeProgress 
                    ORDER BY (credits + fishcaught) DESC LIMIT 3",

                13 => $@"
                    SELECT * FROM ChallengeProgress 
                    ORDER BY (coins + credits + fishcaught) DESC LIMIT 3",

                _ => throw new ArgumentException("Unknown challenge type")
            };

            return query;
        }
    }
}
