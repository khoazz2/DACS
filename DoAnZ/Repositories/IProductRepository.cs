using DoAnZ.Models;

namespace DoAnZ.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<string?> GetAllProducts();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    }
}