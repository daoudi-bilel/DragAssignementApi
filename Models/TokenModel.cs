namespace DragAssignementApi.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive => DateTime.UtcNow <= ExpiryDate;

        public ApplicationUser User { get; set; }
    }
}
