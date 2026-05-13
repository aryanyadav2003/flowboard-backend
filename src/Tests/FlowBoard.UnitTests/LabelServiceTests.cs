using Moq;
using NUnit.Framework;
using FlowBoard.LabelService.Services;
using FlowBoard.LabelService.Interfaces;
using FlowBoard.LabelService.Entities;
using FlowBoard.LabelService.DTOs;
using AutoMapper;

namespace FlowBoard.UnitTests;

[TestFixture]
public class LabelServiceTests
{
    private Mock<ILabelRepository> _repoMock;
    private Mock<IMapper> _mapperMock;
    private LabelServiceImpl _labelService;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<ILabelRepository>();
        _mapperMock = new Mock<IMapper>();
        _labelService = new LabelServiceImpl(_repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldWork()
    {
        var dto = new CreateLabelDto { BoardId = 1, Name = "Urgent", Color = "Red" };
        _mapperMock.Setup(m => m.Map<LabelDto>(It.IsAny<Label>()))
            .Returns(new LabelDto { Name = dto.Name });

        var result = await _labelService.CreateAsync(dto);

        Assert.That(result, Is.Not.Null);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Label>()), Times.Once);
    }
}
