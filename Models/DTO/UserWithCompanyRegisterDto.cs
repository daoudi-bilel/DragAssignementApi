namespace DragAssignementApi.Models.DTO
{
    public class UserWithCompanyRegistrationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyUsername { get; set; }
        public string CompanySize { get; set; }
        public string Industry { get; set; }
    }
}
