namespace TestEv.Infrastructure.Configuration
{
    public class AdminUserSettings
    {
        public const string SectionName = "AdminUser";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
