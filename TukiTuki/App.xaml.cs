using TukiTuki.Models;
using TukiTuki.Pages;

namespace TukiTuki
{
    public partial class App : Application
    {
        public static Usuario CurrentUser { get; set; }

        public App()
        {
            var structureService = new StructureService();
            DependencyService.RegisterSingleton<StructureService>(structureService);

            InitializeComponent();

            MainPage = new NavigationPage(new Login());
        }
    }
}
