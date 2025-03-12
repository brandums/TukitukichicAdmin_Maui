namespace TukiTuki.Models
{
    public class Categoria
    {
        public string Name { get; set; }
        public List<SubCategoria> SubCategories { get; set; } = new();
    }

    public class SubCategoria
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
    }
}
