using System.Collections.ObjectModel;
using TukiTuki.Models;

namespace TukiTuki.Pages;

public partial class CrearProducto : ContentView
{
    private readonly StructureService _structureService;
    private ImageSource[] selectedImages = new ImageSource[21];
    private ImageSource[] selectedLogos = new ImageSource[21];
    private int selectedImageIndex = -1;
    private bool isModifyingLogo = false;
    private string selectedImgDescripcion = null;
    private ObservableCollection<Categoria> _categories;
    private ObservableCollection<string> _subCategories;
    private List<Entry> colorEntries = new List<Entry>();
    private List<Entry> tallaEntries = new List<Entry>();

    private Producto _productoEditado;

    public CrearProducto(Producto producto = null)
    {
        InitializeComponent();
        _structureService = DependencyService.Get<StructureService>();
        _categories = new ObservableCollection<Categoria>();
        _subCategories = new ObservableCollection<string>();

        categoriaPicker.ItemsSource = _categories;
        subcategoriaPicker.ItemsSource = _subCategories;

        categoriaPicker.SelectedIndexChanged += OnCategorySelected;

        _productoEditado = producto;

        InitializeData();
    }

    public async void InitializeData()
    {
        await LoadCategories();

        if (_productoEditado != null)
        {
            CargarDatosProducto(_productoEditado);
        }
    }

    private void CargarDatosProducto(Producto producto)
    {
        nombreEntry.Text = producto.Nombre;
        precioEntry.Text = producto.Precio.ToString();
        precioDescuentoEntry.Text = producto.PrecioDesc.ToString();
        breveDescripcion.Text = producto.BreveDescripcion;
        colorEntry.Text = producto.Color[0];
        tallaEntry.Text = producto.Talla[0];
        tipoMedida.Text = producto.TipoMedida;
        stockEntry.Text = producto.Stock.ToString();
        categoriaPicker.SelectedItem = _categories.FirstOrDefault(c => c.Name == producto.Categoria);
        subcategoriaPicker.SelectedItem = producto.SubCategoria;

        selectedImgDescripcion = producto.Descripcion;
        descripcionImg.Source = producto.Descripcion;

        foreach (var color in producto.Color.Skip(1))
        {
            OnAddColorClicked(null, null);
            colorEntries.Last().Text = color;
        }

        foreach (var talla in producto.Talla.Skip(1))
        {
            OnAddTallaClicked(null, null);
            tallaEntries.Last().Text = talla;
        }

        if (producto.Imagenes != null && producto.Imagenes.Count > 0)
        {
            for (int i = 0; i < producto.Imagenes.Count; i++)
            {
                if (!string.IsNullOrEmpty(producto.Imagenes[i]))
                {
                    selectedImages[i] = ImageSource.FromUri(new Uri(producto.Imagenes[i]));
                    var frame = (Frame)ImagesContainer.Children[i];
                    var grid = (Grid)frame.Content;
                    var image = (Image)grid.Children[0];
                    image.Source = selectedImages[i];
                }
            }
        }
    }


    private async Task LoadCategories()
    {
        var categories = await _structureService.GetCategoriasAsync();

        foreach (var category in categories)
        {
            _categories.Add(category);
        }

        if (_categories.Count > 0)
        {
            categoriaPicker.SelectedIndex = 0;
        }
    }

    private void OnCategorySelected(object sender, EventArgs e)
    {
        var selectedCategory = (Categoria)categoriaPicker.SelectedItem;

        if (selectedCategory != null)
        {
            _subCategories.Clear();

            if (selectedCategory.SubCategories != null && selectedCategory.SubCategories.Any())
            {
                foreach (var subCategory in selectedCategory.SubCategories)
                {
                    if (subCategory.IsEnabled)
                    {
                        _subCategories.Add(subCategory.Name);
                    }
                }

                subcategoriaPicker.IsVisible = true;
            }
            else
            {
                subcategoriaPicker.IsVisible = false;
            }
        }
    }

    private void OnInfoForm(object sender, EventArgs e)
    {
        InfoGrid.IsVisible = true;
        ImagenesGrid.IsVisible = false;
    }

