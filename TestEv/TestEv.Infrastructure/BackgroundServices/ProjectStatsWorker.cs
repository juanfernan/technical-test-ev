using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using TestEv.Domain.Interfaces;

namespace TestEv.Infrastructure.BackgroundServices
{
    public class ProjectStatsWorker : BackgroundService
    {
        private readonly ILogger<ProjectStatsWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public ProjectStatsWorker(
            ILogger<ProjectStatsWorker> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Project Stats Worker started at: {Time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessProjectStatsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing project statistics");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Project Stats Worker stopped at: {Time}", DateTimeOffset.Now);
        }

        private async Task ProcessProjectStatsAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Processing project statistics at: {Time}", DateTimeOffset.Now);

            using var scope = _serviceProvider.CreateScope();
            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();

            var projects = await projectRepository.GetAllAsync(stoppingToken);
            var projectList = projects.ToList();

            var stats = new
            {
                TotalProjects = projectList.Count,
                ActiveProjects = projectList.Count(p => p.Status == Domain.Entities.ProjectStatus.Active),
                CompletedProjects = projectList.Count(p => p.Status == Domain.Entities.ProjectStatus.Completed),
                InactiveProjects = projectList.Count(p => p.Status == Domain.Entities.ProjectStatus.Inactive),
                ArchivedProjects = projectList.Count(p => p.Status == Domain.Entities.ProjectStatus.Archived)
            };

            _logger.LogInformation(
                "Project Stats: Total={Total}, Active={Active}, Completed={Completed}, Inactive={Inactive}, Archived={Archived}",
                stats.TotalProjects,
                stats.ActiveProjects,
                stats.CompletedProjects,
                stats.InactiveProjects,
                stats.ArchivedProjects);
        }
    }


}
