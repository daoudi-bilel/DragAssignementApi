using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using DragAssignementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DragAssignementApi.Controllers
{
    [Authorize]
    [Route("api/v1/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProjects()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
              Console.WriteLine($"User ID: {userId}");
            var projects = await _projectService.GetProjectsByUserIdAsync(userId);
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject([FromBody] ProjectDto projectDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { message = "User not authorized" });
            }

            var project = await _projectService.AddProjectAsync(userId, projectDto);
            return Ok(project);
        }


        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _projectService.DeleteProjectAsync(userId, projectId);
            if (!result) return NotFound(new { message = "Project not found" });
            return Ok(new { message = "Project deleted successfully" });
        }
    }
}
