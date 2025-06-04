using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [IgnoreAntiforgeryToken] 
        public IActionResult Error(int? statusCode = null)
        {
            var viewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = "An unexpected error occurred." 
            };

            if (statusCode.HasValue)
            {
                viewModel.StatusCode = statusCode.Value;
                viewModel.OriginalPath = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>()?.OriginalPath;

                if (statusCode == 404)
                {
                    viewModel.Message = "Sorry, the page you requested could not be found.";
                }
                else if (statusCode == 401)
                {
                    viewModel.Message = "Sorry, you are not authorized to access this page.";
                }
                else if (statusCode == 500)
                {
                    viewModel.Message = "An internal server error occurred. We are looking into it.";
                }
            }

            return View(viewModel); 
        }
        public IActionResult AboutDeveloper()
        {
            return View();
        }

        public IActionResult PlayerHire()
        {
            return View();
        }

        public IActionResult TeamHire()
        {
            return RedirectToAction("Index", "Teams");
        }

        public IActionResult Basket()
        {
            var basketJson = HttpContext.Session.GetString("Basket");
            var priceJson = HttpContext.Session.GetString("Prices");

            var basket = string.IsNullOrEmpty(basketJson)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(basketJson);

            var prices = string.IsNullOrEmpty(priceJson)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(priceJson);

            ViewBag.PlayersPrices = prices;

            return View(basket);
        }

        [HttpPost]
        public IActionResult AddToBasket(string playerName, int price)
        {
            var basketJson = HttpContext.Session.GetString("Basket");
            var basket = string.IsNullOrEmpty(basketJson)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(basketJson);

            if (basket.ContainsKey(playerName))
            {
                basket[playerName]++;
            }
            else
            {
                basket[playerName] = 1;
            }
            HttpContext.Session.SetString("Basket", JsonSerializer.Serialize(basket));

            var priceJson = HttpContext.Session.GetString("Prices");
            var priceDict = string.IsNullOrEmpty(priceJson)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(priceJson);

            priceDict[playerName] = price;
            HttpContext.Session.SetString("Prices", JsonSerializer.Serialize(priceDict));

            return RedirectToAction("Basket");
        }

        public async Task<IActionResult> UserProfile()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login", "Account");
            }

            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                _logger.LogWarning($"Authenticated user '{User.Identity.Name}' not found in database. Logging out.");
                await _signInManager.SignOutAsync();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var model = new UserProfileViewModel
            {
                Username = identityUser.UserName,
                UserSince = DateTime.UtcNow,
                TotalSpent = 0,
                MostHiredPlayers = new List<string>(),
                Reviews = new List<Review>()
            };

            return View("User", model);
        }

        public IActionResult PlayerInfo(string playerName)
        {
            var reviewsDictionary = new Dictionary<string, List<Review>>
            {
                { "Atanas (Cenkata)", new List<Review> { new Review { ReviewerName = "Ivan", Comment = "Insanely good!", StarRating = 5 } } },
                { "Boris (shefa na relefa)", new List<Review> { new Review { ReviewerName = "Mira", Comment = "Very strategic.", StarRating = 4 } } },
                { "Aryaan (Ary)", new List<Review> { new Review { ReviewerName = "Toni", Comment = "Awesome trick shots.", StarRating = 5 } } },
                { "Nikolay (гоuemия)", new List<Review> { new Review { ReviewerName = "Alex", Comment = "Very tactical player.", StarRating = 5 } } },
                { "Maxi (Dragon)", new List<Review> { new Review { ReviewerName = "Maria", Comment = "Catches you off guard.", StarRating = 4 } } },
                { "Vaseto (....)", new List<Review> { new Review { ReviewerName = "Stoyan", Comment = "Great support!", StarRating = 4 } } },
                { "Christian (kryskata)", new List<Review> { new Review { ReviewerName = "Niki", Comment = "New but skilled.", StarRating = 3 } } },
            };

            var model = new PlayerDetailViewModel();

            switch (playerName)
            {
                case "Atanas (Cenkata)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/Nasko_Big.jpeg";
                    model.Description = "An elite Brawl Stars player with anger issues.";
                    model.Reviews = reviewsDictionary.ContainsKey(playerName) ? reviewsDictionary[playerName] : new List<Review>();
                    break;
                case "Boris (shefa na relefa)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/Bobi_Big.jpeg";
                    model.Description = "A strategic Brawl Stars player on whom you can rely to save your game.";
                    model.Reviews = reviewsDictionary.ContainsKey(playerName) ? reviewsDictionary[playerName] : new List<Review>();
                    break;
                case "Aryaan (Ary)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/Ary_Big.jpeg";
                    model.Description = "Renowned for precise aim and amazing trick-shots.";
                    model.Reviews = reviewsDictionary.ContainsKey(playerName) ? reviewsDictionary[playerName] : new List<Review>();
                    break;
                case "Nikolay (гоuemия)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/images/nikolay-large.png";
                    model.Description = "Dominates high-level matches with his tactical brilliance...";
                    model.Reviews = reviewsDictionary.ContainsKey(playerName) ? reviewsDictionary[playerName] : new List<Review>();
                    break;
                case "Maxi (Dragon)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/images/maxi-large.png";
                    model.Description = "Loves to surprise opponents with sneaky ambushes...";
                    model.Reviews = reviewsDictionary.ContainsKey(playerName) ? reviewsDictionary[playerName] : new List<Review>();
                    break;
                case "Vaseto (....)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/images/vaseto-large.png";
                    model.Description = "Specializes in support roles, ensuring team victories...";
                    model.Reviews = reviewsDictionary.ContainsKey(playerName) ? reviewsDictionary[playerName] : new List<Review>();
                    break;
                case "Christian (kryskata)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/images/christian-large.png";
                    model.Description = "New to the pro scene but improving rapidly...";
                    model.Reviews = reviewsDictionary.ContainsKey(playerName) ? reviewsDictionary[playerName] : new List<Review>();
                    break;
                default:
                    model.PlayerName = "Unknown Player";
                    model.BigImageUrl = "/images/unknown.png";
                    model.Description = "No information found.";
                    model.Reviews = new List<Review>
                    {
                        new Review { ReviewerName = "System", Comment = "No reviews available.", StarRating = 0 }
                    };
                    break;
            }
            return View(model);
        }
    }

    public class PlayerDetailViewModel
    {
        public string PlayerName { get; set; }
        public string BigImageUrl { get; set; }
        public string Description { get; set; }
        public List<Review> Reviews { get; set; }
    }

    public class Review
    {
        public string ReviewerName { get; set; }
        public string Comment { get; set; }
        public int StarRating { get; set; }
    }

    public class UserProfileViewModel
    {
        public string Username { get; set; }
        public DateTime UserSince { get; set; }
        public int TotalSpent { get; set; }
        public List<string> MostHiredPlayers { get; set; }
        public List<Review> Reviews { get; set; }
    }
}