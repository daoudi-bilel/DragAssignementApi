using DragAssignementApi.Data;
using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragAssignementApi.Services
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext _context;

        public MemberService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Member>> GetMembersByUserIdAsync(string userId)
        {
            return await _context.Members.Where(m => m.UserId == userId).ToListAsync();
        }

        public async Task<Member> AddMemberAsync(string userId, MemberDto memberDto)
        {
            var member = new Member
            {
                Name = memberDto.Name,
                Email = memberDto.Email,
                Status = memberDto.Status,
                IsOwner = memberDto.IsOwner,
                AvatarUrl = memberDto.AvatarUrl,
                UserId = userId
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return member;
        }

        public async Task<bool> UpdateMemberStatusAsync(string userId, int memberId, string status)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId && m.UserId == userId);
            if (member == null) return false;

            member.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMemberAsync(string userId, int memberId)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId && m.UserId == userId);
            if (member == null) return false;

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
