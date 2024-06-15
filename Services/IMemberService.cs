using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragAssignementApi.Services
{
    public interface IMemberService
    {
        Task<IEnumerable<Member>> GetMembersByUserIdAsync(string userId);
        Task<Member> AddMemberAsync(string userId, MemberDto memberDto);
        Task<bool> UpdateMemberStatusAsync(string userId, int memberId, string status);
        Task<bool> DeleteMemberAsync(string userId, int memberId);
    }
}
