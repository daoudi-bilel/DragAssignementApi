// Controllers/ObjectiveController.cs
using DragAssignementApi.Models.DTO;
using DragAssignementApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DragAssignementApi.Controllers
{
    [Route("api/v1/objectives")]
    [ApiController]
    public class ObjectiveController : ControllerBase
    {
        private readonly IObjectiveService _objectiveService;

        public ObjectiveController(IObjectiveService objectiveService)
        {
            _objectiveService = objectiveService;
        }

        [HttpPost]
        public async Task<IActionResult> AddObjective([FromBody] ObjectiveDto objectiveDto)
        {
            var objective = await _objectiveService.AddObjectiveAsync(objectiveDto);
            return Ok(objective);
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetObjectivesByProjectId(int projectId)
        {
            var objectives = await _objectiveService.GetObjectivesByProjectIdAsync(projectId);
            return Ok(objectives);
        }
    }
}
