using TeseAPIs.Models;

namespace TeseAPIs.Services
{
    public interface IStudentProgressService
    {
        public Task<PlayerProgress?> GetByIdAsync(string studentId);
        public Task<IEnumerable<PlayerProgress>?> GetAllAsync();
        public Task<bool?> UpdateCreditsByIdAsync(string studentId, int sumCredits);
    }
}