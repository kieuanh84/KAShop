using System.ComponentModel.DataAnnotations;

namespace TrangWebBanHang.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation property (Nếu bạn muốn lưu chi tiết từng món trong đơn)
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
