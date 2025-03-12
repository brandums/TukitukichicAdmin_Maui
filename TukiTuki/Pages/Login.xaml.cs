using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TukiTuki.Models;

namespace TukiTuki.Pages;

public partial class Login : ContentPage
{
    public Login()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = EmailEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Por favor, ingrese sus credenciales.", "OK");
            return;
        }

        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"https://localhost:7172/api/User/{username}/{password}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var jsonObject = JsonConvert.DeserializeObject<JObject>(content);
            var userJson = jsonObject?["user"]?.ToString();

            if (userJson != null)
            {
                var user = JsonConvert.DeserializeObject<Usuario>(userJson);
                App.CurrentUser = user;

                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la información del usuario.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Credenciales inválidas", "OK");
        }
    }
}