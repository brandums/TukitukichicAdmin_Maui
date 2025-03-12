using System.Collections.ObjectModel;
using TukiTuki.Models;

namespace TukiTuki.Pages;

public partial class PedidosView : ContentView
{
    private readonly StructureService _structureService;
    public ObservableCollection<Pedido> Pedidos { get; set; } = new ObservableCollection<Pedido>();

    public PedidosView()
    {
        InitializeComponent();
        _structureService = DependencyService.Get<StructureService>();
        BindingContext = this;
        LoadPedidos();
    }

    private async void LoadPedidos()
    {
        var pedidos = await _structureService.GetPurchasedProductsAsync();
        Pedidos.Clear();
        foreach (var pedido in pedidos)
        {
            Pedidos.Add(pedido);
        }
    }

    private async void OnUserTapped(object sender, EventArgs e)
    {
        var label = sender as Label;
        var pedido = label.BindingContext as Pedido;

        if (pedido != null)
        {
            if (pedido.Usuario.Email == null)
            {
                var userDetails = await _structureService.GetUserDetailsAsync(pedido.Usuario.Id);
                if (userDetails != null)
                {
                    pedido.Usuario = userDetails;
                }
            }

            // Mostrar los detalles del usuario en el modal
            UserNameLabel.Text = $"Nombre: {pedido.Usuario.Name}";
            UserEmailLabel.Text = $"Email: {pedido.Usuario.Email}";
            UserCountryLabel.Text = $"País: {pedido.Pais}";
            UserAddressLabel.Text = $"Dirección: {pedido.Direccion}";
            UserPhoneLabel.Text = $"Teléfono: {pedido.Usuario.Phone}";

            UserModal.IsVisible = true;
        }
    }

    private void OnCloseModalClicked(object sender, EventArgs e)
    {
        UserModal.IsVisible = false;
    }
}