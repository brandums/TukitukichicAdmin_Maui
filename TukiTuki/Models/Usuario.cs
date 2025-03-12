namespace TukiTuki.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = "";
        public string Name { get; set; }
        public string CI { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Rol { get; set; }
    }
}
