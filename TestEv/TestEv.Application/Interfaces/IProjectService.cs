using TestEv.Application.DTOs;

namespace TestEv.Application.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(string? status = null, string? ownerId = null, CancellationToken cancellationToken = default);
        Task<ProjectDto?> GetProjectByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
        Task<ProjectDto> UpdateProjectAsync(string id, UpdateProjectRequest request, CancellationToken cancellationToken = default);
        Task DeleteProjectAsync(string id, CancellationToken cancellationToken = default);
        Task<ProjectStatsDto> GetProjectStatsAsync(string id, CancellationToken cancellationToken = default);
    }   
}
