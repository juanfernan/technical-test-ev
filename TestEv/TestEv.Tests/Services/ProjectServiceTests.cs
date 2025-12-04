using FluentAssertions;
using Moq;
using TestEv.Application.DTOs;
using TestEv.Application.Services;
using TestEv.Domain.Entities;
using TestEv.Domain.Exceptions;
using TestEv.Domain.Interfaces;

namespace TestEv.Tests.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly ProjectService _sut;

        public ProjectServiceTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _sut = new ProjectService(_projectRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllProjectsAsync_ReturnsAllProjects()
        {
            var projects = new List<Project>
        {
            CreateTestProject("1", "Project 1", "user-1", ProjectStatus.Active),
            CreateTestProject("2", "Project 2", "user-2", ProjectStatus.Completed)
        };
            _projectRepositoryMock.Setup(x => x.GetByFilterAsync(null, null, It.IsAny<CancellationToken>())).ReturnsAsync(projects);

            var result = await _sut.GetAllProjectsAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProjectByIdAsync_WithValidId_ReturnsProject()
        {
            var project = CreateTestProject("test-id", "Test Project", "user-1", ProjectStatus.Active);
            _projectRepositoryMock.Setup(x => x.GetByIdAsync("test-id", It.IsAny<CancellationToken>())).ReturnsAsync(project);

            var result = await _sut.GetProjectByIdAsync("test-id");

            result.Should().NotBeNull();
            result!.Id.Should().Be("test-id");
        }

        [Fact]
        public async Task GetProjectByIdAsync_WithInvalidId_ReturnsNull()
        {
            _projectRepositoryMock.Setup(x => x.GetByIdAsync("invalid-id", It.IsAny<CancellationToken>())).ReturnsAsync((Project?)null);

            var result = await _sut.GetProjectByIdAsync("invalid-id");

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateProjectAsync_WithValidRequest_ReturnsCreatedProject()
        {
            var request = new CreateProjectRequest("New Project", "Description", "user-123", "active");
            _projectRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>())).ReturnsAsync((Project p, CancellationToken _) => p);

            var result = await _sut.CreateProjectAsync(request);

            result.Should().NotBeNull();
            result.Name.Should().Be("New Project");
            result.Status.Should().Be("active");
        }

        [Fact]
        public async Task CreateProjectAsync_WithInvalidStatus_ThrowsValidationException()
        {
            var request = new CreateProjectRequest("Project", "Desc", "user-123", "invalid_status");

            var act = () => _sut.CreateProjectAsync(request);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateProjectAsync_WithValidRequest_ReturnsUpdatedProject()
        {
            var existingProject = CreateTestProject("test-id", "Old Name", "user-1", ProjectStatus.Active);
            var request = new UpdateProjectRequest("Updated Name", "Updated Desc", "user-2", "completed");
            _projectRepositoryMock.Setup(x => x.GetByIdAsync("test-id", It.IsAny<CancellationToken>())).ReturnsAsync(existingProject);
            _projectRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>())).ReturnsAsync((Project p, CancellationToken _) => p);

            var result = await _sut.UpdateProjectAsync("test-id", request);

            result.Name.Should().Be("Updated Name");
            result.Status.Should().Be("completed");
        }

        [Fact]
        public async Task UpdateProjectAsync_WithNonExistentId_ThrowsEntityNotFoundException()
        {
            var request = new UpdateProjectRequest("Name", "Desc", "owner", "active");
            _projectRepositoryMock.Setup(x => x.GetByIdAsync("non-existent", It.IsAny<CancellationToken>())).ReturnsAsync((Project?)null);

            var act = () => _sut.UpdateProjectAsync("non-existent", request);

            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task DeleteProjectAsync_WithValidId_DeletesProject()
        {
            _projectRepositoryMock.Setup(x => x.ExistsAsync("test-id", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await _sut.DeleteProjectAsync("test-id");

            _projectRepositoryMock.Verify(x => x.DeleteAsync("test-id", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProjectAsync_WithNonExistentId_ThrowsEntityNotFoundException()
        {
            _projectRepositoryMock.Setup(x => x.ExistsAsync("non-existent", It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var act = () => _sut.DeleteProjectAsync("non-existent");

            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private static Project CreateTestProject(string id, string name, string ownerId, ProjectStatus status)
        {
            return Project.Hydrate(id, name, "Test Description", ownerId, status, DateTime.UtcNow, DateTime.UtcNow);
        }
    }
}
