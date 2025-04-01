using Dapper;
using TeseAPIs.Data;
using TeseAPIs.Mapping.PlayersProgress;
using TeseAPIs.Models.ProgressResponse;
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
                $"ma.nick_name AS StudentNick," +
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
                $"strftime('%Y-%m-%d %H:%M:%S', ma.last_login) AS LastLogin," +
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
                $"ma.nick_name AS StudentNick," +
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
                $"strftime('%Y-%m-%d %H:%M:%S', ma.last_login) AS LastLogin," +
                $"ma.days_streak AS DaysStreak," +
                $"ma.credits AS Credits," +
                $"ach.treasure AS Treasure," +
                $"ach.ancient_coral AS AncientCoral," +
                $"ach.lost_research AS LostResearch," +
                $"ach.temple_jewel AS TempleJewel," +
                $"ach.boat_jewel AS BoatJewel," +
                $"ach.old_ice AS OldIce" +
                $" FROM MisteriosAquaticos ma" +
                $" LEFT JOIN Achievements ach ON ma.player_id = ach.player_id" +
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

            if ((studentProgress == null || !creditValidations.VerifyNoNegativeAmounts(studentProgress.Credits, sumCredits)))
            {
                return null;
            }

            studentProgress.Credits += sumCredits;

            using var connection = await connectionFactory.CreateConnectionAsync();
            var result = await connection.ExecuteAsync($"UPDATE MisteriosAquaticos SET credits = {studentProgress.Credits} WHERE player_id = {studentId}");

            return studentProgress;
        }

        public async Task<PlayerProgressResponse?> UpdateTutorialByIdAsync(string studentId)
        {
            var studentProgress = await GetByIdAsync(studentId);

            if (studentProgress == null)
            {
                return null;
            }

            using var connection = await connectionFactory.CreateConnectionAsync();
            var resultMisteriosAquaticos = await connection.ExecuteAsync($"UPDATE MisteriosAquaticos SET" +
                $" tutorial = 1" +
                $" WHERE player_id = {studentProgress.PlayerId}");

            return studentProgress;
        }

        public async Task<PlayerProgressResponse?> UpdateAchievementsByIdAsync(string id, AchievementResponse achievProgress)
        {
            var studentProgress = await GetByIdAsync(id);

            if (studentProgress == null)
            {
                return null;
            }

            using var connection = await connectionFactory.CreateConnectionAsync();
            var resultAchievements = await connection.ExecuteAsync($"UPDATE Achievements SET" +
                $" treasure = {achievProgress.Treasure}," +
                $" ancient_coral = {achievProgress.AncientCoral}," +
                $" lost_research = {achievProgress.LostResearch}," +
                $" temple_jewel = {achievProgress.TempleJewel}," +
                $" boat_jewel = {achievProgress.BoatJewel}," +
                $" old_ice = {achievProgress.OldIce}" +
                $" WHERE player_id = {studentProgress.PlayerId}");

            studentProgress.Treasure = achievProgress.Treasure;
            studentProgress.AncientCoral = achievProgress.AncientCoral;
            studentProgress.LostResearch = achievProgress.LostResearch;
            studentProgress.TempleJewel = achievProgress.TempleJewel;
            studentProgress.BoatJewel = achievProgress.BoatJewel;
            studentProgress.OldIce = achievProgress.OldIce;

            return studentProgress;
        }

        public async Task<PlayerProgressResponse?> UpdateModulesByIdAsync(string id, ModuleResponse modulesProgress)
        {
            var studentProgress = await GetByIdAsync(id);

            if (studentProgress == null)
            {
                return null;
            }

            using var connection = await connectionFactory.CreateConnectionAsync();
            var resultMisteriosAquaticos = await connection.ExecuteAsync($"UPDATE MisteriosAquaticos SET" +
                $" flashlight = {modulesProgress.Flashlight}," +
                $" depth_module = {modulesProgress.DepthModule}," +
                $" temp_module = {modulesProgress.TempModule}," +
                $" storage_module = {modulesProgress.StorageModule}," +
                $" reel_module = {modulesProgress.ReelModule}," +
                $" coins = {modulesProgress.Coins}" +
                $" WHERE player_id = {studentProgress.PlayerId}");

            studentProgress.Flashlight = modulesProgress.Flashlight;
            studentProgress.DepthModule = modulesProgress.DepthModule;
            studentProgress.TempModule = modulesProgress.TempModule;
            studentProgress.StorageModule = modulesProgress.StorageModule;
            studentProgress.ReelModule = modulesProgress.ReelModule;

            return studentProgress;
        }

        public async Task<PlayerProgressResponse?> UpdateDayStreakByIdAsync(string id, DayStreakResponse dayStreakProgress)
        {
            var studentProgress = await GetByIdAsync(id);

            if (studentProgress == null)
            {
                return null;
            }

            using var connection = await connectionFactory.CreateConnectionAsync();
            var resultMisteriosAquaticos = await connection.ExecuteAsync($"UPDATE MisteriosAquaticos SET" +
                $" last_login = '{DateTime.Parse(dayStreakProgress.LastLogin).ToString("yyyy-MM-ddTHH:mm:ss")}'," +
                $" days_streak = {dayStreakProgress.DayStreak}" +
                $" WHERE player_id = {studentProgress.PlayerId}");

            studentProgress.LastLogin = DateTime.Parse(dayStreakProgress.LastLogin);
            studentProgress.DayStreak = dayStreakProgress.DayStreak;
 
            return studentProgress;
        }

        public async Task<PlayerProgressResponse?> UpdateDayByIdAsync(string id, DayResponse dayProgress)
        {
            var studentProgress = await GetByIdAsync(id);

            if (studentProgress == null)
            {
                return null;
            }

            using var connection = await connectionFactory.CreateConnectionAsync();
            var resultMisteriosAquaticos = await connection.ExecuteAsync($"UPDATE MisteriosAquaticos SET" +
                $" fish_caught = {dayProgress.FishCaught}," +
                $" credits = {dayProgress.Credits}," +
                $" days = {dayProgress.Days}," +
                $" coins = {dayProgress.Coins}," +
                $" time_played = {dayProgress.TimePlayed}" +
                $" WHERE player_id = {studentProgress.PlayerId}");

            studentProgress.FishCaught = dayProgress.FishCaught;
            studentProgress.Coins = dayProgress.Coins;
            studentProgress.Days = dayProgress.Days;
            studentProgress.Credits = dayProgress.Credits;

            return studentProgress;
        }
    }
}