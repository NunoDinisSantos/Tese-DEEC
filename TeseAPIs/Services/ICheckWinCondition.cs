namespace TeseAPIs.Services
{
    public interface ICheckWinCondition
    {
        public Task<bool> Check();
    }
}