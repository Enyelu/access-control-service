namespace access_control.domain.Entities
{
    public class Permission : BaseEntity
    {
        public string RoleId { get; set; }
        public string LockId { get; set; }
    }
}
