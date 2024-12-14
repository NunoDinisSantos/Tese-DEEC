using TeseAPIs.Mapping.PlayersProgress;

namespace TeseAPIs.Services
{
    public interface IStudentService
    {
        Task<PlayerProgressResponse> CreateAsync(string studentId);
    }
}