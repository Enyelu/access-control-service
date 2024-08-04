namespace access_control.core.DataTransferObjects
{
    public class DeletePermissionDto
    {
        public Guid Id { get; set; }
        public Guid LockId { get; set; }
        public Guid TenantId { get; set; }
    }
}
