using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
	public class MedicalTestsController : Controller
	{
		public async Task<IActionResult> Index()
		{
			return View();
		}
		public async Task<IActionResult> Create()
		{
			return View();
		}
	}
}
