using NUnit.Framework;
using Moq;
using WebApplication1.Services;
using WebApplication1.Models;
using WebApplication1.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Numerics;

[TestFixture]
public class PlayerServiceTests
{
    private Mock<ApplicationDbContext> _mockDbContext;
    private Mock<DbSet<Player>> _mockPlayersDbSet;
    private PlayerService _playerService;
    private List<Player> _playersData;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _mockDbContext = new Mock<ApplicationDbContext>(options);

        _playersData = new List<Player>
        {
            new Player { Id = 1, GamerTag = "Hero1", PricePerHour = 10, Rating = 4.5, TeamId = 1, Description = "Alpha team player" },
            new Player { Id = 2, GamerTag = "Hero2", PricePerHour = 20, Rating = 4.0, TeamId = null, Description = "Solo player" },
            new Player { Id = 3, GamerTag = "Hero3", PricePerHour = 15, Rating = 4.2, TeamId = 1, Description = "Another Alpha player" }
        };

        _mockPlayersDbSet = new Mock<DbSet<Player>>();

        _mockPlayersDbSet.As<IQueryable<Player>>().Setup(m => m.Provider).Returns(_playersData.AsQueryable().Provider);
        _mockPlayersDbSet.As<IQueryable<Player>>().Setup(m => m.Expression).Returns(_playersData.AsQueryable().Expression);
        _mockPlayersDbSet.As<IQueryable<Player>>().Setup(m => m.ElementType).Returns(_playersData.AsQueryable().ElementType);
        _mockPlayersDbSet.As<IQueryable<Player>>().Setup(m => m.GetEnumerator()).Returns(() => _playersData.AsQueryable().GetEnumerator());

        _mockPlayersDbSet.Setup(s => s.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync((object[] ids) => _playersData.FirstOrDefault(p => p.Id == (int)ids[0]));

        _mockPlayersDbSet.Setup(s => s.Add(It.IsAny<Player>())).Callback<Player>((player) => _playersData.Add(player));
        _mockPlayersDbSet.Setup(s => s.Remove(It.IsAny<Player>())).Callback<Player>((player) => _playersData.Remove(player));

        _mockDbContext.Setup(c => c.Players).Returns(_mockPlayersDbSet.Object);
        _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockDbContext.Setup(m => m.Set<Player>()).Returns(_mockPlayersDbSet.Object);


        _playerService = new PlayerService(_mockDbContext.Object);
    }

    [Test]
    public async Task GetPlayerByIdAsync_WhenPlayerExists_ReturnsPlayer()
    {
        int existingPlayerId = 1;
        var result = await _playerService.GetPlayerByIdAsync(existingPlayerId);
        Assert.IsNotNull(result);
        Assert.AreEqual(existingPlayerId, result.Id);
        Assert.AreEqual("Hero1", result.GamerTag);
    }

    [Test]
    public async Task GetPlayerByIdAsync_WhenPlayerDoesNotExist_ReturnsNull()
    {
        int nonExistingPlayerId = 99;
        _mockPlayersDbSet.Setup(s => s.FindAsync(new object[] { nonExistingPlayerId }))
            .ReturnsAsync((Player)null);
        var result = await _playerService.GetPlayerByIdAsync(nonExistingPlayerId);
        Assert.IsNull(result);
    }

    [Test]
    public async Task CreatePlayerAsync_AddsPlayerAndSavesChanges()
    {
        var newPlayer = new Player { Id = 4, GamerTag = "Hero4", PricePerHour = 25, Rating = 3.9 };
        await _playerService.CreatePlayerAsync(newPlayer);
        _mockPlayersDbSet.Verify(m => m.Add(It.IsAny<Player>()), Times.Once());
        _mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        Assert.Contains(newPlayer, _playersData);
    }

    [Test]
    public async Task UpdatePlayerAsync_WhenPlayerExists_UpdatesPlayerAndSavesChanges_ReturnsTrue()
    {
        var existingPlayer = new Player { Id = 1, GamerTag = "Hero1Updated", PricePerHour = 12, Rating = 4.6 };

        var playerFromDb = _playersData.First(p => p.Id == existingPlayer.Id);
        _mockDbContext.Setup(c => c.Entry(playerFromDb)).Returns(() => {
            var entryMock = new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Player>>();
            entryMock.Setup(e => e.CurrentValues).Returns(new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.PropertyValues>().Object);
            return entryMock.Object;
        });

        var result = await _playerService.UpdatePlayerAsync(existingPlayer);

        Assert.IsTrue(result);
        _mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        var updatedPlayerInDb = _playersData.First(p => p.Id == existingPlayer.Id);
        Assert.AreEqual(existingPlayer.GamerTag, updatedPlayerInDb.GamerTag);
        Assert.AreEqual(existingPlayer.PricePerHour, updatedPlayerInDb.PricePerHour);
    }

