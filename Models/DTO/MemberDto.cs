namespace DragAssignementApi.Models.DTO
{
    public class MemberDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public bool IsOwner { get; set; }
        public string AvatarUrl { get; set; }
    }
}
