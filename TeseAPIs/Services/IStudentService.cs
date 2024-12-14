namespace TeseAPIs.Services
{
    public interface IStudentService
    {
        Task<bool> CreateAsync(string studentId);
    }
}