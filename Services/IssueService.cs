using DragAssignementApi.Data;
using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragAssignementApi.Services
{
    public class IssueService : IIssueService
    {
        private readonly ApplicationDbContext _context;

        public IssueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Issue>> GetIssuesByProjectIdAsync(int projectId)
        {
            return await _context.Issues.Where(i => i.ProjectId == projectId).ToListAsync();
        }

        public async Task<Issue> AddIssueAsync(Issue issueDto)
        {
            var issue = new Issue
            {
                Priority = issueDto.Priority,
                Type = issueDto.Type,
                Description = issueDto.Description,
                ProjectName = issueDto.ProjectName,
                ProjectId = issueDto.ProjectId
            };

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();

            return issue;
        }

        public async Task<bool> DeleteIssueAsync(int issueId)
        {
            var issue = await _context.Issues.FindAsync(issueId);
            if (issue == null) return false;

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}