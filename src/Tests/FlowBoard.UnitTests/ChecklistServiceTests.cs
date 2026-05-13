using Moq;
using NUnit.Framework;
using FlowBoard.ChecklistService.Services;
using FlowBoard.ChecklistService.Interfaces;
using FlowBoard.ChecklistService.Entities;
using FlowBoard.ChecklistService.DTOs;
using AutoMapper;

namespace FlowBoard.UnitTests;

[TestFixture]
public class ChecklistServiceTests
{
    private Mock<IChecklistRepository> _repoMock;
    private Mock<IMapper> _mapperMock;
    private ChecklistServiceImpl _checklistService;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IChecklistRepository>();
        _mapperMock = new Mock<IMapper>();
        _checklistService = new ChecklistServiceImpl(_repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldWork()
    {
        var dto = new CreateChecklistDto { CardId = 1, Title = "Checklist" };
        _repoMock.Setup(r => r.GetByIdWithItemsAsync(It.IsAny<int>()))
            .ReturnsAsync(new Checklist { ChecklistId = 1, Title = "Checklist", Items = new List<ChecklistItem>() });

        var result = await _checklistService.CreateAsync(dto);

        Assert.That(result, Is.Not.Null);
    }
}
