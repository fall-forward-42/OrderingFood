using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderingFood.Data;
using OrderingFood.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OrderingFood.Data;
using OrderingFood.Interfaces;
using OrderingFood.Models;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace OrderingFood.Controllers
{
    public class FoodController : Controller
    {
        private readonly FoodieContext _context;
        readonly INotyfService _notyf;

        public FoodController(FoodieContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }


        public async Task<IActionResult> Index(Guid? id)
        {
            if(id != null)
            {
                var productsInCate = await _context.Products.Include(p => p.Categories).Where(c => c.CategoryId == id).ToListAsync();
                ViewData["CateList"] = await _context.Categories.ToListAsync();
                return View(productsInCate);
            }
            var products = await _context.Products.Include(p => p.Categories).ToListAsync();
            ViewData["CateList"] = await _context.Categories.ToListAsync();
            return View(products);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetProductToCart(Guid? productId)
        {
            if(productId  == null)
            {
                return NotFound();
            }
            //get id customer cookie to render name of customer
            if (Request.Cookies["IdCustomerClient"] == null)
            {
                return NotFound();
            }
            //var idCustomer = new Guid(Request.Cookies["IdCustomer"]!);
                Cart cart = new Cart();
                var idCustomer = new Guid(Request.Cookies["IdCustomerClient"]!);

                cart.ProductId = productId;
                cart.CartId = Guid.NewGuid();
                cart.Quantity = 1;
                cart.UserId = idCustomer;
                cart.Status = "Chưa mua";


                _context.Add(cart);
                await _context.SaveChangesAsync();

                _notyf.Success("thêm sản phẩm thành công, cảm ơn đã chọn lựa!");
                return RedirectToAction("Index");
            }

    }
}
