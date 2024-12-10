using TeseAPIs.Mapping.PlayerProgress;
using TeseAPIs.Mapping.PlayersProgress;

namespace TeseAPIs.Mapping
{
    public static class MapResponse
    {
        public static PlayerProgressResponse MapToResponse(this Models.PlayerProgress student)
        {
            return new PlayerProgressResponse
            {
                PlayerId = student.PlayerId,
                TempoDeJogo = student.TempoDeJogo,
                Moedas = student.Moedas,
                PeixesApanhados = student.PeixesApanhados,
                Tutorial = student.Tutorial,
                Lanterna = student.Lanterna,
                ModuloProfundidade = student.ModuloProfundidade,
                ModuloTemperatura = student.ModuloTemperatura,
                ModuloReel = student.ModuloReel,
                ModuloStorage = student.ModuloStorage,
                Days = student.Days,
                ObjectosRaros = student.ObjectosRaros,
                LastLogin = student.LastLogin,
                DayStreak = student.DayStreak,
                Creditos = student.Creditos,
            };
        }

        public static PlayersProgressResponse MapToResponse(this IEnumerable<Models.PlayerProgress> student)
        {
            return new PlayersProgressResponse
            {
                Items = student.Select(MapToResponse)
            };
        }
    }
}