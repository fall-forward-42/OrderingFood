using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderingFood.Data;
using OrderingFood.Interfaces;
using OrderingFood.Models;

namespace OrderingFood.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class UsersController : Controller
    {
        readonly IBufferedFileUploadService _bufferedFileUploadService;//inject interface into this

        private readonly FoodieContext _context;

        public UsersController(FoodieContext context, IBufferedFileUploadService bufferedFileUploadService)
        {
            _context = context;
            _bufferedFileUploadService = bufferedFileUploadService;

        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            return _context.Users != null ?
                        View(await _context.Users.ToListAsync()) :
                        Problem("Không có người dùng nào");
        }
        // POST: Admin/Users/inputToSearch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string inputToString)
        {

            var data = await _context.Users.Where(e => e.Name!.Contains(inputToString) || e.Mobile!.Contains(inputToString) || e.Address!.Contains(inputToString) || e.Email!.Contains(inputToString)).ToListAsync();
            if (data!=null)
            {
                return View(data);
            }
            else
            {
                return Problem("Không Tìm thấy người dùng");
            }
        }



        //thay đổi mật khẩu
        public async Task<IActionResult> ChangePasswordForUser(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            if(user == null)
            {
                return NotFound();
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordForUser(Guid? id, [Bind("Password,ConfPassword, NewPassword")] User user)
        {
           /* if (id == null)
            {
                return NotFound();
            }*/

            var thisUser = await _context.Users.FindAsync(id);


           /* if (ModelState.IsValid)
            {*/
                    if(thisUser!.Password != user.Password)
                    {
                        ViewData["Error"] = "Mật khẩu cũ không chính xác !";
                        return View();
                    }
                    if(user.ConfPassword != user.NewPassword)
                    {
                        ViewData["Error"] = "Mật khẩu xác nhận không chính xác !";
                        return View();
                    }

                    thisUser.Password = user.NewPassword;
                    _context.Users.Update(thisUser);
                    await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details),new {id});
           /* }


            ViewData["Error"] = "Thay đổi mật khẩu thất bại !";
            return View();*/










        }
        //hiển thị profile admin
        public async Task<IActionResult> ShowAdminProfile()
        {
            if (Request.Cookies["IdAdmin"]== null)
            {
                return NotFound();   
            }

            var idAdmin = new Guid(Request.Cookies["IdAdmin"]!);

            var user = await _context.Users
               .FirstOrDefaultAsync(m => m.UserId == idAdmin);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);

        }
        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Mobile,Address,Email,Password,TypeAccount")] User user, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                
                user.UserId = Guid.NewGuid();
                user.CreatedDate = DateTime.Now;
                user.TypeAccount = "Customer";

                string fileImgForUser = _bufferedFileUploadService.GenerateSlug(user.Name) + Path.GetExtension(file.FileName);

                //handle upload image
                if (await _bufferedFileUploadService.UploadImageFile(file, "userImages", fileImgForUser))
                {
                    user.ImageUrl = fileImgForUser;
                    //can upload and save the img
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //error to upload or save
                    ModelState.AddModelError("ImageUrl", "Không thể tải hoặc lưu hình ảnh đã chọn !");
                    return View();
                }
                
            }
            ViewData["ErrorMessage"] = "Không thể thêm mới người dùng";
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? id, [Bind("UserId,Name,Mobile,Address,CreatedDate,ImageUrl,Email,Password,TypeAccount")] User user, IFormFile file)
        {
            ViewBag.Id = id;
            ViewBag.UserId = user.UserId;

            if (id != user.UserId)
            {
                return NotFound();

            }


            if (ModelState.IsValid)
            {
                try
                {
                    string fileNameByCateName = _bufferedFileUploadService.GenerateSlug(user.Name) + Path.GetExtension(file.FileName);

                    //handle upload image
                    if (await _bufferedFileUploadService.UploadImageFile(file, "userImages", fileNameByCateName))
                    {
                        user.ImageUrl = fileNameByCateName;
                        //can upload and save the img
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        //error to upload or save
                        ModelState.AddModelError("ImageUrl", "Không thể tải hoặc lưu hình ảnh đã chọn !");
                        return View(user);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'FoodieContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private List<User>? FindUserWithSubstring(string input)
        {
            return _context.Users?.Where(e => e.Name!.Contains(input) || e.Mobile!.Contains(input) || e.Address!.Contains(input) || e.Email!.Contains(input)).ToList();
            
        }
        private bool UserExists(Guid id)
        {
          return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
