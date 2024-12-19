using TeseAPIs.Mapping.PlayerProgress;
using TeseAPIs.Mapping.PlayersProgress;
using TeseAPIs.Models.ProgressResponse;

namespace TeseAPIs.Services
{
    public interface IStudentProgressService
    {
        Task<PlayerProgressResponse?> GetByIdAsync(string studentId);
        Task<IEnumerable<PlayerProgressResponse>?> GetAllAsync();
        Task<PlayerProgressResponse> UpdateCreditsByIdAsync(string studentId, int sumCredits);
        Task<PlayerProgressResponse> UpdateTutorialByIdAsync(string id);
        Task<PlayerProgressResponse> UpdateAchievementsByIdAsync(string id, AchievementResponse achievementDto);
        Task<PlayerProgressResponse> UpdateModulesByIdAsync(string id, ModuleResponse moduleDto);
        Task<PlayerProgressResponse> UpdateDayStreakByIdAsync(string id, DayStreakResponse dayStreakDto);
        Task<PlayerProgressResponse> UpdateDayByIdAsync(string id, DayResponse dayDto);
    }
}