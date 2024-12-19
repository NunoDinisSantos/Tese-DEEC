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

            public const string GetAll = Base;

        }
    }
}