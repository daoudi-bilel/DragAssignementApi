using System;
using System.Collections.Generic;

namespace DragAssignementApi.Models
{
    public class Objective
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public string Checkmarks { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public ICollection<Member> Assignees { get; set; }
    }
}