using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrangWebBanHang.Models;
using TrangWebBanHang.Repositories;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace TrangWebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        // Constructor để khởi tạo ProductRepository và CategoryRepository
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                await _productRepository.AddAsync(product); // Thêm sản phẩm vào cơ sở dữ liệu
                return RedirectToAction(nameof(Index)); // Quay lại danh sách sản phẩm
            }

            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Hàm lưu hình ảnh
        private async Task<string> SaveImage(IFormFile image)
        {
            // Đảm bảo thư mục 'wwwroot/images' tồn tại
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, image.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream); // Lưu hình ảnh vào thư mục
            }

            return "/images/" + image.FileName; // Trả về đường dẫn tương đối cho sản phẩm
        }

        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm, trả về lỗi 404
            }
            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm, trả về lỗi 404
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl

            if (id != product.Id)
            {
                return NotFound(); // Nếu ID không khớp, trả về lỗi 404
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);

                if (existingProduct == null)
                {
                    return NotFound(); // Nếu sản phẩm không tồn tại
                }

                // Nếu không có hình ảnh mới, giữ nguyên hình ảnh cũ
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                // Cập nhật các trường thông tin của sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;

                // Cập nhật sản phẩm trong cơ sở dữ liệu
                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index)); // Quay lại danh sách sản phẩm
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm, trả về lỗi 404
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm, trả về lỗi 404
            }

            await _productRepository.DeleteAsync(id); // Xóa sản phẩm khỏi cơ sở dữ liệu
            return RedirectToAction(nameof(Index)); // Quay lại danh sách sản phẩm
        }
    }
}