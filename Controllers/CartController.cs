using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using OrderingFood.Data;
using OrderingFood.Models;
using System.Data;

namespace OrderingFood.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly FoodieContext _context;
        readonly INotyfService _notyf;

        private  String idCustomerStr = "";
        private String idEmployeeStr = "";

        public CartController(FoodieContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }
        private void getCookiesClient()
        {
            if (Request.Cookies["IdCustomerClient"] != null && Request.Cookies["IdAdminClient"] != null)
            {
                idCustomerStr = Request.Cookies["IdCustomerClient"]!;
                idEmployeeStr = Request.Cookies["IdAdminClient"]!;
            }
            else
            {
                _notyf.Warning("Vui lòng đăng nhập");
                RedirectToAction("Login", "Account");
            }
           
        }

        public async Task<IActionResult> Index()
        {
            getCookiesClient();

            var idCustomer = new Guid(idCustomerStr);

            var productInCart = await _context.Carts.Where(c => c.Status == "Chưa mua" && c.UserId == idCustomer).Include(c => c.Product).ToListAsync();

            decimal total = 0;
            foreach (var item in productInCart)
            {
                decimal quantityDe = Convert.ToDecimal(item!.Quantity);
                decimal priceDe = Convert.ToDecimal(item!.Product!.Price);
                total += (quantityDe * priceDe);
            }
            //set cookie for total
            if (Request.Cookies["TotalBillClient"] != null)
            {
                //update cookie
                Response.Cookies.Append("TotalBillClient", total.ToString());
            }
            else
            {
                //new cookie
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddSeconds(30000);
                Response.Cookies.Append("TotalBillClient", total.ToString(), options);
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
        //tạo mới hóa đơn cho đơn hàng chưa thanh toán - giỏ hàng khách
        public IActionResult CreateBillNotPay()
        {

            //get cookie customer and admin
            var idCustomer = Request.Cookies["IdCustomerClient"]!;
            var idEmployee = Request.Cookies["IdAdminClient"]!;
            if(Request.Cookies["TotalBillClient"] == null)
            {
                _notyf.Error("Giỏ hàng đang bị lỗi, vui lòng chọn các sản phẩm khác ");
                return Redirect("Index");
            }
            var TotalBill = Convert.ToDecimal(Request.Cookies["TotalBillClient"]);

            //không thể tính nếu đơn đặt hàng chưa chọn sản phẩm
            if (TotalBill <= 0)
            {
                _notyf.Error("Chưa có sản phẩm nào để thanh toán");
                return RedirectToAction("Index");
            }

            //khởi tạo info bill
            Bill bill = new Bill();
            bill.BillId = Guid.NewGuid();
            bill.Status = "Chưa thanh toán";
            bill.CreatedDate = DateTime.Now;
            bill.UserId = new Guid(idCustomer.ToString());
            bill.EmployeeId = new Guid(idEmployee.ToString());
            bill.Total = TotalBill;

            _context.Bills.Add(bill); //thêm các sản phẩm giỏ hàng 
            _context.SaveChangesAsync();

            //set cookie for id bill
            if (Request.Cookies["IdBillClient"] != null)
            {
                //update cookie
                Response.Cookies.Append("IdBillClient", bill.BillId.ToString());
            }
            else
            {
                //new cookie
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddSeconds(30000);
                Response.Cookies.Append("IdBillClient", bill.BillId.ToString(), options);
            }

            _notyf.Success("Cảm ơn quý khách đã mua hàng !");
            return RedirectToAction("DetailsNotPay");

        }
        //show hóa đơn mới tạo và xóa hết all món đã mua trong giỏ hàng
        public async Task<IActionResult> DetailsNotPay()
        {
            if (Request.Cookies["IdBillClient"] == null)
            {
                _notyf.Information("Chưa có đơn nào được thực hiện");
                return RedirectToAction("Index");

            }
            
            var idBill = new Guid(Request.Cookies["IdBillClient"]!);
            var idCustomer = new Guid(Request.Cookies["IdCustomerClient"]!);


            var bill = await _context.Bills
                .Include(b => b.Employee)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BillId == idBill);

            //xuất list products mà cus mua - update status đã mua trong Carts
            List<Cart> productsInCart = await _context.Carts.Where(c => c.UserId == idCustomer && c.Status == "Chưa mua").ToListAsync();
            //update all products in cart
            foreach (Cart c in productsInCart)
            {
                c.Status = "Đã mua";
                c.BillId = idBill;
                _context.Update(c);
                await _context.SaveChangesAsync();
            }




            if (bill == null)
            {
                return NotFound();
            }
            _notyf.Information("Đơn hàng đang được xử lý...");
            return View(bill);
        }

    }
    
}
