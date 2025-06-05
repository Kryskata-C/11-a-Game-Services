using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication1.Data.Entities;
using WebApplication1.Models;
using WebApplication1.Utilities;

namespace WebApplication1.Controllers
{
    [Authorize] 
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context; 

        public OrderController(UserManager<ApplicationUser> userManager, ApplicationDbContext context )
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Checkout()
        {
            var basketJson = HttpContext.Session.GetString("Basket");
            var pricesJson = HttpContext.Session.GetString("Prices");

            if (string.IsNullOrEmpty(basketJson) || string.IsNullOrEmpty(pricesJson))
            {
                TempData["ErrorMessage"] = "Your basket is empty or item prices are missing.";
                return RedirectToAction("Basket", "Home");
            }

            var basket = JsonSerializer.Deserialize<Dictionary<string, int>>(basketJson);
            var prices = JsonSerializer.Deserialize<Dictionary<string, int>>(pricesJson);

            if (basket == null || !basket.Any())
            {
                TempData["ErrorMessage"] = "Your basket is empty.";
                return RedirectToAction("Basket", "Home");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge(); 

            var checkoutViewModel = new CheckoutViewModel
            {
                Items = new List<CheckoutItemViewModel>(),
                UserBalance = user.Balance
            };

            decimal currentOrderTotal = 0;
            foreach (var item in basket)
            {
                if (prices.TryGetValue(item.Key, out int pricePerHour))
                {
                    checkoutViewModel.Items.Add(new CheckoutItemViewModel
                    {
                        ServiceName = item.Key,
                        Quantity = item.Value,
                        PricePerUnit = pricePerHour,
                        TotalPrice = pricePerHour * item.Value
                    });
                    currentOrderTotal += pricePerHour * item.Value;
                }
            }
            checkoutViewModel.OrderTotal = currentOrderTotal;

            if (!checkoutViewModel.Items.Any())
            {
                TempData["ErrorMessage"] = "No valid items found in your basket to checkout.";
                return RedirectToAction("Basket", "Home");
            }

            return View(checkoutViewModel);
        }

        // POST: /Order/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var basketJson = HttpContext.Session.GetString("Basket");
            var pricesJson = HttpContext.Session.GetString("Prices");

            if (string.IsNullOrEmpty(basketJson) || string.IsNullOrEmpty(pricesJson))
            {
                TempData["ErrorMessage"] = "Your basket seems to have expired or is empty.";
                return RedirectToAction("Checkout");
            }

            var basket = JsonSerializer.Deserialize<Dictionary<string, int>>(basketJson);
            var prices = JsonSerializer.Deserialize<Dictionary<string, int>>(pricesJson);

            if (basket == null || !basket.Any())
            {
                TempData["ErrorMessage"] = "Your basket is empty. Cannot place order.";
                return RedirectToAction("Checkout");
            }

            decimal orderTotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in basket)
            {
                if (prices.TryGetValue(item.Key, out int pricePerHour))
                {
                    orderTotal += pricePerHour * item.Value;
                    orderItems.Add(new OrderItem
                    {
                        HiredServiceName = item.Key,
                        Quantity = item.Value,
                        PriceAtPurchase = pricePerHour
                    });
                }
            }

            if (user.Balance < orderTotal)
            {
                TempData["ErrorMessage"] = "Insufficient funds to place this order. Please deposit more funds.";
                return RedirectToAction("Checkout"); // Or back to basket, or to a deposit page
            }

            // TODO: This logic should ideally be in an IOrderService
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    user.Balance -= orderTotal;
                    var userUpdateResult = await _userManager.UpdateAsync(user);

                    if (!userUpdateResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = "Failed to update your balance. Order not placed.";
                        // Log errors from userUpdateResult.Errors
                        return RedirectToAction("Checkout");
                    }

                    var order = new Order
                    {
                        UserId = user.Id,
                        OrderDate = DateTime.UtcNow,
                        TotalPrice = orderTotal,
                        Status = OrderStatus.Processing, // Or Pending if further confirmation is needed
                        OrderItems = orderItems
                    };
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    HttpContext.Session.Remove("Basket");
                    HttpContext.Session.Remove("Prices");

                    TempData["SuccessMessage"] = $"Order #{order.OrderId} placed successfully! Your new balance is {user.Balance:C2}.";
                    return RedirectToAction("Details", new { orderId = order.OrderId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log exception ex
                    TempData["ErrorMessage"] = "An error occurred while placing your order. Please try again.";
                    return RedirectToAction("Checkout");
                }
            }
        }

        public async Task<IActionResult> Details(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var order = await _context.Orders
                                .Include(o => o.OrderItems)
                                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == user.Id);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found or you do not have permission to view it.";
                return RedirectToAction("History");
            }

            return View(order);
        }

        // GET: /Order/History
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var orders = await _context.Orders
                                 .Where(o => o.UserId == user.Id)
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToListAsync();

            return View(orders);
        }
    }
}