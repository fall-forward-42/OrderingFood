using Microsoft.AspNetCore.Mvc;

namespace OrderingFood.Controllers
{
    public class FoodController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
