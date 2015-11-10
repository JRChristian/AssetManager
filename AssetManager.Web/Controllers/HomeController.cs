using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;

namespace AssetManager.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : AssetManagerControllerBase
    {
        public ActionResult Index()
        {
            return View("~/App/Main/views/layout/layout.cshtml"); //Layout of the angular application.
        }
	}
}