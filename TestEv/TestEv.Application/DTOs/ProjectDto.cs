using TestEv.Domain.Entities;

namespace TestEv.Application.DTOs
{
    public record ProjectDto(
    string Id,
    string Name,
    string Description,
    string OwnerId,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt)
    {
        public static ProjectDto FromEntity(Project project) => new(
            project.Id,
            project.Name,
            project.Description,
            project.OwnerId,
            project.Status.ToString().ToLowerInvariant(),
            project.CreatedAt,
            project.UpdatedAt);
    }

    public record CreateProjectRequest(
        string Name,
        string Description,
        string OwnerId,
        string Status);

    public record UpdateProjectRequest(
        string Name,
        string Description,
        string OwnerId,
        string Status);

    public record ProjectStatsDto(
        string ProjectId,
        string ProjectName,
        int DaysActive,
        DateTime LastUpdate,
        string Status,
        int DaysSinceLastUpdate);
}
