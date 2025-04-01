using TeseAPIs.Mapping.PlayersProgress;
using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public interface IStudentService
    {
        Task<PlayerProgressResponse> CreateAsync(RegistrationData studentId);
    }
}