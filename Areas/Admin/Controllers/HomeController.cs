using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderingFood.Data;
using OrderingFood.Models;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Xml.Linq;

namespace OrderingFood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly FoodieContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly User? currUser;

        public HomeController(FoodieContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        //render slider and header when 
        public IActionResult Index()
        {
            //check whether admin login
            var adminExist = Request.Cookies["AdminExist"];
            //if already login
            if (adminExist != null)
            {
                //set view name on profile
                ViewBag.Name = HttpContext.Session.GetString("NameCurrUser"); 
                //enable header and slider layout
                ViewBag.AdminExist = adminExist;
                //render home with header and slider
                return View();
            }
            //yet login
            else
            {
                return RedirectToAction("Login");

            }



        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AdminExist");
            return Redirect("Login");
        }
        public IActionResult Register()
        {
           
            return View();
        }
        
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,Mobile,Address,Email,Password,TypeAccount")] User _user)
        {

            if (ModelState.IsValid)
            {
                 var check =  _context.Users.FirstOrDefault(s => s.Email == _user.Email);

                 //not email exist - valid
                 if (check == null)
                {
                    _user.UserId = Guid.NewGuid();
                    _user.CreatedDate = DateTime.Now;
                    _user.TypeAccount = "Admin";
                    //add new user in db
                    _context.Users.Add(_user);
                    await _context.SaveChangesAsync();

                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddSeconds(3000);
                    Response.Cookies.Append("AdminExist", "True", options);

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Email đã tồn tại";
                    return View();

                }




            }
            return View();
        }



        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] User _user)
        {

            /*if (ModelState.IsValid)
            {*/
                //find the admin in db
                var data = await _context.Users.FirstOrDefaultAsync(s => s.Email!.Equals(_user.Email) && s.Password!.Equals(_user.Password) && s.TypeAccount!.Equals("Admin"));
            //if admin exist
                if (data != null)
                {

                HttpContext.Session.SetString("NameCurrUser", data.Name);
                    HttpContext.Session.SetString("IdCurrUser", data!.UserId.ToString());

                CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddSeconds(3000);
                    Response.Cookies.Append("AdminExist", "True", options);

                    //return to home with already login
                    return RedirectToAction("Index");
               }
                else
                {
                    ViewBag.error = "Tài khoản hoặc mật khẩu không đúng !";
                    return View();
                }
                    
                    




          /*  }
            return View();*/
        }
        
    }
}
