using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DragAssignementApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public Company Company { get; set; } 
         public ICollection<Project> Projects { get; set; }
         public ICollection<Member> Members { get; set; }
    }
}
