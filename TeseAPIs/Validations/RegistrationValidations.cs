using Dapper;
using TeseAPIs.Data;

namespace TeseAPIs.Validations
{
    public class RegistrationValidations(IDbConnectionFactory connectionFactory) : IRegistrationValidations
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public async Task<bool> VerifyIfCanRegister(string studentId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = connection.QuerySingleOrDefault($"SELECT player_id FROM MisteriosAquaticos WHERE player_id = {studentId} LIMIT 1");

            return result == null;
        }
    }
}