using Moq;
using NUnit.Framework;
using FlowBoard.CommentService.Services;
using FlowBoard.CommentService.Interfaces;
using FlowBoard.CommentService.Entities;
using FlowBoard.CommentService.DTOs;
using AutoMapper;

namespace FlowBoard.UnitTests;

[TestFixture]
public class CommentServiceTests
{
    private Mock<ICommentRepository> _repoMock;
    private Mock<IMapper> _mapperMock;
    private CommentServiceImpl _commentService;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<ICommentRepository>();
        _mapperMock = new Mock<IMapper>();
        _commentService = new CommentServiceImpl(_repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldWork()
    {
        int userId = 1;
        var dto = new CreateCommentDto { CardId = 1, Content = "Nice!" };
        _mapperMock.Setup(m => m.Map<CommentDto>(It.IsAny<Comment>()))
            .Returns(new CommentDto { Content = dto.Content });

        var result = await _commentService.CreateAsync(dto, userId);

        Assert.That(result, Is.Not.Null);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Comment>()), Times.Once);
    }
}
