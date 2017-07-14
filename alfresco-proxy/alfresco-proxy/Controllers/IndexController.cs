using Microsoft.AspNetCore.Mvc;

namespace AlfrescoProxy.Controllers
{
    [Route("/")]
    [Route("api")]
    public class IndexController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
