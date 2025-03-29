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

        // ... other actions unchanged ...
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
