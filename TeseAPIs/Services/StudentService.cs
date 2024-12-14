using Dapper;
using TeseAPIs.Data;
using TeseAPIs.Mapping.PlayersProgress;
using TeseAPIs.Validations;

namespace TeseAPIs.Services
{
    public class StudentService(IDbConnectionFactory connectionFactory, IRegistrationValidations registrationValidations) : IStudentService
    {
        public async Task<PlayerProgressResponse> CreateAsync(string studentId)
        {
            var canRegister = await registrationValidations.VerifyIfCanRegister(studentId);

            if (canRegister)
            {
                using var connection = await connectionFactory.CreateConnectionAsync();
                var dateNow = DateTime.Now;

                var query = $"INSERT INTO MisteriosAquaticos" +
                    $" VALUES('{studentId}',0,0,0,0,0,0,0,0,0,0,'{dateNow.ToString("yyyy-MM-dd HH:mm:ss")}',0,0,0)";

                var result = await connection.ExecuteAsync(query);

                query = $"INSERT INTO Achievements" +
                    $" VALUES ('{studentId}',0,0,0,0,0,0)";

                result = await connection.ExecuteAsync(query);

                return new PlayerProgressResponse()
                {
                    PlayerId = studentId,
                    TimePlayed = 0,
                    Coins = 0,
                    FishCaught = 0,
                    Tutorial = 0,
                    Flashlight = 0,
                    DepthModule = 0,
                    TempModule = 0,
                    StorageModule = 0,
                    ReelModule = 0,
                    Days = 0,
                    RareObjects = 0,
                    LastLogin = dateNow,
                    DayStreak = 0,
                    Credits = 0,
                    Treasure = 0,
                    AncientCoral = 0,
                    LostResearch = 0,
                    TempleJewel = 0,
                    BoatJewel = 0,
                    OldIce = 0
                };
            }

            else
            {
                return new PlayerProgressResponse()
                {
                    PlayerId = "ERROR"
                };
            }
        }
    }
}