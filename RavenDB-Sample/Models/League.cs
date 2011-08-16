using System;
using System.Collections.Generic;

namespace RavenDB_Sample.Models
{
    public class League
    {
        public string LeagueId { get; set; }
        public string Name { get; set; }

        public List<string> TeamIds { get; set; }

        public List<Game> Games { get; set; }

        public class Game
        {
            public string TeamA { get; set; }
            public string TeamB { get; set; }
            public DateTime Date { get; set; }
        }
    }
}