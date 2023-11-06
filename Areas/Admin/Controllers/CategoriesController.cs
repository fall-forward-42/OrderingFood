using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using OrderingFood.Data;
using OrderingFood.Interfaces;
using OrderingFood.Models;

namespace OrderingFood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly FoodieContext _context;
        readonly IBufferedFileUploadService _bufferedFileUploadService;//inject interface into this

        public CategoriesController(FoodieContext context, IBufferedFileUploadService bufferedFileUploadService)
        {
            _context = context;
            _bufferedFileUploadService = bufferedFileUploadService;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
              return _context.Categories != null ? 
                          View(await _context.Categories.ToListAsync()) :
                          Problem("Entity set 'FoodieContext.Categories'  is null.");
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            
           
            //render the img on tag
            ViewBag.imgSrc = "/cateImages/"+category.ImageUrl;
            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Name,ImageUrl,IsActive,CreatedDate")] Category category, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                category.CategoryId = Guid.NewGuid();
                category.CreatedDate = DateTime.Now;
                category.IsActive = "Còn cung ứng";
               
               string fileNameByCateName = _bufferedFileUploadService.GenerateSlug(category.Name) + Path.GetExtension(file.FileName);

                //handle upload image
                if (await _bufferedFileUploadService.UploadImageFile(file,"cateImages", fileNameByCateName))
                {
                    category.ImageUrl = fileNameByCateName;
                    //can upload and save the img
                    _context.Add(category);
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
            ViewData["ErrorMessage"] = "Không thể thêm mới danh mục";
            return View(category);
        }
        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CategoryId,Name,ImageUrl,IsActive,CreatedDate")] Category category, IFormFile file)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            { 
                try
                {
                    category.CreatedDate = DateTime.Now;

                  

                    string fileNameByCateName = _bufferedFileUploadService.GenerateSlug(category.Name) + Path.GetExtension(file.FileName);

                    //handle upload image
                    if (await _bufferedFileUploadService.UploadImageFile(file, "cateImages", fileNameByCateName))
                    {
                        category.ImageUrl = fileNameByCateName;
                        //can upload and save the img
                        _context.Update(category);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        //error to upload or save
                        ModelState.AddModelError("ImageUrl", "Không thể tải hoặc lưu hình ảnh đã chọn !");
                        return View(category);
                    }
                    
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

            }
            }
             ViewData["ErrorMessage"] = "Không thể cập nhật danh mục";
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'FoodieContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(Guid id)
        {
          return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
