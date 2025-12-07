using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmyloxFirstUI.Helpers;
using SmyloxFirstUI.ViewModel.Assistant;
using SmyloxFirstUI.ViewModel.Base;
using SmyloxFirstUI.ViewModel.Doctor;
using SmyloxFirstUI.Model.Auth;
using System;
using System.Diagnostics;
using System.Text.Json;
using SmyloxFirstUI.Services;
using System.Xml.Serialization;



namespace SmyloxFirstUI.ViewModel.Auth
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly ViewModelRouter _viewModelRouter;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool isLoading;

        // Bound from the View using a behavior, NOT a PasswordBox reference
        [ObservableProperty]
        private string password;

        public LoginViewModel(AuthService authService, ViewModelRouter viewModelRouter)
        {
            _authService = authService;
            _viewModelRouter = viewModelRouter;
        }

        [RelayCommand]
        private async Task Login()
        {

            isLoading = true;
            Debug.WriteLine("[LoginViewModel.Login] isLoading = true");

            try
            {
                Debug.WriteLine("[LoginViewModel.Login] Calling AuthService.Login");
                var loginResult = await _authService.Login(username, password);
                Debug.WriteLine($"[LoginViewModel.Login] AuthService returned: {(loginResult == null ? "<null>" : JsonSerializer.Serialize(loginResult))}");

                if (loginResult == null)
                {
                    errorMessage = "Invalid username or password.";
                    Debug.WriteLine("[LoginViewModel.Login] Login failed — set ErrorMessage");
                    return;
                }

                Debug.WriteLine($"[LoginViewModel.Login] role = '{loginResult.role}'");

                switch (loginResult.role)
                {
                    case "doctor":
                        
                        await _viewModelRouter.AsyncNavigateTo("DoctorDashboardView");
                        
                        break;

                    case "assistant":
                       
                        await _viewModelRouter.AsyncNavigateTo("AssistantDashboardView");
                        
                        break;

                    default:
                        
                        break;
                }
            }
            catch (Exception ex)
            {
                
                errorMessage = "An error occurred during login.";
            }
            finally
            {
                isLoading = false;
            }
        }

        [RelayCommand]
        void Navigation(string viewName)
        {
            
            _viewModelRouter.NavigateTo(viewName);
            
        }

        [RelayCommand]
        async Task AsyncNavigateTo(string viewName)
        {
            
            isLoading = true;
            
            try
            {
                await _viewModelRouter.AsyncNavigateTo(viewName);
                
            }
            catch (Exception ex)
            {
                
                throw;
            }
            finally
            {
                isLoading = false;

            }
        }

        [RelayCommand]
        void CreateAccount()
        {
            var url = "https://www.smylox.com/";

            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        [RelayCommand]
        void ForgotPassword()
        {
            var url = "https://www.smylox.com/";
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}