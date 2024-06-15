namespace DragAssignementApi.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string UniqueId { get; set; }
        public string Industry { get; set; }
        public string NumberOfEmployees { get; set; }
        public string UserId { get; set; } 
        public ApplicationUser User { get; set; } 
    }
}
