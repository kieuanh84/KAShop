
using System.Collections.Generic;
using WebBanHang.Models;

namespace WebBanHang.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
    }
}
