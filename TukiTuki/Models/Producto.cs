namespace TukiTuki.Models
{
    public class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public double PrecioDesc { get; set; }
        public string BreveDescripcion { get; set; }
        public string Descripcion { get; set; }
        public List<string> Color { get; set; }
        public List<string> Talla { get; set; }
        public string TipoMedida { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }
        public List<string> Imagenes { get; set; } = new();
        public int Cantidad { get; set; }
        public string Pagina { get; set; }
        public string Posicion { get; set; }
        public string Estado { get; set; }
        public string Promo { get; set; }
        public string Descuento { get; set; }
        public string VentasBase { get; set; }
        public string LikesBase { get; set; }
        public string TiempoOferta { get; set; }
    }
}
