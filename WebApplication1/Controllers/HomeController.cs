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
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string storedUsername = parts[0];
                    string storedPassword = parts[1];

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



        public IActionResult Basket(string playerName)
        {
            var playersPrices = new Dictionary<string, int>
    {
        {"Atanas (Cenkata)", 3500},
        {"Boris (shefa na relefa)", 2800},
        {"Aryaan (Ary)", 3000},
        {"Nikolay (гоuemия)", 5000},
        {"Maxi (Dragon)", 100},
        {"Vaseto (....)", 1000},
        {"Christian (kryskata)", 10}
    };

            var basket = HttpContext.Session.GetObject<Dictionary<string, int>>("BasketItems") ?? new Dictionary<string, int>();

            if (!string.IsNullOrEmpty(playerName))
            {
                if (basket.ContainsKey(playerName))
                    basket[playerName]++;
                else
                    basket[playerName] = 1;

                HttpContext.Session.SetObject("BasketItems", basket);
            }

            ViewBag.PlayersPrices = playersPrices;

            return View(basket);
        }




        public IActionResult PlayerInfo(string playerName)
        {
            var reviewsDictionary = new Dictionary<string, List<Review>>
            {
                {
                    "Atanas (Cenkata)", new List<Review>
                    {
                        new Review { ReviewerName = "ProGamer", Comment = "Elite player, but gets angry fast!", StarRating = 4 },
                        new Review { ReviewerName = "NoobSlayer", Comment = "Carries games effortlessly!", StarRating = 5 }
                    }
                },
                {
                    "Boris (shefa na relefa)", new List<Review>
                    {
                        new Review { ReviewerName = "TryHard", Comment = "Always has a game plan!", StarRating = 5 },
                        new Review { ReviewerName = "CasualMaster", Comment = "Good teammate, but expensive!", StarRating = 4 }
                    }
                },
                {
                    "Aryaan (Ary)", new List<Review>
                    {
                        new Review { ReviewerName = "EliteShooter", Comment = "Best aim I've seen!", StarRating = 5 },
                        new Review { ReviewerName = "Tactician", Comment = "Knows all the trick shots!", StarRating = 5 }
                    }
                },
                {
                    "Nikolay (гоuemия)", new List<Review>
                    {
                        new Review { ReviewerName = "Strategist", Comment = "Always thinking 3 steps ahead!", StarRating = 5 },
                        new Review { ReviewerName = "CasualGamer", Comment = "A bit too serious but solid player!", StarRating = 4 }
                    }
                },
                {
                    "Maxi (Dragon)", new List<Review>
                    {
                        new Review { ReviewerName = "ShadowAssassin", Comment = "Stealth skills are insane!", StarRating = 5 },
                        new Review { ReviewerName = "Nightmare", Comment = "Never see him coming!", StarRating = 5 }
                    }
                },
                {
                    "Vaseto (....)", new List<Review>
                    {
                        new Review { ReviewerName = "SupportMain", Comment = "Always got my back!", StarRating = 5 },
                        new Review { ReviewerName = "TeamPlayer", Comment = "Plays for the win!", StarRating = 4 }
                    }
                },
                {
                    "Christian (kryskata)", new List<Review>
                    {
                        new Review { ReviewerName = "RookieStar", Comment = "Still learning, but improving fast!", StarRating = 3 },
                        new Review { ReviewerName = "Underdog", Comment = "Surprisingly good for a new player!", StarRating = 4 }
                    }
                }
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
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
}
