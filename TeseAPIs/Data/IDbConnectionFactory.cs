using System.Data;

namespace TeseAPIs.Data
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}