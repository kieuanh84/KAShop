using Microsoft.AspNetCore.Mvc;
using TrangWebBanHang.Models;
using TrangWebBanHang.Repositories;

namespace TrangWebBanHang.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        // Constructor nhận vào ICategoryRepository để thao tác với dữ liệu
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Hiển thị danh sách các category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        // Hiển thị form thêm category mới
        public IActionResult Add()
        {
            return View();
        }

        // Xử lý thêm category mới
        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index)); // Sau khi thêm, quay lại trang danh sách
            }
            return View(category); // Nếu dữ liệu không hợp lệ, hiển thị lại form
        }

        // Hiển thị form cập nhật category
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(); // Nếu không tìm thấy category, trả về 404
            }
            return View(category);
        }

        // Xử lý cập nhật category
        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound(); // Nếu ID không khớp, trả về 404
            }

            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index)); // Sau khi cập nhật, quay lại trang danh sách
            }

            return View(category); // Nếu dữ liệu không hợp lệ, hiển thị lại form
        }

        // Hiển thị form xác nhận xóa category
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(); // Nếu không tìm thấy category, trả về 404
            }
            return View(category);
        }

        // Xử lý xóa category
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index)); // Sau khi xóa, quay lại trang danh sách
        }
    }
}
