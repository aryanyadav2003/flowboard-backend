using Moq;
using NUnit.Framework;
using FlowBoard.WorkspaceService.Services;
using FlowBoard.WorkspaceService.Interfaces;
using FlowBoard.WorkspaceService.Entities;
using FlowBoard.WorkspaceService.DTOs;
using AutoMapper;

namespace FlowBoard.UnitTests;

[TestFixture]
public class WorkspaceServiceTests
{
    private Mock<IWorkspaceRepository> _repoMock;
    private Mock<IMapper> _mapperMock;
    private WorkspaceServiceImpl _workspaceService;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IWorkspaceRepository>();
        _mapperMock = new Mock<IMapper>();
        _workspaceService = new WorkspaceServiceImpl(_repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldWork()
    {
        // Arrange
        int ownerId = 1;
        var dto = new CreateWorkspaceDto { Name = "Test" };
        _repoMock.Setup(r => r.GetByIdWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(new Workspace { WorkspaceId = 1, Name = "Test" });

        // Act
        var result = await _workspaceService.CreateAsync(ownerId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
    }
}
