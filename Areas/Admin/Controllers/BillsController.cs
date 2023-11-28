using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderingFood.Data;
using OrderingFood.Models;

namespace OrderingFood.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class BillsController : Controller
    {
        private readonly FoodieContext _context;

        public BillsController(FoodieContext context)
        {
            _context = context;
        }

        //hiển thị doanh thu
        public async Task<IActionResult> RevenueBill()
        {
           //ViewData["BillDate"] = new SelectList(_context.Bills, "CreatedDate", "CreatedDate");
            var foodieContext = _context.Bills.Where(b => b.Status == "Đã thanh toán").Include(b => b.Employee).Include(b => b.User);
            return View(await foodieContext.ToListAsync());
        }
       

        [HttpPost]
        public async Task<IActionResult> RevenueBill(int dayR, int monthR,int yearR)
        {
           /* string convert_dayR =  dayRevenue.ToString("dd-mm-yyyy");*/

            var foodieContext = await _context.Bills.Where(b => b.Status == "Đã thanh toán" && b.CreatedDate.Day==dayR && b.CreatedDate.Month == monthR && b.CreatedDate.Year == yearR)
                .Include(b => b.Employee).Include(b => b.User).ToListAsync();

            decimal TotalBill = 0;
            foreach (Bill b in foodieContext)
            {
                decimal thisTotal = Convert.ToDecimal(b.Total);
                TotalBill += thisTotal;
            }
            ViewData["DateToRevenue"] = " ngày "+ dayR.ToString()+ " Tháng " + monthR.ToString()+" năm " + yearR.ToString();
            ViewData["TotalBills"] = "Tổng tiền: " + string.Format("{0:C}", TotalBill) + " VNĐ";
            return View(foodieContext);
        }

        //hiển thị các hóa đơn đã xử lý - tính doanh thu chọn tìm kiếm
        public async Task<IActionResult> Index()
        {
            var foodieContext = _context.Bills.Where(b => b.Status == "Đã thanh toán").Include(b => b.Employee).Include(b => b.User);
            return View(await foodieContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string inputToString)
        {
            var foodieContext = await _context.Bills.Where(b => b.Status == "Đã thanh toán" && b.User!.Name.Contains(inputToString) || b.Employee!.Name.Contains(inputToString) || b.User!.Mobile.Contains(inputToString) || b.User!.Address.Contains(inputToString) || b.CreatedDate.ToString()!.Contains(inputToString))
                .Include(b => b.Employee).Include(b => b.User).ToListAsync();
            decimal TotalBill = 0;
            foreach(Bill b in foodieContext)
            {
                decimal thisTotal = Convert.ToDecimal(b.Total);
                TotalBill += thisTotal;
            }

            ViewData["TotalBills"] = "Tổng tiền: " + string.Format("{0:C}", TotalBill) + " VNĐ";
            return View( foodieContext);
        }

        //hiển thị các bill cần xử lý đơn hàng
        public async Task<IActionResult> HandlePackage()
        {
            var foodieContext = _context.Bills.Where(b=>b.Status=="Chưa thanh toán").Include(b => b.Employee).Include(b => b.User);
            return View(await foodieContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> HandlePackage(string inputToString)
        {
            var foodieContext = _context.Bills.Where(b => b.Status == "Chưa thanh toán" &&  b.User!.Name.Contains(inputToString) || b.Employee!.Name.Contains(inputToString) || b.User!.Mobile.Contains(inputToString) || b.User!.Address.Contains(inputToString))
                .Include(b => b.Employee).Include(b => b.User);
            return View(await foodieContext.ToListAsync());
        }


        //xác nhận thanh toán hóa đơn
        public async Task<IActionResult> ConfirmBill(Guid? id)
        {
            if (id == null || _context.Bills == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            bill.Status = "Đã thanh toán";

             _context.Update(bill);
             _context.SaveChanges();

            return RedirectToAction("Index");


        }


        // GET: Admin/Bills/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Bills == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.Employee)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BillId == id);
            
            var productsHaveBought = await _context.Carts.Where(c=>c.BillId== id).Include(c=>c.Product).ToListAsync();


            ViewBag.productsHaveBought = productsHaveBought ;

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Admin/Bills/Create
        public IActionResult Create()
        {
            if (Request.Cookies["IdCustomer"] == null)
            {
                return BadRequest();
            }
            if (Request.Cookies["IdAdmin"] == null)
            {
                return BadRequest();

            }
            if (Request.Cookies["TotalCart"] == null)
            {
                return BadRequest();

            }
            //get cookie customer and admin
            var idCustomer = new Guid(Request.Cookies["IdCustomer"]!);
            var idEmployee = new Guid(Request.Cookies["IdAdmin"]!);
            var TotalBill = Convert.ToDecimal(Request.Cookies["TotalCart"]);


            //show name of customer and employee for this bill
            string Ename = _context.Users.Where(u => u.UserId == idEmployee && u.TypeAccount == "Admin").FirstOrDefault()!.Name;
            ViewData["EmployeeName"] = Ename;
            ViewData["EmployeeId"] = idEmployee;

            string Cname = _context.Users.Where(u => u.UserId == idCustomer && u.TypeAccount == "Customer").FirstOrDefault()!.Name;
            ViewData["CustomerName"] = Cname;
            ViewData["CustomerId"] = idCustomer;

            ViewData["TotalBillString"]=  string.Format("{0:C}", TotalBill) + " VNĐ";
            ViewData["TotalBill"] = TotalBill;

            return View(); 
        }

        // POST: Admin/Bills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Total,CreatedDate,UserId,EmployeeId")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                bill.BillId = Guid.NewGuid();
                bill.Status = "Chưa thanh toán";
                bill.CreatedDate = DateTime.Now;
                bill.UserId = new Guid(Request.Cookies["IdCustomer"]!);
                bill.EmployeeId = new Guid(Request.Cookies["IdAdmin"]!);
                bill.Total = Convert.ToDecimal(Request.Cookies["TotalCart"]!);



                _context.Add(bill);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details",bill.BillId);
            }
            ViewData["Error"] = "Không thể lập hóa đơn này !";
            return View(bill);
        }

        //show hóa đơn mới tạo và xóa hết all món đã mua trong giỏ hàng
        public async Task<IActionResult> DetailsNotPay()
        {
            if (Request.Cookies["IdBill"] == null)
            {
                return BadRequest();

            }
            if (Request.Cookies["IdCustomer"] == null)
            {
                return BadRequest();

            }
            var idBill = new Guid(Request.Cookies["IdBill"]!);
            var idCustomer = new Guid(Request.Cookies["IdCustomer"]!);


            var bill = await _context.Bills
                .Include(b => b.Employee)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BillId == idBill);

            //xuất list products mà cus mua - update status đã mua trong Carts
            List<Cart> productsInCart = await _context.Carts.Where(c => c.UserId == idCustomer && c.Status == "Chưa mua").ToListAsync();
            //update all products in cart
            foreach(Cart c in productsInCart)
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

            return View(bill);
        }

        

        //tạo mới hóa đơn cho đơn hàng chưa thanh toán - giỏ hàng khách
        public IActionResult CreateBillNotPay()
        {

            if (Request.Cookies["IdCustomer"] == null)
            {
                return BadRequest();
            }
            if (Request.Cookies["IdAdmin"] == null)
            {
                return BadRequest();

            }
            if (Request.Cookies["TotalCart"] == null)
            {
                return BadRequest();

            }
            //get cookie customer and admin
            var idCustomer = new Guid(Request.Cookies["IdCustomer"]!);
            var idEmployee = new Guid(Request.Cookies["IdAdmin"]!);
            var TotalBill = Convert.ToDecimal(Request.Cookies["TotalCart"]);

            //không thể tính nếu đơn đặt hàng chưa chọn sản phẩm
            if (TotalBill <= 0)
            {
                return BadRequest();

            }
          
            //khởi tạo info bill
            Bill bill = new Bill();
            bill.BillId = Guid.NewGuid();
            bill.Status = "Chưa thanh toán";
            bill.CreatedDate = DateTime.Now;
            bill.UserId = new Guid(Request.Cookies["IdCustomer"]!);
            bill.EmployeeId = new Guid(Request.Cookies["IdAdmin"]!);
            bill.Total = Convert.ToDecimal(Request.Cookies["TotalCart"]!);

            _context.Add(bill);
            _context.SaveChangesAsync();

            //set cookie for id bill
            if (Request.Cookies["IdBill"] != null)
            {
                //update cookie
                Response.Cookies.Append("IdBill", bill.BillId.ToString());
            }
            else
            {
                //new cookie
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddSeconds(30000);
                Response.Cookies.Append("IdBill", bill.BillId.ToString(), options);
            }

            return RedirectToAction("DetailsNotPay");

        }



        // GET: Admin/Bills/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Bills == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Users, "UserId", "Address", bill.EmployeeId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Address", bill.UserId);
            return View(bill);
        }

        // POST: Admin/Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BillId,Status,Total,CreatedDate,UserId,EmployeeId")] Bill bill)
        {
            if (id != bill.BillId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Users, "UserId", "Address", bill.EmployeeId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Address", bill.UserId);
            return View(bill);
        }

        // GET: Admin/Bills/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Bills == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.Employee)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BillId == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Admin/Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Bills == null)
            {
                return Problem("Entity set 'FoodieContext.Bills'  is null.");
            }

            //remove bill and detailsbill
            var bill = await _context.Bills.Where(b=>b.BillId==id).Include(b=>b.Carts).FirstOrDefaultAsync();


            if (bill != null)
            {
                _context.Bills.Remove(bill);
            }


     

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(Guid id)
        {
          return (_context.Bills?.Any(e => e.BillId == id)).GetValueOrDefault();
        }
    }
}
