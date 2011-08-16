using System.Web.Mvc;
using Raven.Client;
using RavenDB_Sample.Models;

namespace RavenDB_Sample.Controllers
{
    public class RavenController : Controller
    {
        public IDocumentSession DocumentSession { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DocumentSession = MvcApplication.Store.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            using(DocumentSession)
            {
                if (filterContext.Exception == null && DocumentSession != null)
                    DocumentSession.SaveChanges();
            }
        }
    }
}