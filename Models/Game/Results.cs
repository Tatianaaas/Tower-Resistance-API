using System;
using System.Collections.Generic;

namespace JogoApi.Model.Game
{
    public class Results
    {

        public DateTime Date { get; set; }
        public string Winner { get; set; }
        public string GameTime { get; set; }
        public List<Match> Team1 { get; set; }

        public List<Match> Team2 { get; set; }

    }
}