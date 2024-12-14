using Dapper;
using TeseAPIs.Data;
using TeseAPIs.Mapping.PlayersProgress;
using TeseAPIs.Validations;

namespace TeseAPIs.Services
{
    public class StudentProgressService(IDbConnectionFactory connectionFactory, ICreditValidations creditValidations) : IStudentProgressService
    {
        public async Task<IEnumerable<PlayerProgressResponse>?> GetAllAsync()
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            var query = $"SELECT " +
                $"ma.player_id AS PlayerId," +
                $"ma.time_played AS TimePlayed," +
                $"ma.tutorial AS Tutorial," +
                $"ma.coins AS Coins," +
                $"ma.fish_caught AS FishCaught," +
                $"ma.flashlight AS Flashlight," +
                $"ma.depth_module AS DepthModule," +
                $"ma.reel_module AS ReelModule," +
                $"ma.storage_module AS StorageModule," +
                $"ma.temp_module AS TempModule," +
                $"ma.days AS Days," +
                $"ma.rare_objects AS RareObjects," +
                $"ma.last_login AS LastLogin," +
                $"ma.days_streak AS DaysStreak," +
                $"ma.credits AS Credits," +
                $"ach.treasure AS Treasure," +
                $"ach.ancient_coral AS AncientCoral," +
                $"ach.lost_research AS LostResearch," +
                $"ach.temple_jewel AS TempleJewel," +
                $"ach.boat_jewel AS BoatJewel," +
                $"ach.old_ice AS OldIce" +
                $" FROM MisteriosAquaticos ma" +
                $" LEFT JOIN Achievements ach ON ma.player_id = ach.player_id";

            var result = await connection.QueryAsync<PlayerProgressResponse>(query);

            return result;
        }

        public async Task<PlayerProgressResponse?> GetByIdAsync(string studentId)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();
            var result = await connection.QuerySingleOrDefaultAsync<PlayerProgressResponse>(new CommandDefinition($"SELECT " +
                $"ma.player_id AS PlayerId," +
                $"ma.time_played AS TimePlayed," +
                $"ma.tutorial AS Tutorial," +
                $"ma.coins AS Coins," +
                $"ma.fish_caught AS FishCaught," +
                $"ma.flashlight AS Flashlight," +
                $"ma.depth_module AS DepthModule," +
                $"ma.reel_module AS ReelModule," +
                $"ma.storage_module AS StorageModule," +
                $"ma.temp_module AS TempModule," +
                $"ma.days AS Days," +
                $"ma.rare_objects AS RareObjects," +
                $"ma.last_login AS LastLogin," +
                $"ma.days_streak AS DaysStreak," +
                $"ma.credits AS Credits," +
                $"ach.treasure AS Treasure," +
                $"ach.ancient_coral AS AncientCoral," +
                $"ach.lost_research AS LostResearch," +
                $"ach.temple_jewel AS TempleJewel," +
                $"ach.boat_jewel AS BoatJewel," +
                $"ach.old_ice AS OldIce" +
                $" FROM MisteriosAquaticos ma" +
                $" LEFT JOIN Achievements ach ON ma.player_id = ach.player_id"+
                $" WHERE ma.player_id = {studentId}"));

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<PlayerProgressResponse?> UpdateCreditsByIdAsync(string studentId, int sumCredits)
        {
            var studentProgress = await GetByIdAsync(studentId);

           if((studentProgress == null || !creditValidations.VerifyNoNegativeAmounts(studentProgress.Credits, sumCredits)))
           {
                return null;
           }

            studentProgress.Credits += sumCredits;

            using var connection = await connectionFactory.CreateConnectionAsync();
            var result = await connection.ExecuteAsync($"UPDATE MisteriosAquaticos SET credits = {studentProgress.Credits} WHERE player_id = {studentId}");

            return studentProgress;
        }
    }
}