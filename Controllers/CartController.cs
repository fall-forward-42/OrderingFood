using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using OrderingFood.Data;

namespace OrderingFood.Controllers
{
    public class CartController : Controller
    {
        private readonly FoodieContext _context;
        readonly INotyfService _notyf;


        public CartController(FoodieContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            var idCustomer = new Guid("374c3bc7-e148-4252-8cd9-9dbdea3352b0");

            var productInCart = await _context.Carts.Where(c => c.Status == "Chưa mua" && c.UserId == idCustomer).Include(c => c.Product).ToListAsync();

            decimal total = 0;
            foreach (var item in productInCart)
            {
                decimal quantityDe = Convert.ToDecimal(item!.Quantity);
                decimal priceDe = Convert.ToDecimal(item!.Product!.Price);
                total += (quantityDe * priceDe);
            }

            //show total money on cart
            ViewData["TotalCart"] = "Tổng tiền: " + string.Format("{0:C}", total) + " VNĐ";

            return View(productInCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'FoodieContext.Carts'  is null.");
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            _notyf.Information("Xóa sản phẩm thành công");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(Guid id, int numberOf)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'FoodieContext.Carts'  is null.");
            }
            if(numberOf <= 0)
            {
                _notyf.Error("Vui lòng nhập đúng số lượng");
                return RedirectToAction(nameof(Index));
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
             
                cart.Quantity =  numberOf;
                _context.Carts.Update(cart);
            }

            await _context.SaveChangesAsync();

            _notyf.Success("Thay đổi số lượg thành công");
            return RedirectToAction(nameof(Index));
        }
    }
}
