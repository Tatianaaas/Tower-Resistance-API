using System;

namespace JogoApi.Model.Game
{
    public class Leagues
    {
        public int Id { get; set; }

        public int IdUser { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }

    }
}

namespace JogoApi.Model.Game
{
    public class LeaguesList
    {
        public string Username { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }

    }
}