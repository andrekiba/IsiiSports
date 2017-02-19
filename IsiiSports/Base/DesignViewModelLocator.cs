using System;
using IsiiSports.ViewModels;

namespace IsiiSports.Base
{
	public static class DesignViewModelLocator
	{
		static LoginViewModel loginViewModel;
		public static LoginViewModel LoginViewModel => loginViewModel ?? (loginViewModel = new LoginViewModel());
		static GamesViewModel gamesViewModel;
		public static GamesViewModel GamesViewModel => gamesViewModel ?? (gamesViewModel = new GamesViewModel());
		static TeamsViewModel teamsViewModel;
		public static TeamsViewModel TeamsViewModel => teamsViewModel ?? (teamsViewModel = new TeamsViewModel());
		static GameViewModel gameViewModel;
		public static GameViewModel GameViewModel => gameViewModel ?? (gameViewModel = new GameViewModel());
		static TeamViewModel teamViewModel;
		public static TeamViewModel TeamViewModel => teamViewModel ?? (teamViewModel = new TeamViewModel());
	}
}
