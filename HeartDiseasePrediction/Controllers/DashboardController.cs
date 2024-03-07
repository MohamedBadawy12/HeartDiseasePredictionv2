using Microsoft.AspNetCore.Mvc;

namespace HeartDiseasePrediction.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
