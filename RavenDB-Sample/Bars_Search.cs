using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenDB_Sample.Models;

namespace RavenDB_Sample
{
    public class Bars_Search : AbstractIndexCreationTask<Bar>
    {
        public Bars_Search()
        {
            Map = bars =>
                  from bar in bars
                  select new
                             {
                                 bar.OpensAt,
                                 bar.ClosesAt,
                                 bar.Songs,
                                 _ = SpatialIndex.Generate(bar.Lat, bar.Lng)
                             };

            Indexes.Add(x => x.Songs, FieldIndexing.Analyzed);
        }
    }


    public class LeagueAnTeams : AbstractIndexCreationTask<League>
    {
        public LeagueAnTeams()
        {
            Map = leagues =>
                  from league in leagues
                  select new
                             {
                                 league.Name
                             };

            TransformResults = (database, leagues) =>
                               from league in leagues
                               let teams = database.Load<Team>(league.TeamIds)
                               select new
                                          {
                                              league.Name,
                                              Teams = teams.Select(x=>x.Name)
                                          };
        }
    }
}