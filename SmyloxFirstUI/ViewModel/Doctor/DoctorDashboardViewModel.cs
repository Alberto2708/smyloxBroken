using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.Helpers;
using SmyloxFirstUI.ViewModel.Base;
using SmyloxFirstUI.Services;
using SmyloxFirstUI.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using SmyloxFirstUI.Model.Patient;
using CommunityToolkit.Mvvm.Input;
using SmyloxFirstUI.Model.Doctor;
using System.ComponentModel;
using System.Windows.Data;
using System.Diagnostics;

namespace SmyloxFirstUI.ViewModel.Doctor
{
    public partial class DoctorDashboardViewModel : ViewModelBase, IAsyncNavigationService
    {

        private readonly PatientStore _patientStore;
        private readonly ViewModelRouter _viewModelRouter;
        private readonly DoctorStore _doctorStore;
        
        private ICollectionView _patientsView;
        public ICollectionView PatientsView
        {
            get => _patientsView;
            set => SetProperty(ref _patientsView, value);
        }

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private ObservableCollection<PatientDTO> _patients;

        [ObservableProperty]
        private DoctorDTO _doctorinfo;

        [ObservableProperty]
        private PatientDTO? _selectedPatient;

        public DoctorDashboardViewModel(PatientStore patientStore,DoctorStore doctorStore, ViewModelRouter viewModelRouter)
        {

            _patients = new ObservableCollection<PatientDTO>();
            _doctorinfo = new DoctorDTO();
            _patientStore = patientStore;
            _doctorStore = doctorStore;
            _viewModelRouter = viewModelRouter;

            PatientsView = CollectionViewSource.GetDefaultView(Patients);
            PatientsView.Filter = PatientFilter;
            init();

        }

        void init()
        {
            if (_patientStore.PatientList != null && _patientStore.PatientList.Count() > 0)
            {
                foreach (var patient in _patientStore.PatientList)
                {
                    Patients.Add(patient);
                }
            }

            if(_doctorStore.Doctor != null)
            {
                Doctorinfo = _doctorStore.Doctor;
            }


        }

        private bool PatientFilter(object item)
        {
            if (item is not PatientDTO p) return false;

            if (string.IsNullOrEmpty(SearchText)) return true;

            return p.firstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                p.lastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

        }

        partial void OnSearchTextChanged(string value)
        {
            PatientsView.Refresh();
        }


        public async Task InitializeAsync()
        {
            await LoadPatients();
            await LoadDoctorInfo();
        }

        private async Task LoadPatients()
        {
            try
            {

                Patients.Clear();

                await _patientStore.Load();
                foreach (var patient in _patientStore.PatientList)
                {
                    Patients.Add(patient);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private async Task LoadDoctorInfo()
        {
            try
            {
                await _doctorStore.Load();
                Doctorinfo = _doctorStore.Doctor;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);


            }
        }

        [RelayCommand]
        void NavigateTo(string view)
        {
            _viewModelRouter.NavigateTo(view);

        }

        [RelayCommand]
        async Task GoToPatientInfo(PatientDTO patient)
        {
            Debug.WriteLine($"GoToPatientInfo called with patient: {(patient == null ? "null" : $"{patient.firstName} {patient.lastName}")}");
            if (patient == null) return;
            _isLoading = true;

            Debug.WriteLine("GoToPatientInfo: _isLoading set to true");
            try
            {
                Debug.WriteLine("GoToPatientInfo: Navigating to PatientInfoView");

                _patientStore.SelectedPatient = patient;
                await _viewModelRouter.AsyncNavigateTo("PatientInfoView", patient);
                Debug.WriteLine("GoToPatientInfo: Navigation completed successfully");
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"GoToPatientInfo: Exception during navigation: {ex}");
                throw;
            }
            finally
            {
                _isLoading = false;
                Debug.WriteLine("GoToPatientInfo: _isLoading set to false (finally)");
            }
        }

        [RelayCommand]
        void CreatePatient()
        {
            Debug.WriteLine("Navigating to CreatePatientView");
            _viewModelRouter.NavigateTo("CreatePatientView");
        }

        partial void OnSelectedPatientChanged(PatientDTO? value)
        {
            Debug.WriteLine($"OnSelectedPatientChanged invoked. New value: {(value == null ? "null" : $"{value.firstName} {value.lastName}")}");
            if (value != null)
            {
                Debug.WriteLine("OnSelectedPatientChanged: calling GoToPatientInfo");
                _ = GoToPatientInfo(value);

            }
        }

    }

}
