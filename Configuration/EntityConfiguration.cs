namespace DataVerse_MVC.Configuration
{
    public class EntityConfiguration
    {
        public const string SectionName = "EntityConfiguration";
        public string Resource { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Authority { get; set; }
        public string EntityInternalName { get; set; }
        public string[] Columns { get; set; }
        public string UpdateColumn { get; set; }
    }
}
