using System;
using System.Collections.Generic;
using System.Text;
using TestEv.Domain.Entities;

namespace TestEv.Domain.Interfaces
{

    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetByFilterAsync(string? status, string? ownerId, CancellationToken cancellationToken = default);
        Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default);
        Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    }
}
