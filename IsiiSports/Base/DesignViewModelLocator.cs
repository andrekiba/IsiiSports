﻿using System;
using IsiiSports.ViewModels;

namespace IsiiSports.Base
{
	public static class DesignViewModelLocator
	{
	    private static LoginViewModel loginViewModel;
		public static LoginViewModel LoginViewModel => loginViewModel ?? (loginViewModel = new LoginViewModel());
	    private static GamesViewModel gamesViewModel;
		public static GamesViewModel GamesViewModel => gamesViewModel ?? (gamesViewModel = new GamesViewModel());
	    private static TeamsViewModel teamsViewModel;
		public static TeamsViewModel TeamsViewModel => teamsViewModel ?? (teamsViewModel = new TeamsViewModel());
        private static InfoViewModel infoViewModel;
        public static InfoViewModel InfoViewModel => infoViewModel ?? (infoViewModel = new InfoViewModel());
    }
}
