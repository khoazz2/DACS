using DoAnZ.Models;
using DoAnZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace DoAnZ.Controllers
{
    public class UserController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

    

        public async Task<IActionResult> DSDH()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .ToListAsync();

            return View(orders);
        }
        public async Task<IActionResult> CreateOrder(Order order)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            order.UserId = user.Id;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DSDH));
        }

    }
}
