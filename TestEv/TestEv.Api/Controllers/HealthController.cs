using Microsoft.AspNetCore.Mvc;

namespace TestEv.Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new HealthResponse(
                Status: "Healthy",
                Timestamp: DateTime.UtcNow,
                Version: typeof(HealthController).Assembly.GetName().Version?.ToString() ?? "1.0.0"));
        }
    }

    public record HealthResponse(string Status, DateTime Timestamp, string Version);
}
