namespace TukiTuki.Models
{
    public class Pedido
    {
        public Usuario Usuario { get; set; }
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public double Total { get; set; }
        public string Descuento { get; set; }
        public string Direccion { get; set; }
        public string Pais { get; set; }
        public bool ShowTotal => App.CurrentUser?.Rol == "Admin";
    }
}
