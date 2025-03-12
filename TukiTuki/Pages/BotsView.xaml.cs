using System.Collections.ObjectModel;
using TukiTuki.Models;

namespace TukiTuki.Pages;

public partial class BotsView : ContentView
{
    private readonly StructureService _structureService;
    public ObservableCollection<Usuario> Bots { get; set; } = new ObservableCollection<Usuario>();

    public BotsView()
    {
        InitializeComponent();
        _structureService = DependencyService.Get<StructureService>();
        BindingContext = this;
        LoadBots();
    }

    private async void LoadBots()
    {
        var bots = await _structureService.GetBotsAsync();
        Bots.Clear();
        foreach (var bot in bots)
        {
            Bots.Add(bot);
        }
    }

    private async void OnCreateUserClicked(object sender, EventArgs e)
    {
        var userName = CreateUserNameEntry.Text;
        if (string.IsNullOrWhiteSpace(userName))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Por favor, ingrese un nombre.", "OK");
            return;
        }

        bool success = await _structureService.CreateUserAsync(userName);
        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Bot creado exitosamente.", "OK");
            CreateUserNameEntry.Text = string.Empty;
            LoadBots();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo crear el bot.", "OK");
        }
    }
}