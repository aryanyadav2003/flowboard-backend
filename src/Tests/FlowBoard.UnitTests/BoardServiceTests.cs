using Moq;
using NUnit.Framework;
using FlowBoard.BoardService.Services;
using FlowBoard.BoardService.Interfaces;
using FlowBoard.BoardService.Entities;
using FlowBoard.BoardService.DTOs;
using AutoMapper;

namespace FlowBoard.UnitTests;

[TestFixture]
public class BoardServiceTests
{
    private Mock<IBoardRepository> _repoMock;
    private Mock<IMapper> _mapperMock;
    private BoardServiceImpl _boardService;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IBoardRepository>();
        _mapperMock = new Mock<IMapper>();
        _boardService = new BoardServiceImpl(_repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldWork()
    {
        int userId = 1;
        var dto = new CreateBoardDto { Name = "New Board", WorkspaceId = 1 };
        _repoMock.Setup(r => r.GetByIdWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(new Board { BoardId = 1, Name = "New Board" });

        var result = await _boardService.CreateAsync(userId, dto);

        Assert.That(result, Is.Not.Null);
    }
}
