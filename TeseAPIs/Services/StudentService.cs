using Dapper;
using TeseAPIs.Data;
using TeseAPIs.Validations;

namespace TeseAPIs.Services
{
    public class StudentService(IDbConnectionFactory connectionFactory, IRegistrationValidations registrationValidations) : IStudentService
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
        private readonly IRegistrationValidations _registrationValidations = registrationValidations;

        public async Task<bool> CreateAsync(string studentId)
        {
            var canRegister = await _registrationValidations.VerifyIfCanRegister(studentId);

            if (canRegister)
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var dateNow = DateTime.Now;

                var query = $"INSERT INTO MisteriosAquaticos" +
                    $" VALUES('{studentId}',0,0,0,0,0,0,0,0,0,0,'{dateNow.ToString("yyyy-MM-dd HH:mm:ss")}',0,0,0)";

                var result = await connection.ExecuteAsync(query);

                query = $"INSERT INTO Achievements" +
                    $" VALUES ('{studentId}',0,0,0,0,0,0)";

                result = await connection.ExecuteAsync(query);

                return true;
            }

            else
            {
                return false;
            }
        }
    }
}