using System.Collections.Concurrent;
using TestEv.Domain.Entities;
using TestEv.Domain.Interfaces;

namespace TestEv.Infrastructure.Persistence
{
    public class InMemoryProjectRepository : IProjectRepository
    {
        private readonly ConcurrentDictionary<string, Project> _projects = new();

        public Task<Project?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            _projects.TryGetValue(id, out var project);
            return Task.FromResult(project);
        }

        public Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_projects.Values.AsEnumerable());
        }

        public Task<IEnumerable<Project>> GetByFilterAsync(string? status, string? ownerId, CancellationToken cancellationToken = default)
        {
            var query = _projects.Values.AsEnumerable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProjectStatus>(status, true, out var statusEnum))
            {
                query = query.Where(p => p.Status == statusEnum);
            }

            if (!string.IsNullOrEmpty(ownerId))
            {
                query = query.Where(p => p.OwnerId == ownerId);
            }

            return Task.FromResult(query);
        }

        public Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default)
        {
            _projects[project.Id] = project;
            return Task.FromResult(project);
        }

        public Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default)
        {
            _projects[project.Id] = project;
            return Task.FromResult(project);
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            _projects.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_projects.ContainsKey(id));
        }
    }
}
