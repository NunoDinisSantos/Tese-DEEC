namespace TeseAPIs.Services
{
    public interface IStudentService
    {
        public Task<bool> CreateAsync(string studentId);
    }
}