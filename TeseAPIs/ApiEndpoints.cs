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
            public const string Update = $"{Base}/{{id:int}}";
            public const string GetAll = Base;
        }
    }
}