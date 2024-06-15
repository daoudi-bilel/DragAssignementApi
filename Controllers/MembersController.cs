using DragAssignementApi.Models.DTO;
using DragAssignementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DragAssignementApi.Controllers
{
    [Authorize]
    [Route("api/v1/members")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMembers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var members = await _memberService.GetMembersByUserIdAsync(userId);
            return Ok(members);
        }

        [HttpPost]
        public async Task<IActionResult> AddMember([FromBody] MemberDto memberDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { message = "User not authorized" });
            }

            var member = await _memberService.AddMemberAsync(userId, memberDto);
            return Ok(member);
        }

        [HttpPut("{memberId}/status")]
        public async Task<IActionResult> UpdateMemberStatus(int memberId, [FromBody] string status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _memberService.UpdateMemberStatusAsync(userId, memberId, status);
            if (!result) return NotFound(new { message = "Member not found" });
            return Ok(new { message = "Member status updated successfully" });
        }

        [HttpDelete("{memberId}")]
        public async Task<IActionResult> DeleteMember(int memberId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _memberService.DeleteMemberAsync(userId, memberId);
            if (!result) return NotFound(new { message = "Member not found" });
            return Ok(new { message = "Member deleted successfully" });
        }
    }
}
