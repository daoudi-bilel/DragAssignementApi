namespace DragAssignementApi.Models.DTO
{
    public class IssueDto
    {
        public string Priority { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ProjectName { get; set; }
        public int ProjectId { get; set; }
    }
}