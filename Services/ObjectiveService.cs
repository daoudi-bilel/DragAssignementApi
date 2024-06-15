using DragAssignementApi.Data;
using DragAssignementApi.Models;
using DragAssignementApi.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragAssignementApi.Services
{
    public class ObjectiveService : IObjectiveService
    {
        private readonly ApplicationDbContext _context;

        public ObjectiveService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Objective> AddObjectiveAsync(ObjectiveDto objectiveDto)
        {
            var objective = new Objective
            {
                Name = objectiveDto.Name,
                Priority = objectiveDto.Priority,
                Status = objectiveDto.Status,
                DueDate = objectiveDto.DueDate,
                Checkmarks = objectiveDto.Checkmarks,
                ProjectId = objectiveDto.ProjectId,
                Assignees = await _context.Members
                                    .Where(m => objectiveDto.AssigneeIds.Contains(m.Id))
                                    .ToListAsync()
            };

            _context.Objectives.Add(objective);
            await _context.SaveChangesAsync();

            return objective;
        }

        public async Task<IEnumerable<Objective>> GetObjectivesByProjectIdAsync(int projectId)
        {
            return await _context.Objectives
                .Where(o => o.ProjectId == projectId)
                .Include(o => o.Assignees)
                .ToListAsync();
        }
    }
}