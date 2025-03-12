using TukiTuki.Models;

namespace TukiTuki.Pages;

public partial class UsuariosView : ContentView
{
    private readonly StructureService _structureService;
    private Usuario _selectedUser = new();

    public UsuariosView()
    {
        InitializeComponent();
        _structureService = DependencyService.Get<StructureService>();
        LoadUsers();
    }

    private async void LoadUsers()
    {
        var users = await _structureService.GetUsersAsync();
        UsersCollectionView.ItemsSource = users;
    }

    private void OnUserSelected(object sender, EventArgs e)
    {
        var button = sender as Button;

        _selectedUser = button?.CommandParameter as Usuario;

        if (_selectedUser != null)
        {
            RoleGrid.IsVisible = true;
            RolePicker.SelectedItem = _selectedUser.Rol ?? "User";
        }
    }

    private async void OnSaveRoleClicked(object sender, EventArgs e)
    {
        if (_selectedUser.Id != null && !string.IsNullOrEmpty(RolePicker.SelectedItem?.ToString()))
        {
            var result = await _structureService.UpdateUserRoleAsync(_selectedUser, RolePicker.SelectedItem.ToString());

            if (result)
            {
                RoleGrid.IsVisible = false;
                LoadUsers();
            }
        }
    }

    private void CerrarPopup(object sender, EventArgs e)
    {
        RoleGrid.IsVisible = false;
        _selectedUser = null;
    }
}