namespace access_control.domain.Entities
{
    public class EventLog : BaseEntity
    {
        public string TenantId { get; set; }
        public string Action { get; set; }
        public string Changes { get; set; }
        public bool IsSuccessful { get; set; }
    }
}