// Models/Member.cs
using System.Collections.Generic;

namespace DragAssignementApi.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public bool IsOwner { get; set; }
        public string AvatarUrl { get; set; }
        public string UserId { get; set; } 
        public ApplicationUser User { get; set; }
        public ICollection<Objective> Objectives { get; set; }
    }
}