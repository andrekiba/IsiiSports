using System.Threading.Tasks;
using IsiiSports.Auth;

namespace IsiiSports.Interfaces
{
    public interface IAzureService
    {
        IPlayerStore PlayerStore { get; }
        ITeamStore TeamStore { get; }
        IGameStore GameStore { get; }
        IGameFieldStore GameFieldStore { get; }
        IGameResultStore GameResultStore { get; }

        bool IsInitialized { get; }
        Task InitializeAsync();
        Task<bool> SyncAllAsync();
        Task DropEverythingAsync();
		Task<bool> LoginAsync(string authProvider = null);
    }
}
