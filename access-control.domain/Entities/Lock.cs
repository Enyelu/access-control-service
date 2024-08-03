namespace access_control.domain.Entities
{
    public class Lock : BaseEntity
    {
        public string Name { get; set; }
        public bool IsLocked { get; set; }
        public string TenantId { get; set; }
        public string SerialNumber { get; set; }
    }
}
