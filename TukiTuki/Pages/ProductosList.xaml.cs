using System.Collections.ObjectModel;
using TukiTuki.Models;

namespace TukiTuki.Pages;

public partial class ProductosList : ContentView
{
    private readonly StructureService _structureService;
    public ObservableCollection<Usuario> Bots { get; set; } = new ObservableCollection<Usuario>();
    public ObservableCollection<Producto> Productos { get; set; } = new ObservableCollection<Producto>();
    private List<Producto> _todosLosProductos = new List<Producto>();
    private Producto _productoSeleccionado;

    public ProductosList()
    {
        InitializeComponent();
        _structureService = DependencyService.Get<StructureService>();
        IniciarPag();
        BindingContext = this;
    }

    protected async void IniciarPag()
    {
        Productos = new ObservableCollection<Producto>(_structureService.Productos);
        await CargarProductos();
        await LoadBots();
    }

    private async Task LoadBots()
    {
        var bots = await _structureService.GetBotsAsync();
        Bots.Clear();
        foreach (var bot in bots)
        {
            Bots.Add(bot);
        }

        OpinionUserPicker.ItemsSource = Bots;
    }

    private async void OnComment(object sender, EventArgs e)
    {
        commentContent.IsVisible = true;
        settingsContent.IsVisible = false;
    }

    private async void OnSettings(object sender, EventArgs e)
    {
        settingsContent.IsVisible = true;
        commentContent.IsVisible = false;
    }

