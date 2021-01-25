using System;

namespace JogoApi.Model.Game
{
    public class MatchHistory
    {

        public int Id { get; set; }
        public int IdUser { get; set; }
        public DateTime Date { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int TowersDestroyed { get; set; }
        public TimeSpan GameTime { get; set; }
        public bool Result { get; set; }

       
    }
    
}

namespace JogoApi.Model.Game
{
    public class Match
    {
        public string Username { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int TowersDestroyed { get; set; }
       
    }
    
}