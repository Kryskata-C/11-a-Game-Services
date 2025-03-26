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

        public IActionResult PlayerInfo(string playerName)
        {
            var reviewsDictionary = new Dictionary<string, List<Review>>
            {
                { "Atanas (Cenkata)", new List<Review>
                    { new Review { ReviewerName = "ProGamer", Comment = "Elite player, but gets angry fast!", StarRating = 4 },
                      new Review { ReviewerName = "NoobSlayer", Comment = "Carries games effortlessly!", StarRating = 5 } }
                },
                { "Boris (shefa na relefa)", new List<Review>
                    { new Review { ReviewerName = "TryHard", Comment = "Always has a game plan!", StarRating = 5 },
                      new Review { ReviewerName = "CasualMaster", Comment = "Good teammate, but expensive!", StarRating = 4 } }
                }
            };

            var model = new PlayerDetailViewModel
            {
                PlayerName = playerName,
                BigImageUrl = "/images/default.png",
                Description = "No information found.",
                Reviews = reviewsDictionary.ContainsKey(playerName) ? reviewsDictionary[playerName] : new List<Review>
                {
                    new Review { ReviewerName = "System", Comment = "No reviews available.", StarRating = 0 }
                }
            };

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
    