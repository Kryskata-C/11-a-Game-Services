using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data.Entities; 
using WebApplication1.Models;
using WebApplication1.Utilities;      

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context; 
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber, string statusFilter)
        {

            if (searchString != null)
            {
                pageNumber = 1; 
            }
            else
            {
                searchString = currentFilter; 
            }
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatusFilter"] = statusFilter; 

            var ordersQuery = _context.Orders.Include(o => o.User).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                ordersQuery = ordersQuery.Where(o => o.User.UserName.Contains(searchString) || o.OrderId.ToString().Contains(searchString));
            }

            if (!String.IsNullOrEmpty(statusFilter))
            {
                if (Enum.TryParse<OrderStatus>(statusFilter, out var status))
                {
                    ordersQuery = ordersQuery.Where(o => o.Status == status);
                }
            }

            switch (sortOrder)
            {
                case "date_desc":
                    ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate);
                    break;
                default:
                    ordersQuery = ordersQuery.OrderBy(o => o.OrderDate);
                    break;
            }

            int pageSize = 10;
            var paginatedOrders = await PaginatedList<Order>.CreateAsync(ordersQuery.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(paginatedOrders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null) return NotFound();

            ViewBag.OrderStatuses = Enum.GetValues(typeof(OrderStatus))
                                        .Cast<OrderStatus>()
                                        .Select(s => new SelectListItem
                                        {
                                            Text = s.ToString(),
                                            Value = ((int)s).ToString()
                                        }).ToList();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Status = status;
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["AdminSuccessMessage"] = $"Order #{orderId} status updated to {status}.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}