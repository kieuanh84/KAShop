using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrangWebBanHang.Models;
using TrangWebBanHang.Repositories;
using TrangWebBanHang.Helpers;
using Microsoft.EntityFrameworkCore;

namespace TrangWebBanHang.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context; // Dùng để lưu đơn hàng trực tiếp

        public ShoppingCartController(IProductRepository productRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        // Thêm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }
            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }

        // Trang nhập thông tin thanh toán (Yêu cầu đăng nhập)
        [Authorize]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (cart.Count == 0) return RedirectToAction("Index");
            return View(new Order());
        }

        // Xử lý lưu đơn hàng vào Database
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProcessCheckout(Order order)
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            if (cart == null || !cart.Any()) return RedirectToAction("Index");

            // 1. Cấu hình thông tin cơ bản cho đơn hàng
            order.OrderDate = DateTime.Now;
            order.TotalAmount = cart.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = new List<OrderDetail>();

            // 2. Chuyển đổi từ CartItem (trong Session) sang OrderDetail (để lưu DB)
            foreach (var item in cart)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            // 3. Lưu vào Database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 4. Xóa giỏ hàng sau khi đặt thành công
            HttpContext.Session.Remove("Cart");

            return View("OrderSuccess", order.Id); // Trả về trang thông báo thành công
        }
    }
}