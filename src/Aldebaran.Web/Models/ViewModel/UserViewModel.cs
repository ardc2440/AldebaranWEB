namespace Aldebaran.Web.Models.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool LockoutEnabled { get; set; }
        public string UserName { get; set; }
        public string IdentificationNumber { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Area { get; set; }
    }
}
