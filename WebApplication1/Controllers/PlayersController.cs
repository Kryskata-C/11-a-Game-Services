using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Services;

public class PlayersController : Controller
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    public async Task<IActionResult> Index()
    {
        var players = await _playerService.GetAllPlayersAsync();
        return View(players);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var player = await _playerService.GetPlayerByIdAsync(id.Value);
        if (player == null)
        {
            return NotFound();
        }
        return View(player);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("GamerTag,Description,Email,Level,ExperiencePoints,VirtualCurrencyBalance,ApplicationUserId")] Player player)
    {
        if (ModelState.IsValid)
        {
            await _playerService.CreatePlayerAsync(player);
            return RedirectToAction(nameof(Index));
        }
        return View(player);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var player = await _playerService.GetPlayerByIdAsync(id.Value);
        if (player == null)
        {
            return NotFound();
        }
        return View(player);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,GamerTag,Description,Email,RegistrationDate,Level,ExperiencePoints,VirtualCurrencyBalance,ApplicationUserId")] Player player)
    {
        if (id != player.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var updated = await _playerService.UpdatePlayerAsync(player);
            if (!updated)
            {
                if (!await _playerService.PlayerExistsAsync(player.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred updating the player. The player might have been modified or deleted by another user.");
                    return View(player);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(player);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var player = await _playerService.GetPlayerByIdAsync(id.Value);
        if (player == null)
        {
            return NotFound();
        }
        return View(player);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _playerService.DeletePlayerAsync(id);
        return RedirectToAction(nameof(Index));
    }
}