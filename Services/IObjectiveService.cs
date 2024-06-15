using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragAssignementApi.Services
{
    public interface IObjectiveService
    {
        Task<Objective> AddObjectiveAsync(ObjectiveDto objectiveDto);
        Task<IEnumerable<Objective>> GetObjectivesByProjectIdAsync(int projectId);
    }
}