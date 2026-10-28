using Microsoft.AspNetCore.Mvc;

namespace SubscriberService.Controllers
{
    public class SubscriberServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
