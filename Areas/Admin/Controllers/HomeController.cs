using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderingFood.Data;
using OrderingFood.Interfaces;
using OrderingFood.Models;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Security.Principal;

namespace OrderingFood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly FoodieContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly WebApplicationBuilder _builder;
        private readonly IJwtAuthenService _JwtAuthenService;//inject interface into this


        public HomeController(FoodieContext context, IHttpContextAccessor httpContextAccessor, IJwtAuthenService JwtAuthenService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _JwtAuthenService = JwtAuthenService;
        }
        //render slider and header when 
        public IActionResult Index()
        {
            //check whether admin login
            var adminExist = Request.Cookies["AdminExist"];
            //if already login
            if (adminExist != null)
            {
                //render home with header and slider
                return View();
            }
            //yet login
            else
            {
                return RedirectToAction("Login");

            }



        }
        //logout
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AdminExist");
            Response.Cookies.Delete("IdAdmin");
            Response.Cookies.Delete("NameAdmin");
            //remove cookie
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("Login");
        }
        [AllowAnonymous]

        //sign up
        public IActionResult Register()
        {
           
            return View();
        }
        [AllowAnonymous]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,Mobile,Address,Email,Password,TypeAccount")] User user)
        {
            //éo xài model binding được nên validate tay
            /*if(user.Name == null || user.Mobile == null || user.Address == null || user.Email == null || user.Password == null || user.TypeAccount == null)
            {
                ViewBag.error = "Vui lòng không để trống !";
                return View(); 
            }
            //check mobile
            if (user.Mobile.Length > 10)
            {
                ModelState.AddModelError("Mobile", "số điện thoại tối đa 10 số");
            }
            if (!Regex.IsMatch(user.Mobile, @"^\d+$"))
            {
                ModelState.AddModelError("Mobile", "Số điện thoại không hợp lệ");
            }
            //check password
            if (user.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Mật khẩu tối thiểu 6 ký tự");
            }*/
            

            var check =  _context.Users.FirstOrDefault(s => s.Email == user.Email);

                 //not email exist - valid
                 if (check == null)
                {
                    user.UserId = Guid.NewGuid();
                    user.CreatedDate = DateTime.Now;
                    user.ImageUrl = "Default.png";
                   
                    
                    //check admin code 
                    if (user.TypeAccount=="newadmin")
                    {
                        user.TypeAccount = "Admin";
                        //add new user in db
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                         ModelState.AddModelError("TypeAccount", "Mã cấp quyền không chính xác !");
                        return View();
                    }
                    
                    
                    //sign up successfully
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View();

                }

        



        }
        [AllowAnonymous]

        //login
        public IActionResult Login()
        {

            return View();
        }
        [AllowAnonymous]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] User user)
        {
            if(user.Email == null || user.Password == null)
            {
                ViewBag.error = "Vui lòng không để trống !";
                return View();
            }
            //find the admin in db
            var thisUser = await _context.Users.FirstOrDefaultAsync(s => s.Email!.Equals(user.Email!)  && s.TypeAccount!.Equals("Admin") && s.Password!.Equals(user.Password!));
              
                //if admin exist
                if (thisUser != null)
                {
                    //authen by cookie
                    var claims = new List<Claim>
                    {
                         new Claim(ClaimTypes.Name, thisUser.Name),
                         new Claim(ClaimTypes.Role, thisUser.TypeAccount)
                    };

                    //Create the identity for the user
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme) ;
                       //sign the cookie
                    var principal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme, principal);




                //set cookie to render header and slider bar
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Append("AdminExist", "True", options);

                CookieOptions options2 = new CookieOptions();
                options2.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Append("NameAdmin", thisUser.Name!.ToString(), options2);

                CookieOptions options3 = new CookieOptions();
                options3.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Append("IdAdmin", thisUser.UserId.ToString(), options3);


                //return to home with already login
                return RedirectToAction("Index");
                        
                       

                
                }
                else
                {
                    ViewBag.error = "Tài khoản không đúng !";
                    Unauthorized();
                    return View();
                }
                    
                    




        }
        
    }
}
