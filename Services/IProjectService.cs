using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragAssignementApi.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetProjectsByUserIdAsync(string userId);
        Task<Project> AddProjectAsync(string userId, ProjectDto projectDto);
        Task<bool> DeleteProjectAsync(string userId, int projectId);
    }
}
