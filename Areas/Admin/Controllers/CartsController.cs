using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderingFood.Data;
using OrderingFood.Models;

namespace OrderingFood.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CartsController : Controller
    {
        private readonly FoodieContext _context;

        public CartsController(FoodieContext context)
        {
            _context = context;
        }


     
        // GET: Admin/Carts
        public async Task<IActionResult> Index()
        {
            //find customer cookie - exists
            if (Request.Cookies["IdCustomer"] != null)
            {
                ViewData["CustomerChoosen"] = "True";

                var idCustomer = new Guid(Request.Cookies["IdCustomer"]!);
                //show list of customer  - cus selected
                ViewData["UserId"] = new SelectList(_context.Users.Where(c => c.TypeAccount == "Customer"), "UserId", "Name",idCustomer);
                //show all carts of customer selected
                var foodieContext = await _context.Carts.Where(c => c.Status == "Chưa mua").Include(c => c.Product).Include(c => c.User).Where(u=>u.UserId==idCustomer).ToListAsync();
               
                //cal total of all items in this cart
                decimal total = 0;
                foreach(Cart item in foodieContext)
                {
                    //decimal convertQuantity =  + 0.0m; 
                    decimal quantityDe = Convert.ToDecimal(item!.Quantity);
                    decimal priceDe = Convert.ToDecimal(item!.Product!.Price);
                    total += (quantityDe* priceDe);
                }

                //find totalCart cookie to handle create Bill
                if (Request.Cookies["TotalCart"] != null)
                {
                    //update cookie
                    Response.Cookies.Append("TotalCart", total.ToString());
                }
                else
                {
                    //new cookie
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddSeconds(30000);
                    Response.Cookies.Append("TotalCart", total.ToString(), options);
                }
                //show total money on cart
                ViewData["TotalCart"] = "Tổng tiền: "+ string.Format("{0:C}", total) + " VNĐ";
                return View(foodieContext);
            }
            //if not yet choose customer
            else
            {
                ViewData["CustomerChoosen"] = "False";
                //pls choose cus to find 
                ViewData["UserId"] = new SelectList(_context.Users.Where(c => c.TypeAccount == "Customer"), "UserId", "Name");
                //show all carts
                var foodieContext = _context.Carts.Where(c => c.Status == "Chưa mua").Include(c => c.Product).Include(c => c.User);
                ViewData["TotalCart"] = "Vui lòng chọn khách hàng để xem đơn đặt hàng";
                return View(await foodieContext.ToListAsync());
            }

            

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Guid UserId)
        {
            //get customer
            var customer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == UserId);
            //if customer found
            if (customer != null)
            {
                //set cookie for id customer
                if (Request.Cookies["IdCustomer"]  != null)
                {
                    //update cookie
                    Response.Cookies.Append("IdCustomer", UserId.ToString());
                }
                else
                {
                    //new cookie
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddSeconds(30000);
                    Response.Cookies.Append("IdCustomer", UserId.ToString(), options);
                }
               
            }
            return RedirectToAction("Index");
         
        }



        // GET: Admin/Carts/Create
        public IActionResult Create()
        {
            //choose product to buy
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            //get id customer cookie to render name of customer
            var idCustomer = new Guid(Request.Cookies["IdCustomer"]!) ;
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", idCustomer);

           
            return View();
        }

        // POST: Admin/Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,ProductId,Quantity,UserId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                cart.CartId = Guid.NewGuid();
                //get cookie and set to cart
                var Idcustomer = new Guid(Request.Cookies["IdCustomer"]!);
                cart.UserId = Idcustomer;
                cart.Status = "Chưa mua";


                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", Idcustomer);
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", cart.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", cart.UserId);
            return View(cart);
        }

        // GET: Admin/Carts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", cart.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", cart.UserId);
            return View(cart);
        }

        // POST: Admin/Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public IActionResult Plus1Product(Guid? id)
        {
            var itemOfCart = _context.Carts.Where(c=>c.CartId == id).FirstOrDefault();
               itemOfCart!.Quantity += 1;
            _context.Update(itemOfCart);
             _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Remove1Product(Guid? id)
        {

            var itemOfCart = _context.Carts.Where(c => c.CartId == id).FirstOrDefault();
            itemOfCart!.Quantity -= 1;
            _context.Update(itemOfCart);
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CartId,ProductId,Quantity,UserId")] Cart cart)
        {
            if (id != cart.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    cart.Status = "Chưa mua";
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", cart.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", cart.UserId);
            return View(cart);
        }

        // GET: Admin/Carts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Admin/Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
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
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(Guid id)
        {
          return (_context.Carts?.Any(e => e.CartId == id)).GetValueOrDefault();
        }
    }
}
