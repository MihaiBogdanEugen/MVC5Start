using System.Web.Mvc;

namespace MVC5Start.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous, HttpGet, Route("~/", Name = "home")]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous, HttpGet, Route("~/about", Name = "about")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous, HttpGet, Route("~/contact", Name = "contact")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}