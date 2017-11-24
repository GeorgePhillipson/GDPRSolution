using System.Linq;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Web.Core.Model;
using Umbraco.Core;


namespace Web.Core.Controllers
{
    public class PrivateInfoSurfaceController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult Index()
        {
            return PartialView("~/Views/Partials/pvPrivateInfo.cshtml");
        }

        [HttpPost]
        public ActionResult PrivateInfoSubmit(ExampleViewModel model)
        {
            var contentService = ApplicationContext.Current.Services.ContentService;
            var allEventsNode = GetAllEventsNode();

            var content = contentService.CreateContent("Page Title", allEventsNode.Id, "example");
            content.SetValue("firstName", model.FirstName);
            contentService.SaveAndPublishWithStatus(content);

            return CurrentUmbracoPage();
        }

        private IPublishedContent GetAllEventsNode()
        {
            return Umbraco.TypedContentAtRoot().DescendantsOrSelf("example").FirstOrDefault();
        }
    }
}
