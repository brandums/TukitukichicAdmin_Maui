using System.Collections.ObjectModel;
using TukiTuki.Models;

namespace TukiTuki.Pages;

public partial class CategoriasView : ContentView
{
    private readonly StructureService _structureService;
    public ObservableCollection<Categoria> Categories { get; set; } = new ObservableCollection<Categoria>();
    public ObservableCollection<Categoria> CategoriesBase { get; set; } = new ObservableCollection<Categoria>();
    private bool _isInitializing = true;

    public CategoriasView()
    {
        InitializeComponent();
        _structureService = DependencyService.Get<StructureService>();
        BindingContext = this;
        LoadCategories();
    }

    private async void LoadCategories()
    {
        _isInitializing = true;

        var categories = await _structureService.GetCategoriasAsync();

        CategoriesBase.Clear();
        Categories.Clear();
        foreach (var category in categories)
        {
            CategoriesBase.Add(category);
            if (category.SubCategories != null)
            {
                Categories.Add(category);
            }
        }
        categoriaPicker.ItemsSource = CategoriesBase;

        _isInitializing = false;
    }

    private async void OnUpdateSeasonClicked(object sender, EventArgs e)
    {
        var newName = SeasonNameEntry.Text;
        if (string.IsNullOrWhiteSpace(newName))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Por favor ingrese un nombre.", "OK");
            return;
        }

        bool success = await _structureService.UpdateSeasonNameAsync(((Categoria)categoriaPicker.SelectedItem).Name, newName);
        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Nombre de temporada actualizado correctamente.", "OK");
            SeasonNameEntry.Text = string.Empty;
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar el nombre de Temporada.", "OK");
        }
    }

    private async void OnSubCategoryStatusChanged(object sender, ToggledEventArgs e)
    {
        if (_isInitializing) return;

        var switchControl = sender as Switch;
        var subCategory = switchControl?.BindingContext as SubCategoria;
        var category = Categories.FirstOrDefault(c => c.SubCategories.Contains(subCategory));

        if (category != null && subCategory != null)
        {
            bool success = await _structureService.UpdateSubCategoryStatusAsync(category.Name, subCategory.Name, e.Value);
            if (!success)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to update subcategory status.", "OK");
                switchControl.IsToggled = !e.Value;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "Estado de subcategoria actualizado correctamente.", "OK");
            }
        }
    }
}