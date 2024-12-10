using Dapper;
using TeseAPIs.Data;
using TeseAPIs.Models;
using TeseAPIs.Validations;

namespace TeseAPIs.Services
{
    public class StudentProgressService : IStudentProgressService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICreditValidations _creditValidations;

        public StudentProgressService(IDbConnectionFactory connectionFactory, ICreditValidations creditValidations)
        {
            _connectionFactory = connectionFactory;
            _creditValidations = creditValidations;
        }

        public async Task<IEnumerable<PlayerProgress>?> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var query = $"SELECT * FROM MisteriosAquaticos";
            var result = await connection.QueryAsync<PlayerProgress>(query);
 
            return result.Select(x => new PlayerProgress
            {
                PlayerId = x.PlayerId,
                TempoDeJogo = x.TempoDeJogo,
                Moedas = x.Moedas,
                PeixesApanhados = x.PeixesApanhados,
                Tutorial = x.Tutorial,
                Lanterna = x.Lanterna,
                ModuloProfundidade = x.ModuloProfundidade,
                ModuloReel = x.ModuloReel,
                ModuloStorage = x.ModuloStorage,
                ModuloTemperatura = x.ModuloTemperatura,
                Days = x.Days,
                ObjectosRaros = x.ObjectosRaros,
                LastLogin = x.LastLogin,
                DayStreak = x.DayStreak,
                Creditos = x.Creditos         
            }
            );
        }

        public async Task<PlayerProgress?> GetByIdAsync(string studentId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = await connection.QuerySingleOrDefaultAsync<PlayerProgress>(new CommandDefinition($"SELECT * FROM MisteriosAquaticos WHERE PlayerId = {studentId}"));

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<bool?> UpdateCreditsByIdAsync(string studentId, int sumCredits)
        {
            var studentProgress = await GetByIdAsync(studentId);

            if (studentProgress == null)
            {
                return false;
            }

           if(!_creditValidations.VerifyNoNegativeAmounts(studentProgress, sumCredits))
           {
                return false;
           }

            studentProgress.Creditos += sumCredits;

            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = await connection.ExecuteAsync($"UPDATE MisteriosAquaticos SET Creditos = {studentProgress.Creditos} WHERE PlayerId = {studentId}");

            return true;
        }
    }
}