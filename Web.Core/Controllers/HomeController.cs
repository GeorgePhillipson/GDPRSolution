using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Web.Core.Controllers
{
    public class HomeController : RenderMvcController
    {
        //Username for login - mylogin@me.com
        //Password for login - MyPassword
        public ActionResult Home(RenderModel objModel)
        {
            return View("~/Views/Home.cshtml");
        }
    }
}
