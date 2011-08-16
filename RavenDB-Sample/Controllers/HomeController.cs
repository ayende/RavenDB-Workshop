using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RavenDB_Sample.Models;
using Raven.Client.Linq;
using System.Linq;

namespace RavenDB_Sample.Controllers
{
    public class HomeController : RavenController
    {
        public ActionResult BarsSearch(string song)
        {
            var list = DocumentSession.Advanced.LuceneQuery<Bar, Bars_Search>()
                .WithinRadiusOf(1, 35.0456297, -85.3096801)
                .WhereEquals("Songs",song)
                .OpenSubclause()
                    .WhereGreaterThan("OpensAt", DateTime.Now.TimeOfDay)
                    .AndAlso()
                    .WhereLessThan("ClosesAt", DateTime.Now.TimeOfDay)
                .CloseSubclause()
                .ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Bars()
        {
            DocumentSession.Store(new Bar
                                      {
                                          Lat = 35.049181266699335,
                                          Lng = -85.30660629272461,
                                          Name = "Choo Choo, maybe",
                                          ClosesAt = new TimeSpan(20,30,00),
                                          OpensAt = new TimeSpan(08, 30,00),
                                          Songs = new[]
                                                      {
                                                          "New York New York",
                                                          "I have a dream",
                                                          "One upon a time",
                                                          "Blue bird",
                                                          "Wicked",
                                                      }

                                      });

            DocumentSession.Store(new Bar
            {
                Lat = 35.045457078508896,
                Lng = -85.26167392730713,
                Name = "Choo Choo, probably not",
                ClosesAt = new TimeSpan(20, 30, 00),
                OpensAt = new TimeSpan(08, 30, 00),
                Songs = new[]
                                                      {
                                                          "New York New York",
                                                          "I have a dream",
                                                          "One upon a time",
                                                          "Blue bird",
                                                          "Wicked",
                                                      }

            });
            return Content("");
        }

        public ActionResult LazilyDownTheStream()
        {
            var lazyTeams = DocumentSession.Advanced.Lazily.Load<Team>("teams/1", "teams/2");
            var lazyQuery = DocumentSession.Query<Team>()
                .Lazily();

            var lazyQuery2 = DocumentSession.Query<League>()
               .Lazily();

            DocumentSession.Advanced.Eagerly.ExecuteAllPendingLazyOperations();


            return Content("OK");
        }
        public ActionResult Modify()
        {
            Team entity;
            using (var s1 = MvcApplication.Store.OpenSession())
            {
                entity = new Team
                             {
                                 Name = "Ayende"
                             };
                s1.Store(new Team
                             {
                                 Id = Request.QueryString["id"],
                                 Name = "Rahien"
                             });
                s1.Store(entity);
                s1.SaveChanges();
            }


            var load = DocumentSession.Load<Team>(entity.Id);
            RavenQueryStatistics stats;
            var array = DocumentSession.Query<Team>()
                .Statistics(out stats).Where(x=>x.Name == "Ayende")
                .ToArray();

            return Content("OK");
        }

        public ActionResult Named()
        {
            var results = from team in DocumentSession.Query<object>()
                          select team;

            return Json(results, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult Querying()
        {
            var results = from team in DocumentSession.Query<Team>()
                          where team.Players.Any(player => player.Name == "Westin")
                          select team;

            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShowGames(string league)
        {
            var l = DocumentSession
                .Include<League>(x=>x.TeamIds)
                .Load(league);


            return Json(l.Games.Select(x => new
                                                {
                                                    x.Date,
                                                    TeamA_Name =
                                                x.TeamA != null ? DocumentSession.Load<Team>(x.TeamA).Name : "undecided",
                                                    TeamB_Name =
                                                x.TeamB != null ? DocumentSession.Load<Team>(x.TeamB).Name : "undecided"
                                                }), JsonRequestBehavior.AllowGet);

        }

        public ActionResult CreateLeauge()
        {
            DocumentSession.Store(new League
                                      {
                                          Name = "East Seattle",
                                          Games = new List<League.Game>
                                                      {
                                                          new League.Game
                                                              {
                                                                  Date = DateTime.Now,
                                                                  TeamA = "teams/1",
                                                                  TeamB = "teams/2"
                                                              },
                                                              new League.Game
                                                                  {
                                                                      Date = DateTime.Now.AddDays(5)
                                                                  }
                                                      },
                                          TeamIds = new List<string>
                                                                    {
                                                                        "teams/1",
                                                                        "teams/2"
                                                                    }
                                      });

            return Content("Success");
        }
        public ActionResult CreateTeam()
        {
            DocumentSession.Store(new Team
                                      {
                                          Age = 12,
                                          LeagueId = "leagues/east-seatle",
                                          LogoUrl = "/leages/images/east-seatle.png",
                                          Manager = new Team.TheManager
                                                        {
                                                            Certified = true,
                                                            Name = "Jackson"
                                                        },
                                          Name = "East Seattle",
                                          Players = new List<Team.Player>
                                                        {
                                                            new Team.Player
                                                                {
                                                                    Name = "Westin",
                                                                    Birthday = DateTime.Now
                                                                },
                                                            new Team.Player
                                                                {
                                                                    Name = "Travis"
                                                                },
                                                            new Team.Player
                                                                {
                                                                    Name = "John"
                                                                }
                                                        }
                                      });

            return Content("Success");
        }
    }
}