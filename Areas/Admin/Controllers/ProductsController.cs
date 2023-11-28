using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

namespace OrderingFood.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly FoodieContext _context;
        readonly IBufferedFileUploadService _bufferedFileUploadService;//inject interface into this

        public ProductsController(FoodieContext context, IBufferedFileUploadService bufferedFileUploadService)
        {
            _context = context;
            _bufferedFileUploadService = bufferedFileUploadService;
        }


        //hiển thị danh sách
        public  IActionResult ImportProductsPdf()
        {
            var products = new List<Product>();
            return View(products);
        }

        //upload pdf and save products
        [HttpPost]
        public async Task<IActionResult> ImportProductsPdf(IFormFile file)
        {
            if(file == null)
            {
                return NotFound();
            }
            var fileExtension = Path.GetExtension(file.FileName);
            if (!fileExtension.ToLower().Equals(".xlsx"))
            {
                return NotFound();

            }


            var list = new List<Product>();
            using ( var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using(ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    var rowcount = workSheet.Dimension.Rows;
                    for(int row = 2;row<= rowcount; row++)
                    {
                       
                        
                        //create new Product()
                        list.Add(new Product
                        {
                            ProductId = Guid.NewGuid(),
                            Name = workSheet.Cells[row, 1].Value.ToString().Trim(),
                            Description = workSheet.Cells[row, 2].Value.ToString().Trim(),
                            Price = Convert.ToDecimal(workSheet.Cells[row, 3].Value) ,
                            Quantity = Convert.ToInt32(workSheet.Cells[row, 4].Value) ,
                            IsActive = "Còn cung ứng",
                            CreatedDate = DateTime.Now,

                        });

                    }


                }
            }
            
            //save in db
            foreach (Product item in list)
            {
                _context.Add(item);
            }
            await _context.SaveChangesAsync();

            return View(list);
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var foodieContext = _context.Products.Include(p => p.Categories);
            return View(await foodieContext.ToListAsync());
        }
        
        //search input
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string inputToString)
        {

            var data = await _context.Products.Where(e => e.Name!.Contains(inputToString) || e.Description!.Contains(inputToString) || e.Categories!.Name!.Contains(inputToString) || e.IsActive!.Contains(inputToString)).Include(p => p.Categories).ToListAsync();


            if (data != null)
            {
                return View(data);
            }
            else
            {
                return Problem("Không Tìm thấy món ăn");
            }
        }
        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            //render the img on tag
            ViewBag.imgSrc = "/productImages/" + product.ImageUrl;
            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,Price,Quantity,ImageUrl,CategoryId,IsActive,CreatedDate")] Product product,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                product.ProductId = Guid.NewGuid();
                product.CreatedDate = DateTime.Now;

                //enctype="multipart/form-data"
                string fileNameByCateName = _bufferedFileUploadService.GenerateSlug(product.Name) + Path.GetExtension(file.FileName);

                //handle upload image
                if (await _bufferedFileUploadService.UploadImageFile(file, "productImages", fileNameByCateName))
                {
                    product.ImageUrl = fileNameByCateName;
                    //can upload and save the img
                   
                }
                else
                {
                    //error to upload or save
                    ModelState.AddModelError("ImageUrl", "Không thể tải hoặc lưu hình ảnh đã chọn !");
                    return View(product);
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));


            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewData["ErrorMessage"] = "Không thể thêm mới món ăn";

            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductId,Name,Description,Price,Quantity,ImageUrl,CategoryId,IsActive,CreatedDate")] Product product, IFormFile file)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.CreatedDate = DateTime.Now;

                    string fileNameByCateName = _bufferedFileUploadService.GenerateSlug(product.Name) + Path.GetExtension(file.FileName);

                    //handle upload image
                    if (await _bufferedFileUploadService.UploadImageFile(file, "productImages", fileNameByCateName))
                    {
                        product.ImageUrl = fileNameByCateName;
                        //can upload and save the img
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        //error to upload or save
                        ModelState.AddModelError("ImageUrl", "Không thể tải hoặc lưu hình ảnh đã chọn !");

                    return View(product);
    

                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["ErrorMessage"] = "Không thể cập nhật món ăn";
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'FoodieContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }


        public string convertToVND(Decimal dem)
        {
            return string.Format("{0:C}", dem) + "VNĐ";
        }
    }
    
}
