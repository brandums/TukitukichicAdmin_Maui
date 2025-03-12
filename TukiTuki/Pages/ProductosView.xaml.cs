namespace TukiTuki.Pages;

public partial class ProductosView : ContentView
{
    public ProductosView()
    {
        InitializeComponent();
        ContentGrid.Children.Add(new ProductosList());
    }

    private void MostrarLista(object sender, EventArgs e)
    {
        ContentGrid.Children.Clear();
        ContentGrid.Children.Add(new ProductosList());
    }

    private void MostrarForm(object sender, EventArgs e)
    {
        ContentGrid.Children.Clear();
        ContentGrid.Children.Add(new CrearProducto());
    }
}