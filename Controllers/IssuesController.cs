// Controllers/IssueController.cs
using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using DragAssignementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DragAssignementApi.Controllers
{
    [Authorize]
    [Route("api/v1/issues")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly IIssueService _issueService;

        public IssueController(IIssueService issueService)
        {
            _issueService = issueService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetIssuesByProjectId(int projectId)
        {
            var issues = await _issueService.GetIssuesByProjectIdAsync(projectId);
            return Ok(issues);
        }

        [HttpPost]
        public async Task<IActionResult> CreateIssue([FromBody] Issue issueDto)
        {
            try
            {
                var issue = new Issue
                {
                    Priority = issueDto.Priority,
                    Type = issueDto.Type,
                    Description = issueDto.Description,
                    ProjectId = issueDto.ProjectId
                };

                await _issueService.AddIssueAsync(issue);
                return Ok(issue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{issueId}")]
        public async Task<IActionResult> DeleteIssue(int issueId)
        {
            var result = await _issueService.DeleteIssueAsync(issueId);
            if (!result) return NotFound(new { message = "Issue not found" });
            return Ok(new { message = "Issue deleted successfully" });
        }
    }
}
