using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmyloxFirstUI.Helpers;
using SmyloxFirstUI.HostBuilder;
using SmyloxFirstUI.Stores;
using SmyloxFirstUI.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SmyloxFirstUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly IHost _host;
        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .AddViewModels()
                .ConfigureServices(services =>
                {

                    services.AddSingleton<SessionService>();
                    services.AddSingleton<PatientStore>();
                    services.AddSingleton<DoctorStore>();
                    services.AddSingleton<MedicalCaseStore>();
                    services.AddHttpClient<ApiClient>(client =>
                {
                    client.BaseAddress = new Uri("http://localhost:8081/");
                    
                });
                services.AddSingleton<AuthService>();

                    services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>((s) => new MainWindow()
                    {
                    DataContext = s.GetRequiredService<MainViewModel>()
                    });

                }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            var router = _host.Services.GetRequiredService<ViewModelRouter>();
            await router.AsyncNavigateTo("LoginView");

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }

}
