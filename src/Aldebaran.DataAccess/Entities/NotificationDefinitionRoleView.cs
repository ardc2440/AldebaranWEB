namespace Aldebaran.DataAccess.Entities
{
    public class NotificationDefinitionRoleView
    {
        public int NotificationDefinitionRoleId { get; set; }
        public int NotificationDefinitionId { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
