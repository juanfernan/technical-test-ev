using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestEv.Application.DTOs;
using TestEv.Application.Interfaces;

namespace TestEv.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll(
            [FromQuery] string? status = null,
            [FromQuery] string? owner = null,
            CancellationToken cancellationToken = default)
        {
            var projects = await _projectService.GetAllProjectsAsync(status, owner, cancellationToken);
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetById(string id, CancellationToken cancellationToken = default)
        {
            var project = await _projectService.GetProjectByIdAsync(id, cancellationToken);
            if (project == null)
                return NotFound(new { message = $"Project with ID '{id}' not found." });
            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Create(
            [FromBody] CreateProjectRequest request,
            CancellationToken cancellationToken = default)
        {
            var project = await _projectService.CreateProjectAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDto>> Update(
            string id,
            [FromBody] UpdateProjectRequest request,
            CancellationToken cancellationToken = default)
        {
            var project = await _projectService.UpdateProjectAsync(id, request, cancellationToken);
            return Ok(project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
        {
            await _projectService.DeleteProjectAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}/stats")]
        public async Task<ActionResult<ProjectStatsDto>> GetStats(string id, CancellationToken cancellationToken = default)
        {
            var stats = await _projectService.GetProjectStatsAsync(id, cancellationToken);
            return Ok(stats);
        }
    }
}
