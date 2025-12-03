using TestEv.Application.DTOs;
using TestEv.Application.Interfaces;
using TestEv.Domain.Entities;
using TestEv.Domain.Exceptions;
using TestEv.Domain.Interfaces;

namespace TestEv.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(
            string? status = null,
            string? ownerId = null,
            CancellationToken cancellationToken = default)
        {
            var projects = await _projectRepository.GetByFilterAsync(status, ownerId, cancellationToken);
            return projects.Select(ProjectDto.FromEntity);
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
            return project == null ? null : ProjectDto.FromEntity(project);
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
        {
            if (!Enum.TryParse<ProjectStatus>(request.Status, true, out var status))
            {
                throw new ValidationException("status", $"Invalid status value. Valid values are: {string.Join(", ", Enum.GetNames<ProjectStatus>())}");
            }

            var project = Project.Create(
                request.Name,
                request.Description,
                request.OwnerId,
                status);

            var createdProject = await _projectRepository.CreateAsync(project, cancellationToken);
            return ProjectDto.FromEntity(createdProject);
        }

        public async Task<ProjectDto> UpdateProjectAsync(string id, UpdateProjectRequest request, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new EntityNotFoundException("Project", id);

            if (!Enum.TryParse<ProjectStatus>(request.Status, true, out var status))
            {
                throw new ValidationException("status", $"Invalid status value. Valid values are: {string.Join(", ", Enum.GetNames<ProjectStatus>())}");
            }

            project.Update(request.Name, request.Description, request.OwnerId, status);

            var updatedProject = await _projectRepository.UpdateAsync(project, cancellationToken);
            return ProjectDto.FromEntity(updatedProject);
        }

        public async Task DeleteProjectAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!await _projectRepository.ExistsAsync(id, cancellationToken))
                throw new EntityNotFoundException("Project", id);

            await _projectRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<ProjectStatsDto> GetProjectStatsAsync(string id, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new EntityNotFoundException("Project", id);

            var now = DateTime.UtcNow;
            var daysActive = (int)(now - project.CreatedAt).TotalDays;
            var daysSinceLastUpdate = (int)(now - project.UpdatedAt).TotalDays;

            return new ProjectStatsDto(
                project.Id,
                project.Name,
                daysActive,
                project.UpdatedAt,
                project.Status.ToString().ToLowerInvariant(),
                daysSinceLastUpdate);
        }
    }
}
