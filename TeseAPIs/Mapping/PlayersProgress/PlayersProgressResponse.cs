using TeseAPIs.Mapping.PlayersProgress;

namespace TeseAPIs.Mapping.PlayerProgress
{
    public class PlayersProgressResponse
    {
        public required IEnumerable<PlayerProgressResponse> Items { get; init; } = [];
    }
}