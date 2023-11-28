using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderingFood.Data;
using OrderingFood.Models;
using System.Security.Claims;

namespace OrderingFood.Controllers
{

    public class AccountController : Controller
    {
        private readonly FoodieContext _context;
        readonly INotyfService _notyf;
        public AccountController(FoodieContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }
        //logout
        public IActionResult Logout()
        {
            Response.Cookies.Delete("IdCustomerClient");
            Response.Cookies.Delete("NameCustomerClient");
            //remove cookie
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("Login");
        }
        [AllowAnonymous]
        public IActionResult Login()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] User user)
        {
            if (user.Email == null || user.Password == null)
            {
                ViewBag.error = "Vui lòng không để trống !";
                return View();
            }
            //find the admin in db
            var thisUser = await _context.Users.FirstOrDefaultAsync(s => s.Email!.Equals(user.Email!) && s.TypeAccount!.Equals("Customer") && s.Password!.Equals(user.Password!));

            //if admin exist
            if (thisUser != null)
            {
                //authen by cookie
                var claimsClient = new List<Claim>
                    {
                         new Claim(ClaimTypes.Name, thisUser.Name),
                         new Claim(ClaimTypes.Role, thisUser.TypeAccount)
                    };

                //Create the identity for the user
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claimsClient, CookieAuthenticationDefaults.AuthenticationScheme);
                //sign the cookie
                var principal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

             
                CookieOptions options3 = new CookieOptions();
                options3.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Append("IdCustomerClient", thisUser.UserId.ToString(), options3);

                CookieOptions options4 = new CookieOptions();
                options4.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Append("NameCustomerClient", thisUser.Name.ToString(), options4);

                CookieOptions options5 = new CookieOptions();
                options5.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Append("IdAdminClient", "e3756653-5826-4f48-8072-efc61c0beb76", options5);


                //return to home with already login
                _notyf.Success("Đăng nhập thành công");
                return RedirectToAction("Index", "Food");
            }
            else
            {
                ViewBag.error = "Tài khoản không đúng !";
                Unauthorized();
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult Register()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,Mobile,Address,Email,Password")] User user)
        {

            
            var check = _context.Users.FirstOrDefault(s => s.Email == user.Email);

            //not email exist - valid
            if (check == null)
            {
                user.UserId = Guid.NewGuid();
                user.CreatedDate = DateTime.Now;
                user.ImageUrl = "Default.png";
                user.TypeAccount = "Customer";
                    //add new user in db



                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();



                //sign up successfully
                _notyf.Success("Đăng ký thành công !");
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View();

            }





        }
    }
}
