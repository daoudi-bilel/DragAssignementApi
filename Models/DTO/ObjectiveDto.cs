// Models/DTO/ObjectiveDto.cs
namespace DragAssignementApi.Models.DTO
{
    public class ObjectiveDto
    {
        public string Name { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public string Checkmarks { get; set; }
        public int ProjectId { get; set; }
        public List<int> AssigneeIds { get; set; }
    }
}
