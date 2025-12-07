using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmyloxFirstUI.Helpers;
using SmyloxFirstUI.Services;
using SmyloxFirstUI.ViewModel.Assistant;
using SmyloxFirstUI.ViewModel.Doctor;
using SmyloxFirstUI.ViewModel.Auth;
using Microsoft.Extensions.Http;
using System.Net.Http;
using SmyloxFirstUI.ViewModel.Modal;
using SmyloxFirstUI.Stores;
using SmyloxFirstUI.ViewModel.Patient;
namespace SmyloxFirstUI.HostBuilder
{
    public static class AddViewsHostBuilderExtention
    {
        public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                // Register ViewModels here
                // services.AddTransient<MainViewModel>();

                services.AddSingleton<NavigationStore>();
                services.AddSingleton<PatientService>();
                services.AddSingleton<ModalNavigationStore>();
                services.AddSingleton<DoctorService>();
                services.AddSingleton<MedicalCaseService>();


                services.AddTransient<LoginViewModel>();
                services.AddTransient<DoctorDashboardViewModel>();
                services.AddTransient<AssistantDashboardViewModel>();
                services.AddTransient<PatientInfoViewModel>();

                services.AddTransient<CreatePatientViewModel>();
                services.AddTransient<CreateMedicalCaseViewModel>();

                services.AddSingleton<ViewModelRouter>((s) =>
                new ViewModelRouter(new Dictionary<string, INavigationService>
                {
                    {
                        "LoginView",
                        new NavigationService<LoginViewModel>(
                            s.GetRequiredService<NavigationStore>(),
                            () => s.GetRequiredService<LoginViewModel>())
                    },
                    {
                        "DoctorDashboardView",
                        new NavigationService<DoctorDashboardViewModel>(
                            s.GetRequiredService<NavigationStore>(),
                            () => s.GetRequiredService<DoctorDashboardViewModel>())

                    },
                    {
                        "AssistantDashboardView",
                        new NavigationService<AssistantDashboardViewModel>(
                            s.GetRequiredService<NavigationStore>(),
                            () => s.GetRequiredService<AssistantDashboardViewModel>())
                    },
                    {
                        "CreatePatientView",
                        new ModalNavigationService<CreatePatientViewModel>(
                            s.GetRequiredService<ModalNavigationStore>(),
                            () => s.GetRequiredService<CreatePatientViewModel>())

                    },
                    {
                        "PatientInfoView",
                        new NavigationService<PatientInfoViewModel>(
                            s.GetRequiredService<NavigationStore>(),
                            () => s.GetRequiredService<PatientInfoViewModel>())
                    },
                    {
                        "CreateMedicalCaseView",
                        new ModalNavigationService<CreateMedicalCaseViewModel>(
                            s.GetRequiredService<ModalNavigationStore>(),
                            () => s.GetRequiredService<CreateMedicalCaseViewModel>())
                    }

                }));
            });
            return hostBuilder;
        }
    }
}
