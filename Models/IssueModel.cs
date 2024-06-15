namespace DragAssignementApi.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public string Priority { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ProjectName { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}