using Dapper;
using TeseAPIs.Data;

namespace TeseAPIs.Validations
{
    public class RegistrationValidations : IRegistrationValidations
    {
        private IDbConnectionFactory _connectionFactory;

        public RegistrationValidations(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> VerifyIfCanRegister(string studentId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = connection.QuerySingleOrDefault($"SELECT * FROM MisteriosAquaticos WHERE PlayerId = {studentId} LIMIT 1");

            if (result == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}