    private void OnImages(object sender, EventArgs e)
    {
        InfoGrid.IsVisible = false;
        ImagenesGrid.IsVisible = true;
    }

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        try
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona una imagen",
                FileTypes = FilePickerFileType.Images
            });

            if (file != null)
            {
                selectedImgDescripcion = file.FullPath;
                descripcionImg.Background = Colors.Transparent;
                descripcionImg.Source = ImageSource.FromFile(file.FullPath);
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
        }
    }

    private void OnAddColorClicked(object sender, EventArgs e)
    {
        var newEntry = new Entry { Placeholder = "Ejemplo: Rojo, Azul" };
        var deleteButton = new Button { Text = "x", Padding = new Thickness(10, 0) };
        deleteButton.Clicked += (s, args) => OnDeleteColorClicked(newEntry);

        var grid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(), new ColumnDefinition { Width = GridLength.Auto } } };
        grid.Children.Add(newEntry);
        grid.Children.Add(deleteButton);
        Grid.SetColumn(deleteButton, 1);

        colorContainer.Children.Add(grid);
        colorEntries.Add(newEntry);
    }

    private void OnDeleteColorClicked(Entry entry)
    {
        var grid = (Grid)entry.Parent;
        colorContainer.Children.Remove(grid);
        colorEntries.Remove(entry);
    }

    private void OnAddTallaClicked(object sender, EventArgs e)
    {
        var newEntry = new Entry { Placeholder = "Ejemplo: S, M, L" };
        var deleteButton = new Button { Text = "x", Padding = new Thickness(10, 0) };
        deleteButton.Clicked += (s, args) => OnDeleteTallaClicked(newEntry);

        var grid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(), new ColumnDefinition { Width = GridLength.Auto } } };
        grid.Children.Add(newEntry);
        grid.Children.Add(deleteButton);
        Grid.SetColumn(deleteButton, 1);

        tallaContainer.Children.Add(grid);
        tallaEntries.Add(newEntry);
    }

    private void OnDeleteTallaClicked(Entry entry)
    {
        var grid = (Grid)entry.Parent;
        tallaContainer.Children.Remove(grid);
        tallaEntries.Remove(entry);
    }

    //Bloque de imagenes
    private void OnButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        selectedImageIndex = int.Parse(button.StyleId);

        foreach (var child in ImagesContainer.Children)
        {
            if (child is Frame frame)
            {
                frame.IsVisible = false;
            }
        }

        if (selectedImageIndex >= 0 && selectedImageIndex < ImagesContainer.Children.Count)
        {
            var selectedFrame = (Frame)ImagesContainer.Children[selectedImageIndex];
            selectedFrame.IsVisible = true;

            ChangeImageButton.IsVisible = true;
        }
    }

    private async void OnChangeImageClicked(object sender, EventArgs e)
    {
        if (selectedImageIndex != -1)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Pick Image Please",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                var frame = (Frame)ImagesContainer.Children[selectedImageIndex];
                var grid = (Grid)frame.Content;
                var image = (Image)grid.Children[0];
                image.Source = ImageSource.FromStream(() => stream);
            }
        }
    }

    private void OnSliderValueChangedX(object sender, ValueChangedEventArgs e)
    {
        if (selectedImageIndex != -1 && selectedImageIndex < ImagesContainer.Children.Count)
        {
            var frame = (Frame)ImagesContainer.Children[selectedImageIndex];
            var grid = (Grid)frame.Content;
            var target = isModifyingLogo ? (Image)grid.Children[1] : (Image)grid.Children[0];
            target.TranslationX = e.NewValue;
        }
    }

    private void OnSliderValueChangedY(object sender, ValueChangedEventArgs e)
    {
        if (selectedImageIndex != -1 && selectedImageIndex < ImagesContainer.Children.Count)
        {
            var frame = (Frame)ImagesContainer.Children[selectedImageIndex];
            var grid = (Grid)frame.Content;
            var target = isModifyingLogo ? (Image)grid.Children[1] : (Image)grid.Children[0];
            target.TranslationY = e.NewValue;
        }
    }

    private void OnSliderValueChangedScale(object sender, ValueChangedEventArgs e)
    {
        if (selectedImageIndex != -1 && selectedImageIndex < ImagesContainer.Children.Count)
        {
            var frame = (Frame)ImagesContainer.Children[selectedImageIndex];
            var grid = (Grid)frame.Content;
            var target = isModifyingLogo ? (Image)grid.Children[1] : (Image)grid.Children[0];
            target.Scale = e.NewValue;
        }
    }

    private void OnIncrementXClicked(object sender, EventArgs e)
    {
        MoveX.Value = Math.Min(MoveX.Value + 1, MoveX.Maximum);
    }

    private void OnDecrementXClicked(object sender, EventArgs e)
    {
        MoveX.Value = Math.Max(MoveX.Value - 1, MoveX.Minimum);
    }

    private void OnIncrementYClicked(object sender, EventArgs e)
    {
        MoveY.Value = Math.Min(MoveY.Value + 1, MoveY.Maximum);
    }

    private void OnDecrementYClicked(object sender, EventArgs e)
    {
        MoveY.Value = Math.Max(MoveY.Value - 1, MoveY.Minimum);
    }

    private void OnIncrementScaleClicked(object sender, EventArgs e)
    {
        Scale.Value = Math.Min(Scale.Value + 0.01, Scale.Maximum);
    }

    private void OnDecrementScaleClicked(object sender, EventArgs e)
    {
        Scale.Value = Math.Max(Scale.Value - 0.01, Scale.Minimum);
    }

    private async void OnAddGlobalImageClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Selecciona una imagen global",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
        {
            using var stream = await result.OpenReadAsync();
            var imageBytes = await ReadStreamAsync(stream);

            for (int i = 0; i < ImagesContainer.Children.Count; i++)
            {
                var frame = (Frame)ImagesContainer.Children[i];
                var grid = (Grid)frame.Content;
                var image = (Image)grid.Children[0];

                var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                image.Source = imageSource;
            }

            await Application.Current.MainPage.DisplayAlert("Éxito", "La imagen se ha aplicado a todos los contenedores.", "OK");
        }
    }

    private async Task<byte[]> ReadStreamAsync(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    private async void SaveAllGrids_Clicked(object sender, EventArgs e)
    {
        bool allImagesLoaded = true;
        for (int i = 0; i < ImagesContainer.Children.Count; i++)
        {
            var frame = (Frame)ImagesContainer.Children[i];
            var grid = (Grid)frame.Content;
            var image = (Image)grid.Children[0];

            if (image.Source == null)
            {
                allImagesLoaded = false;
                break;
            }
        }

        if (!allImagesLoaded)
        {
            await Application.Current.MainPage.DisplayAlert("Advertencia", "Debes completar todas las imágenes antes de guardar.", "OK");
            return;
        }

        for (int i = 0; i < ImagesContainer.Children.Count; i++)
        {
            var frame = (Frame)ImagesContainer.Children[i];
            frame.IsVisible = true;
        }

        for (int i = 0; i < ImagesContainer.Children.Count; i++)
        {
            var frame = (Frame)ImagesContainer.Children[i];
            var imageStream = await frame.CaptureAsync();

            if (imageStream != null)
            {
                using var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                selectedImages[i] = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
        }

        for (int i = 0; i < ImagesContainer.Children.Count; i++)
        {
            var frame = (Frame)ImagesContainer.Children[i];
            frame.IsVisible = (i == selectedImageIndex);
        }

        //await SaveImagesToFolderAsync();
        await CreateProduct();
    }

    private async Task CreateProduct()
    {
        var substruct = new Substruct();
        substruct.Nombre[0] = nombreEntry.Text;
        substruct.Precio[0] = precioEntry.Text;
        substruct.Extra7[0] = precioDescuentoEntry.Text ?? "";
        substruct.BreveDescripcion[0] = breveDescripcion.Text;
        substruct.Color[0] = GetEntryValues(colorEntry, colorEntries);
        substruct.Talla[0] = GetEntryValues(tallaEntry, tallaEntries);
        substruct.SubCategoria[0] = (subcategoriaPicker.SelectedItem != null) ? subcategoriaPicker.SelectedItem.ToString() : "";
        substruct.Categoria[0] = ((Categoria)categoriaPicker.SelectedItem)?.Name;
        substruct.Images[0] = await UploadAllImagesAsync();
        substruct.Extra1[0] = stockEntry.Text;
        substruct.Extra2[0] = tipoMedida.Text;
        substruct.Descripcion[0] = await SubirImagenSeleccionada(selectedImgDescripcion);

        var response = false;
        if (_productoEditado != null)
        {
            substruct.Codigo[0] = _productoEditado.Codigo;
            response = await _structureService.UpdateProduct(substruct.Codigo[0], substruct);
        }
        else
        {
            response = await _structureService.CreateProduct(substruct);
        }

        if (response)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", $"Producto {(_productoEditado != null ? "actualizado" : "creado")} correctamente", "OK");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al {(_productoEditado != null ? "actualizar" : "crear")} el producto", "OK");
        }
    }

    private string[] GetEntryValues(Entry fixedEntry, List<Entry> dynamicEntries)
    {
        var values = new List<string>();
        if (!string.IsNullOrEmpty(fixedEntry.Text))
        {
            values.Add(fixedEntry.Text);
        }
        values.AddRange(dynamicEntries.Select(e => e.Text).Where(text => !string.IsNullOrEmpty(text)));
        return values.ToArray();
    }

    private async Task<string> SubirImagenSeleccionada(string imagePath)
    {
        if (imagePath.Contains("imagetesteo1.blob.core.windows.net"))
        {
            return imagePath;
        }

        if (string.IsNullOrEmpty(imagePath))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No hay una imagen seleccionada.", "OK");
            return "";
        }

        using (var stream = File.OpenRead(imagePath))
        {
            try
            {
                return await _structureService.UploadImageAsync(stream, Path.GetFileName(imagePath));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al subir la imagen: {ex.Message}", "OK");
                return "";
            }
        }
    }


    private async Task<string[]> UploadAllImagesAsync()
    {
        var imageUrls = new List<string>();

        for (int i = 0; i < selectedImages.Length; i++)
        {
            if (selectedImages[i] != null)
            {
                try
                {
                    if (selectedImages[i] is UriImageSource uriImageSource && uriImageSource.Uri.ToString().Contains("imagetesteo1.blob.core.windows.net"))
                    {
                        imageUrls.Add(uriImageSource.Uri.ToString());
                    }
                    else
                    {
                        var imageStream = await ImageSourceToStreamAsync(selectedImages[i]);

                        string fileName = $"imagen_{i + 1}.jpg";
                        string imageUrl = await _structureService.UploadImageAsync(imageStream, fileName);

                        imageUrls.Add(imageUrl);
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error al subir la imagen {i + 1}: {ex.Message}", "OK");
                }
            }
        }

        return imageUrls.ToArray();
    }

    private async Task<Stream> ImageSourceToStreamAsync(ImageSource imageSource)
    {
        if (imageSource is StreamImageSource streamImageSource)
        {
            var stream = await streamImageSource.Stream(CancellationToken.None);
            return stream;
        }
        throw new ArgumentException("El ImageSource no es compatible.");
    }

    //private async Task SaveImagesToFolderAsync()
    //{
    //    string folderPath = @"C:\Users\brran\OneDrive\Escritorio\prueba";

    //    if (!Directory.Exists(folderPath))
    //    {
    //        Directory.CreateDirectory(folderPath);
    //    }

    //    for (int i = 0; i < selectedImages.Length; i++)
    //    {
    //        if (selectedImages[i] is StreamImageSource streamImageSource)
    //        {
    //            var stream = await streamImageSource.Stream(CancellationToken.None);

    //            string filePath = System.IO.Path.Combine(folderPath, $"imagen_{i + 1}.png");
    //            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
    //            await stream.CopyToAsync(fileStream);
    //        }
    //    }

    //    await Application.Current.MainPage.DisplayAlert("Éxito", "Todas las imágenes se han guardado en la carpeta.", "OK");
    //}
}