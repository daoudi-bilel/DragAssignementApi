namespace DragAssignementApi.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Shortcut { get; set; }
        public string Color { get; set; }
        public string UserId { get; set; } 
        public ApplicationUser User { get; set; }
        public ICollection<Issue> Issues { get; set; }
        public ICollection<Objective> Objectives { get; set; }
    }
}
