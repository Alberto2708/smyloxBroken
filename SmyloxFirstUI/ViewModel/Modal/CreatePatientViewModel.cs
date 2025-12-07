using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmyloxFirstUI.Helpers;
using SmyloxFirstUI.Stores;
using SmyloxFirstUI.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.Model.Patient;

namespace SmyloxFirstUI.ViewModel.Modal
{
    public partial class CreatePatientViewModel : ViewModelBase
    {
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly SessionService _sessionService;
        private readonly PatientService _patientService;
        private readonly ViewModelRouter _viewModelRouter;


        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool isLoading;


        public CreatePatientViewModel(
            ModalNavigationStore modalNavigationStore, 
            SessionService sessionService,
            PatientService patientService,
            ViewModelRouter viewModelRouter)
        {
            _modalNavigationStore = modalNavigationStore;
            _sessionService = sessionService;
            _patientService = patientService;
            _viewModelRouter = viewModelRouter;
        }

        [RelayCommand]
        async Task CreatePatient()
        {
            CreatePatient createPatient = new CreatePatient
            {
                firstName = FirstName,
                lastName = LastName,
                doctor = _sessionService.UserId
            };
            var patient = await _patientService.PostCreatePatient(createPatient);

            if(patient == null)
            {
                ErrorMessage = "Error creating patient. Please try again.";
            }

            else
            {

                await ToPatientInfo(patient);
                Close();

            }


        }

        [RelayCommand]
        void Close()
        {
            _modalNavigationStore.Close();
        }



        [RelayCommand]
        async Task ToPatientInfo(PatientDTO patient)
        {
            isLoading = true;

            try
            {
                await _viewModelRouter.AsyncNavigateTo("PatientInfoView", patient);

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

    }
}
