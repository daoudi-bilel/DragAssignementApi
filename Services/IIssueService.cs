using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragAssignementApi.Services
{
    public interface IIssueService
    {
        Task<IEnumerable<Issue>> GetIssuesByProjectIdAsync(int projectId);
        Task<Issue> AddIssueAsync(Issue issueDto);
        Task<bool> DeleteIssueAsync(int issueId);
    }
}