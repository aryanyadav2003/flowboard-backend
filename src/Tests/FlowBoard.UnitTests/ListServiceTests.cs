using Moq;
using NUnit.Framework;
using FlowBoard.ListService.Services;
using FlowBoard.ListService.Interfaces;
using FlowBoard.ListService.Entities;
using FlowBoard.ListService.DTOs;
using AutoMapper;

namespace FlowBoard.UnitTests;

[TestFixture]
public class ListServiceTests
{
    private Mock<IListRepository> _repoMock;
    private Mock<IMapper> _mapperMock;
    private ListServiceImpl _listService;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IListRepository>();
        _mapperMock = new Mock<IMapper>();
        _listService = new ListServiceImpl(_repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldWork()
    {
        var dto = new CreateListDto { Name = "New List", BoardId = 1 };
        _repoMock.Setup(r => r.GetMaxPositionAsync(dto.BoardId)).ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<TaskListDto>(It.IsAny<TaskList>()))
            .Returns(new TaskListDto { Name = dto.Name });

        var result = await _listService.CreateAsync(dto, 1);

        Assert.That(result, Is.Not.Null);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<TaskList>()), Times.Once);
    }
}