    private async void OnSaveOpinionClicked(object sender, EventArgs e)
    {
        var selectedBot = OpinionUserPicker.SelectedItem as Usuario;
        if (selectedBot == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Por favor, seleccione un bot.", "OK");
            return;
        }

        if (!int.TryParse(_productoSeleccionado.Codigo, out int productCode) ||
            !int.TryParse(OpinionRatingEntry.Text, out int rating) ||
            string.IsNullOrWhiteSpace(OpinionCommentEntry.Text) ||
            OpinionDatePicker.Date == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Por favor, complete todos los campos correctamente.", "OK");
            return;
        }

        var date = OpinionDatePicker.Date.ToString("yyyy-MM-dd");

        bool success = await _structureService.SaveOpinionAsync(selectedBot.Id, productCode, rating, OpinionCommentEntry.Text, date);
        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Opinión guardada exitosamente.", "OK");
            OpinionRatingEntry.Text = string.Empty;
            OpinionCommentEntry.Text = string.Empty;
            OpinionDatePicker.Date = DateTime.Now;
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la opinión.", "OK");
        }
    }

    private async Task CargarProductos()
    {
        var productos = await _structureService.GetProductosAsync();
        if (productos != null)
        {
            _todosLosProductos = productos;
            Productos.Clear();
            foreach (var producto in productos)
            {
                Productos.Add(producto);
            }
        }
    }

    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var textoBusqueda = e.NewTextValue?.ToLower() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(textoBusqueda))
        {
            Productos.Clear();
            foreach (var producto in _todosLosProductos)
            {
                Productos.Add(producto);
            }
        }
        else
        {
            var productosFiltrados = _todosLosProductos
                .Where(p => p.Nombre.ToLower().Contains(textoBusqueda) ||
                            p.Categoria.ToLower().Contains(textoBusqueda) ||
                            p.SubCategoria.ToLower().Contains(textoBusqueda))
                .ToList();

            Productos.Clear();
            foreach (var producto in productosFiltrados)
            {
                Productos.Add(producto);
            }
        }
    }

    private void OnEditarProductoClicked(object sender, EventArgs e)
    {
        var productoSeleccionado = (sender as Button)?.BindingContext as Producto;
        if (productoSeleccionado != null)
        {
            var editarProductoView = new CrearProducto(productoSeleccionado);
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(editarProductoView);
            ContentGrid.IsVisible = true;
        }
    }

    private async void OnVerDetallesClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        _productoSeleccionado = button?.BindingContext as Producto;

        if (_productoSeleccionado != null)
        {
            PopupImage.Source = _productoSeleccionado.Image;
            PopupNombre.Text = _productoSeleccionado.Nombre;
            PopupPrecio.Text = $"Precio: {_productoSeleccionado.Precio:C}";
            PopupPrecioDesc.Text = $"Precio descuento: {_productoSeleccionado.PrecioDesc:C}";
            PopupCategoria.Text = $"Categoría: {_productoSeleccionado.Categoria}";
            PopupStock.Text = $"Stock: {_productoSeleccionado.Stock}";

            await CargarPaginas();
            CargarPosiciones();

            PopupEstado.IsToggled = _productoSeleccionado.Estado == "1";
            PopupPortada.IsToggled = _productoSeleccionado.Promo == "1";
            PopupDescuento.IsToggled = _productoSeleccionado.Descuento == "1";
            PopupVentas.Text = _productoSeleccionado.VentasBase;
            PopupLikes.Text = _productoSeleccionado.LikesBase;
            PopupOferta.Text = _productoSeleccionado.TiempoOferta;

            PopupDetalles.IsVisible = true;
        }
    }

    private async Task CargarPaginas()
    {
        try
        {
            var totalPaginas = await _structureService.GetPaginasAsync();
            var paginas = Enumerable.Range(1, totalPaginas).Select(p => p.ToString()).ToList();
            PopupPagina.ItemsSource = paginas;

            if (_productoSeleccionado != null && !string.IsNullOrEmpty(_productoSeleccionado.Pagina))
            {
                PopupPagina.SelectedItem = _productoSeleccionado.Pagina;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar las páginas: {ex.Message}");
        }
    }

    private void CargarPosiciones()
    {
        try
        {
            if (_productoSeleccionado == null || string.IsNullOrEmpty(_productoSeleccionado.Pagina))
            {
                Console.WriteLine("No se ha seleccionado un producto o no tiene página asignada.");
                return;
            }

            int currentPage = int.Parse(_productoSeleccionado.Pagina);
            int productsPerPage = 10;
            int totalProducts = _todosLosProductos.Count;

            int maxPositions = totalProducts - ((currentPage - 1) * productsPerPage);
            maxPositions = Math.Min(maxPositions, productsPerPage);

            var posiciones = Enumerable.Range(1, maxPositions).Select(p => p.ToString()).ToList();

            PopupPosicion.ItemsSource = posiciones;

            if (!string.IsNullOrEmpty(_productoSeleccionado.Posicion))
            {
                PopupPosicion.SelectedItem = _productoSeleccionado.Posicion;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar las posiciones: {ex.Message}");
        }
    }

    private void OnCerrarPopupClicked(object sender, EventArgs e)
    {
        PopupDetalles.IsVisible = false;
    }

    private void OnPaginaChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        var nuevaPagina = picker.SelectedItem?.ToString();
        var imagenGuardar = PopupPagina.FindByName<Image>("ImagenGuardarPagina");

        if (imagenGuardar != null)
        {
            imagenGuardar.IsVisible = nuevaPagina != _productoSeleccionado.Pagina;
        }
    }

    private void OnPosicionChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        var nuevaPosicion = picker.SelectedItem?.ToString();
        var imagenGuardar = PopupPosicion.FindByName<Image>("ImagenGuardarPosicion");

        if (imagenGuardar != null)
        {
            imagenGuardar.IsVisible = nuevaPosicion != _productoSeleccionado.Posicion;
        }
    }

    private void OnEstadoChanged(object sender, ToggledEventArgs e)
    {
        var nuevoEstado = e.Value ? "1" : "0";
        var imagenGuardar = PopupEstado.FindByName<Image>("ImagenGuardarEstado");

        if (imagenGuardar != null)
        {
            imagenGuardar.IsVisible = nuevoEstado != _productoSeleccionado.Estado;
        }
    }

    private void OnPortadaChanged(object sender, ToggledEventArgs e)
    {
        var nuevaPortada = e.Value ? "1" : "0";
        var imagenGuardar = PopupPortada.FindByName<Image>("ImagenGuardarPortada");

        if (imagenGuardar != null)
        {
            imagenGuardar.IsVisible = nuevaPortada != _productoSeleccionado.Promo;
        }
    }

    private void OnDescuentoChanged(object sender, ToggledEventArgs e)
    {
        var nuevoDescuento = e.Value ? "1" : "0";
        var imagenGuardar = PopupDescuento.FindByName<Image>("ImagenGuardarDescuento");

        if (imagenGuardar != null)
        {
            imagenGuardar.IsVisible = nuevoDescuento != _productoSeleccionado.Descuento;
        }
    }

    private void OnVentasChanged(object sender, TextChangedEventArgs e)
    {
        var nuevasVentas = e.NewTextValue;
        var imagenGuardar = PopupVentas.FindByName<Image>("ImagenGuardarVentas");

        if (imagenGuardar != null)
        {
            imagenGuardar.IsVisible = nuevasVentas != _productoSeleccionado.VentasBase;
        }
    }

    private void OnLikesChanged(object sender, TextChangedEventArgs e)
    {
        var nuevosLikes = e.NewTextValue;
        var imagenGuardar = PopupLikes.FindByName<Image>("ImagenGuardarLikes");

        if (imagenGuardar != null)
        {
            imagenGuardar.IsVisible = nuevosLikes != _productoSeleccionado.LikesBase;
        }
    }

    private void OnOfertaChanged(object sender, TextChangedEventArgs e)
    {
        var nuevaOferta = e.NewTextValue;
        var imagenGuardar = PopupOferta.FindByName<Image>("ImagenGuardarOferta");

        if (imagenGuardar != null)
        {
            imagenGuardar.IsVisible = nuevaOferta != _productoSeleccionado.TiempoOferta;
        }
    }

    private async void GuardarPagina(object sender, EventArgs e)
    {
        if (_productoSeleccionado == null || PopupPagina.SelectedItem == null)
            return;

        int nuevaPagina;
        if (!int.TryParse(PopupPagina.SelectedItem.ToString(), out nuevaPagina))
            return;

        bool actualizado = await _structureService.UpdateProductPageAsync(_productoSeleccionado.Codigo, nuevaPagina);

        if (actualizado)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Página actualizada correctamente", "OK");
            ImagenGuardarPagina.IsVisible = false;
            _todosLosProductos = await _structureService.GetProductosAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar la página", "OK");
        }
    }

    private async void GuardarPosicion(object sender, EventArgs e)
    {
        if (PopupPosicion.SelectedItem == null || _productoSeleccionado == null)
            return;

        string nuevaPosicion = PopupPosicion.SelectedItem.ToString();

        bool success = await _structureService.UpdateProductPositionAsync(_productoSeleccionado.Codigo, int.Parse(nuevaPosicion));

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Posición actualizada correctamente.", "OK");
            ImagenGuardarPosicion.IsVisible = false;
            _todosLosProductos = await _structureService.GetProductosAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar la posición.", "OK");
        }
    }

    private async void GuardarEstado(object sender, EventArgs e)
    {
        if (_productoSeleccionado == null || PopupEstado.IsToggled == null)
            return;

        var nuevoEstado = PopupEstado.IsToggled ? "1" : "0";

        var index = _todosLosProductos.FindIndex(p => p.Codigo == _productoSeleccionado.Codigo);

        bool success = await _structureService.UpdateProductStateAsync(index, PopupEstado.IsToggled);

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Estado actualizado correctamente.", "OK");
            ImagenGuardarEstado.IsVisible = false;
            _todosLosProductos = await _structureService.GetProductosAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar el estado.", "OK");
            PopupEstado.IsToggled = !_productoSeleccionado.Estado.Equals("1");
        }
    }


    private async void GuardarPortada(object sender, EventArgs e)
    {
        if (_productoSeleccionado == null || PopupPortada.IsToggled == null)
            return;

        var nuevoPortada = PopupPortada.IsToggled ? "1" : "0";

        var index = _todosLosProductos.FindIndex(p => p.Codigo == _productoSeleccionado.Codigo);

        bool success = await _structureService.UpdateProductPromoAsync(index, nuevoPortada);

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Portada actualizada correctamente.", "OK");
            ImagenGuardarPortada.IsVisible = false;
            _todosLosProductos = await _structureService.GetProductosAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar la portada.", "OK");
            PopupPortada.IsToggled = !_productoSeleccionado.Promo.Equals("1");
        }
    }

    private async void GuardarDescuento(object sender, EventArgs e)
    {
        if (_productoSeleccionado == null || PopupDescuento.IsToggled == null)
            return;

        var descuento = PopupDescuento.IsToggled ? "1" : "0";

        var index = _todosLosProductos.FindIndex(p => p.Codigo == _productoSeleccionado.Codigo);

        bool success = await _structureService.UpdateProductDiscountAsync(index, descuento);

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Descuento actualizado correctamente.", "OK");
            ImagenGuardarDescuento.IsVisible = false;
            _todosLosProductos = await _structureService.GetProductosAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar el descuento.", "OK");
            PopupDescuento.IsToggled = !_productoSeleccionado.Descuento.Equals("1");
        }
    }

    private async void GuardarVentas(object sender, EventArgs e)
    {
        var nuevaBase = PopupVentas.Text;
        bool success = await _structureService.UpdateVentasBaseAsync(int.Parse(_productoSeleccionado.Codigo), int.Parse(nuevaBase));

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Ventas base se actualizado correctamente.", "OK");
            ImagenGuardarVentas.IsVisible = false;
            _todosLosProductos = await _structureService.GetProductosAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar la base de ventas.", "OK");
        }
    }

    private async void GuardarLikes(object sender, EventArgs e)
    {
        var nuevaBase = PopupLikes.Text;
        bool success = await _structureService.UpdateLikesBaseAsync(int.Parse(_productoSeleccionado.Codigo), int.Parse(nuevaBase));

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Likes base se actualizado correctamente.", "OK");
            ImagenGuardarLikes.IsVisible = false;
            _todosLosProductos = await _structureService.GetProductosAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar la base de Likes.", "OK");
        }
    }

    private async void GuardarOferta(object sender, EventArgs e)
    {
        var time = PopupOferta.Text;
        bool success = await _structureService.UpdateTimeOfertaAsync(int.Parse(_productoSeleccionado.Codigo), int.Parse(time));

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Tiempo de Oferta actualizado correctamente.", "OK");
            ImagenGuardarOferta.IsVisible = false;
            _todosLosProductos = await _structureService.GetProductosAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar el tiempo de Oferta.", "OK");
        }
    }
}