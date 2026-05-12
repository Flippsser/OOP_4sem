namespace Lab_04_05
{
    public class UserData
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Role { get; set; } = "Client";
        public string Theme { get; set; } = "Light";
        public string Language { get; set; } = "ru";
        public ProfileData Profile { get; set; } = new();
    }
}
