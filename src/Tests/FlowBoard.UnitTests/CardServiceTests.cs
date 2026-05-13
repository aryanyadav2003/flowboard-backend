using Moq;
using NUnit.Framework;
using FlowBoard.CardService.Services;
using FlowBoard.CardService.Interfaces;
using FlowBoard.CardService.Entities;
using FlowBoard.CardService.DTOs;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace FlowBoard.UnitTests;

[TestFixture]
public class CardServiceTests
{
    private Mock<ICardRepository> _repoMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IDistributedCache> _cacheMock;
    private Mock<ILogger<CacheService>> _loggerMock;
    private CacheService _cacheService;
    private CardServiceImpl _cardService;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<ICardRepository>();
        _mapperMock = new Mock<IMapper>();
        _cacheMock = new Mock<IDistributedCache>();
        _loggerMock = new Mock<ILogger<CacheService>>();
        
        _cacheService = new CacheService(_cacheMock.Object, _loggerMock.Object);
        _cardService = new CardServiceImpl(_repoMock.Object, _mapperMock.Object, _cacheService);
    }

    [Test]
    public async Task GetMyTasksAsync_ShouldReturnTasks()
    {
        // Arrange
        int userId = 1;
        var cards = new List<Card> { new Card { CardId = 1, Title = "Task 1" } };
        _repoMock.Setup(r => r.GetCardsByAssigneeAsync(userId)).ReturnsAsync(cards);
        _mapperMock.Setup(m => m.Map<List<CardDto>>(It.IsAny<List<Card>>()))
            .Returns(new List<CardDto> { new CardDto { Title = "Task 1" } });

        // Act
        var result = await _cardService.GetMyTasksAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
    }
}