    [Test]
    public async Task UpdatePlayerAsync_WhenPlayerDoesNotExist_ReturnsFalse()
    {
        var nonExistingPlayer = new Player { Id = 99, GamerTag = "NonExistent" };
        _mockPlayersDbSet.Setup(s => s.FindAsync(new object[] { nonExistingPlayer.Id })).ReturnsAsync((Player)null);
        var result = await _playerService.UpdatePlayerAsync(nonExistingPlayer);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task DeletePlayerAsync_WhenPlayerExists_RemovesPlayerAndSavesChanges_ReturnsTrue()
    {
        int existingPlayerId = 1;
        var result = await _playerService.DeletePlayerAsync(existingPlayerId);
        Assert.IsTrue(result);
        _mockPlayersDbSet.Verify(m => m.Remove(It.Is<Player>(p => p.Id == existingPlayerId)), Times.Once());
        _mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        Assert.IsNull(_playersData.FirstOrDefault(p => p.Id == existingPlayerId));
    }

    [Test]
    public async Task DeletePlayerAsync_WhenPlayerDoesNotExist_ReturnsFalse()
    {
        int nonExistingPlayerId = 99;
        _mockPlayersDbSet.Setup(s => s.FindAsync(new object[] { nonExistingPlayerId })).ReturnsAsync((Player)null);
        var result = await _playerService.DeletePlayerAsync(nonExistingPlayerId);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task PlayerExistsAsync_WhenPlayerExists_ReturnsTrue()
    {
        _mockPlayersDbSet.As<IAsyncEnumerable<Player>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Player>(_playersData.Where(p => p.Id == 1).GetEnumerator()));

        var result = await _playerService.PlayerExistsAsync(1);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task PlayerExistsAsync_WhenPlayerDoesNotExist_ReturnsFalse()
    {
        _mockPlayersDbSet.As<IAsyncEnumerable<Player>>()
           .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
           .Returns(new TestAsyncEnumerator<Player>(_playersData.Where(p => p.Id == 99).GetEnumerator()));

        var result = await _playerService.PlayerExistsAsync(99);
        Assert.IsFalse(result);
    }

    [Test]
    public async Task GetAllPlayersAsync_NoSearchOrSort_ReturnsAllPlayersFirstPage()
    {
        var (players, totalCount) = await _playerService.GetAllPlayersAsync(null, "gamertag_asc", 1, 2);
        Assert.AreEqual(2, players.Count());
        Assert.AreEqual(3, totalCount);
        Assert.AreEqual("Hero1", players.First().GamerTag);
    }

    [Test]
    public async Task GetAllPlayersAsync_WithSearchTerm_ReturnsMatchingPlayers()
    {
        var (players, totalCount) = await _playerService.GetAllPlayersAsync("Alpha", "gamertag_asc", 1, 10);
        Assert.AreEqual(2, players.Count());
        Assert.AreEqual(2, totalCount);
        Assert.IsTrue(players.All(p => p.Description.Contains("Alpha")));
    }

    [Test]
    public async Task GetAllPlayersAsync_WithSortOrderPriceDesc_ReturnsSortedPlayers()
    {
        var (players, totalCount) = await _playerService.GetAllPlayersAsync(null, "price_desc", 1, 10);
        Assert.AreEqual(3, players.Count());
        Assert.AreEqual(3, totalCount);
        Assert.AreEqual("Hero2", players.First().GamerTag);
        Assert.AreEqual(20, players.First().PricePerHour);
    }

    [Test]
    public async Task GetAllPlayersAsync_Pagination_ReturnsCorrectPage()
    {
        var (players, totalCount) = await _playerService.GetAllPlayersAsync(null, "gamertag_asc", 2, 1);
        Assert.AreEqual(1, players.Count());
        Assert.AreEqual(3, totalCount);
        Assert.AreEqual("Hero2", players.First().GamerTag);
    }
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _enumerator;
    public T Current => _enumerator.Current;
    public TestAsyncEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;
    public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);
    public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_enumerator.MoveNext());
}