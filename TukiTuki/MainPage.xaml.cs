using TukiTuki.Pages;

namespace TukiTuki
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            VerifyRols();
        }

        private void VerifyRols()
        {
            ProductosTab.IsVisible = false;
            CategoriasTab.IsVisible = false;
            BotsTab.IsVisible = false;
            PedidosTab.IsVisible = false;
            ConfiguracionTab.IsVisible = false;
            CerrarSesionTab.IsVisible = true;

            switch (App.CurrentUser.Rol)
            {
                case "Envios":
                    ContentGrid.Children.Add(new PedidosView());
                    PedidosTab.IsVisible = true;
                    break;
                case "Editor":
                    ContentGrid.Children.Add(new ProductosView());
                    ProductosTab.IsVisible = true;
                    CategoriasTab.IsVisible = true;
                    BotsTab.IsVisible = true;
                    break;
                case "Admin":
                    ContentGrid.Children.Add(new ProductosView());
                    ProductosTab.IsVisible = true;
                    CategoriasTab.IsVisible = true;
                    BotsTab.IsVisible = true;
                    PedidosTab.IsVisible = true;
                    ConfiguracionTab.IsVisible = true;
                    break;
            }
        }

        private void OnMenuItemTapped(object sender, EventArgs e)
        {
            if (sender is Grid grid)
            {
                var label = grid.Children.FirstOrDefault(c => c is Label) as Label;
                if (label != null)
                {
                    string text = label.Text;
                    ContentView contentView = text switch
                    {
                        "Productos" => new ProductosView(),
                        "Categorias" => new CategoriasView(),
                        "Bots" => new BotsView(),
                        "Pedidos" => new PedidosView(),
                        "Configuración" => new UsuariosView(),
                        _ => null
                    };

                    if (contentView != null)
                    {
                        ContentGrid.Children.Clear();
                        ContentGrid.Children.Add(contentView);
                    }
                }
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            App.CurrentUser = null;
            await Navigation.PushAsync(new Login());
        }
    }
}
