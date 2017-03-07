using System.Threading.Tasks;
using IsiiSports.DataObjects;

namespace IsiiSports.Interfaces
{
	public interface IPlayerStore : IBaseStore<Player>
    {
		Task<Player> GetPlayerByMail(string email);
	}
}
