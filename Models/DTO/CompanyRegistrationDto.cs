namespace DragAssignementApi.Models.DTO{

public class CompanyRegistrationDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public string UniqueId { get; set; }
    public string Industry { get; set; }
    public int NumberOfEmployees { get; set; }
     public string UserId { get; set; } 
    public ApplicationUser User { get; set; } 
}

}
