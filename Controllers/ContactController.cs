using Microsoft.AspNetCore.Mvc;

namespace OrderingFood.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
    }
}
