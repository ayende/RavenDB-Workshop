using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using RavenDB_Sample.Models;

namespace RavenDB_Sample
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static DocumentStore _store;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            InitializeRaven();
        }

        private void InitializeRaven()
        {
            _store = new DocumentStore
                         {
                             Url = "http://localhost:8080"
                         };

            _store.Conventions.FindIdentityProperty = info => info.Name == "Id" ||
                                                              info.DeclaringType.Name + "Id" == info.Name;

            _store.Initialize();

            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), _store);

            var defaultDocumentKeyGenerator = _store.Conventions.DocumentKeyGenerator;

            _store.Conventions.DocumentKeyGenerator = o =>
                                                          {
                                                              var league = o as League;
                                                              if (league == null)
                                                                  return defaultDocumentKeyGenerator(o);

                                                              return "leagues/" +
                                                                     league.Name.ToLowerInvariant().Replace(" ", "-");
                                                          };
        }

        public static IDocumentStore Store
        {
            get { return _store; }
        }
    }
}