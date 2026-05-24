using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrangWebBanHang.Models;
// Giả sử bạn đã có OrderRepository, nếu chưa có hãy tạo tương tự ProductRepository
// using TrangWebBanHang.Repositories; 

namespace TrangWebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        // Giả sử dùng DbContext trực tiếp nếu chưa làm Repository cho Order
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders.ToList(); // Lấy danh sách đơn hàng
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                // order.Status = status; 
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}