using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TrangWebBanHang.Models;
using TrangWebBanHang.Repositories; // Cần thêm dòng này

namespace TrangWebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // Khai báo thêm Repository để lấy dữ liệu sản phẩm
        private readonly IProductRepository _productRepository;

        // Inject ProductRepository vào Constructor
        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách sản phẩm từ Database
            var products = await _productRepository.GetAllAsync();

            // Truyền danh sách sản phẩm qua View
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}