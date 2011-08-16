using System;
using System.Collections.Generic;

namespace RavenDB_Sample.Models
{
    public class Team
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public int Age { get; set; }
        public List<Player> Players { get; set; }
        public TheManager Manager { get; set; }

        public string LeagueId { get; set; }

        public class TheManager
        {
            public string Name { get; set; }
            public bool Certified { get; set; }
        }


        public class Player
        {
            public string Name { get; set; }
            public DateTime Birthday { get; set; }
        }
    }
}