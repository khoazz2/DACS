using DoAnZ.Models;
using DoAnZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DoAnZ.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;


        public AdminController(IProductRepository productRepository, ICategoryRepository categoryRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> ProductList()
        {
            var products = await _productRepository.GetAllAsync();
            return View("ProductList", products); // Sử dụng tên view "ProductList"
        }
        public async Task<IActionResult> DSSP()
        {
            var products = await _productRepository.GetAllAsync();
            return View("DSSP", products); // Sử dụng tên view "ProductList"
        }
        // Action method để hiển thị danh sách đơn hàng
        public async Task<IActionResult> DSDH()
        {
            // Lấy danh sách đơn hàng từ cơ sở dữ liệu
            var orders = await _context.Orders.ToListAsync();

            // Trả về view "DSDH" với danh sách đơn hàng
            return View(orders);
        }

        // Action method để hiển thị chi tiết đơn hàng
        public async Task<IActionResult> ChiTietDonHang(int id)
        {
            // Tìm đơn hàng trong cơ sở dữ liệu dựa trên id
            var order = await _context.Orders.FindAsync(id);

            // Nếu không tìm thấy đơn hàng, trả về trang 404
            if (order == null)
            {
                return NotFound();
            }

            // Trả về view "ChiTietDonHang" với đối tượng đơn hàng
            return View(order);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatus(int orderId, string status)
        {
            // Tìm đơn hàng trong cơ sở dữ liệu dựa trên orderId
            var order = await _context.Orders.FindAsync(orderId);

            // Nếu không tìm thấy đơn hàng, trả về trang 404
            if (order == null)
            {
                return NotFound();
            }

            // Thiết lập trạng thái mới cho đơn hàng
            Enum.TryParse(status, out OrderStatus newStatus);
            order.Status = newStatus;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Chuyển hướng đến trang chi tiết đơn hàng
            return RedirectToAction("ChiTietDonHang", new { id = order.Id });
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
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(ProductList));
            }

            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Viết thêm hàm SaveImage (tham khảo bào 02)
        private async Task<string> SaveImage(IFormFile imageUrl)
        {
            var fileName = $"{Guid.NewGuid()}_{imageUrl.FileName}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageUrl.CopyToAsync(fileStream);
            }

            return fileName;
        }

        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm

        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
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
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var existingProduct = await _productRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync

                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;

                await _productRepository.UpdateAsync(existingProduct);

                return RedirectToAction(nameof(ProductList));
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
                return NotFound();
            }

            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(ProductList));
        }
        public async Task<IActionResult> ShowCategory(string categoryName)
        {
            // Tìm category có categoryName tương ứng trong cơ sở dữ liệu
            var category = await _context.Set<Category>().FirstOrDefaultAsync(c => c.Name == categoryName);


            if (category == null)
            {
                // Trả về view báo lỗi nếu không tìm thấy danh mục
                return NotFound();
            }

            // Lấy categoryId từ category tìm được
            int categoryId = category.Id;

            // Gọi phương thức để lấy danh sách sản phẩm theo categoryId
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);

            // Truyền danh sách sản phẩm vào view thông qua ViewBag hoặc ViewData
            ViewBag.Products = products;

            return View();
        }
    }
}




