namespace access_control.domain.Entities
{
    public class Complaint : BaseEntity
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}