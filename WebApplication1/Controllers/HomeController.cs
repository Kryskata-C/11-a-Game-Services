using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
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
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Basket()
        {
            var basketJson = HttpContext.Session.GetString("Basket");
            var priceJson = HttpContext.Session.GetString("Prices");

            var basket = string.IsNullOrEmpty(basketJson)
                ? new Dictionary<string, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(basketJson);

            var prices = string.IsNullOrEmpty(priceJson)
                ? new Dictionary<string, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(priceJson);

            ViewBag.PlayersPrices = prices;

            return View(basket);
        }


        [HttpPost]
        public IActionResult AddToBasket(string playerName, int price)
        {
            var basketJson = HttpContext.Session.GetString("Basket");
            var basket = string.IsNullOrEmpty(basketJson) ? new Dictionary<string, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(basketJson);

            if (basket.ContainsKey(playerName))
            {
                basket[playerName]++;
            }
            else
            {
                basket[playerName] = 1;
            }
            HttpContext.Session.SetString("Basket", System.Text.Json.JsonSerializer.Serialize(basket));

            var priceJson = HttpContext.Session.GetString("Prices");
            var priceDict = string.IsNullOrEmpty(priceJson)
                ? new Dictionary<string, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(priceJson);

            priceDict[playerName] = price;
            HttpContext.Session.SetString("Prices", System.Text.Json.JsonSerializer.Serialize(priceDict));

            return RedirectToAction("Basket");
        }





        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "users.txt");

            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, "");
            }

            var lines = System.IO.File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (line.Contains($"user \"{username}\""))
                {
                    ViewBag.Message = "Username already taken.";
                    return View();
                }
            }

            string entry = $"user \"{username}\" {{ password: \"{password}\", since: \"{DateTime.Now:yyyy-MM-dd}\", spent: 0, mosthired: [], reviews: [] }}\n";
            System.IO.File.AppendAllText(filePath, entry);

            HttpContext.Session.SetString("Username", username);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "users.txt");

            if (!System.IO.File.Exists(filePath))
            {
                ViewBag.Message = "User storage not found.";
                return View();
            }

            var lines = System.IO.File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var userMatch = System.Text.RegularExpressions.Regex.Match(
                    line,
                    @"user\s+""(.+?)""\s+\{\s+password:\s+""(.+?)"",\s+since:\s+""(.+?)"",\s+spent:\s+(\d+),\s+mosthired:\s+\[(.*?)\],\s+reviews:\s+\[(.*?)\]\s+\}"
                );


                if (userMatch.Success)
                {
                    var storedUsername = userMatch.Groups[1].Value;
                    var storedPassword = userMatch.Groups[2].Value;

                    if (storedUsername == username && storedPassword == password)
                    {
                        HttpContext.Session.SetString("Username", username);
                        return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.Message = "Invalid username or password.";
            return View();
        }

        public new IActionResult User()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "users.txt");
            if (!System.IO.File.Exists(filePath)) return RedirectToAction("Login");

            var lines = System.IO.File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (line.Contains($"user \"{username}\""))
                {
                    var userMatch = System.Text.RegularExpressions.Regex.Match(
                         line,
                         @"user\s+""(.+?)""\s+\{\s+password:\s+""(.+?)"",\s+since:\s+""(.+?)"",\s+spent:\s+(\d+),\s+mosthired:\s+\[(.*?)\],\s+reviews:\s+\[(.*?)\]\s+\}"
                    );


                    if (userMatch.Success)
                    {
                        var model = new UserProfileViewModel
                        {
                            Username = userMatch.Groups[1].Value,
                            UserSince = DateTime.TryParse(userMatch.Groups[3].Value, out var date) ? date : DateTime.Now,
                            TotalSpent = int.Parse(userMatch.Groups[4].Value),
                            MostHiredPlayers = userMatch.Groups[5].Value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList(),
                            Reviews = new List<Review>()
                        };
                        return View(model);
                    }
                }
            }

            return RedirectToAction("Login");
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
                    model.Reviews = reviewsDictionary[playerName];
                    break;

                case "Boris (shefa na relefa)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/Bobi_Big.jpeg";
                    model.Description = "A strategic Brawl Stars player on whom you can rely to save your game.";
                    model.Reviews = reviewsDictionary[playerName];
                    break;

                case "Aryaan (Ary)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/Ary_Big.jpeg";
                    model.Description = "Renowned for precise aim and amazing trick-shots.";
                    model.Reviews = reviewsDictionary[playerName];
                    break;

                case "Nikolay (гоuemия)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/images/nikolay-large.png";
                    model.Description = "Dominates high-level matches with his tactical brilliance...";
                    model.Reviews = reviewsDictionary[playerName];
                    break;

                case "Maxi (Dragon)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/images/maxi-large.png";
                    model.Description = "Loves to surprise opponents with sneaky ambushes...";
                    model.Reviews = reviewsDictionary[playerName];
                    break;

                case "Vaseto (....)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/images/vaseto-large.png";
                    model.Description = "Specializes in support roles, ensuring team victories...";
                    model.Reviews = reviewsDictionary[playerName];
                    break;

                case "Christian (kryskata)":
                    model.PlayerName = playerName;
                    model.BigImageUrl = "/images/christian-large.png";
                    model.Description = "New to the pro scene but improving rapidly...";
                    model.Reviews = reviewsDictionary[playerName];
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
