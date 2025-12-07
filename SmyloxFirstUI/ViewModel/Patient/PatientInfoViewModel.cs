using SmyloxFirstUI.Helpers;
using SmyloxFirstUI.Services;
using SmyloxFirstUI.Stores;
using SmyloxFirstUI.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using SmyloxFirstUI.Model.MedicalCase;
using SmyloxFirstUI.Model.Patient;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace SmyloxFirstUI.ViewModel.Patient
{
    public partial class PatientInfoViewModel : ViewModelBase, IAsyncNavigationService, IParameterNavigationService
    {
        private readonly PatientStore _patientStore;
        private readonly MedicalCaseStore _medicalCaseStore;
        private readonly NavigationStore _navigationStore;
        private readonly ViewModelRouter _viewModelRouter;

        private ICollectionView _medicalCasesView;

        public ICollectionView MedicalCasesView
        {
            get => _medicalCasesView;
            set
            {
                Debug.WriteLine($"[PatientInfoViewModel] MedicalCasesView set. Current view is null? {value == null}");
                SetProperty(ref _medicalCasesView, value);
            }
        }

        public bool HasCases => MedicalCasesView?.Cast<object>().Any() ?? false;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private ObservableCollection<MedicalCaseDTO> _medicalCases;

        [ObservableProperty]
        private PatientDTO _patientInfo;

        public PatientInfoViewModel(NavigationStore navigationStore,
            ViewModelRouter viewModelRouter,
            PatientStore patientStore,
            MedicalCaseStore medicalCaseStore)
        {
            Debug.WriteLine("[PatientInfoViewModel] Constructor called.");

            _medicalCases = new ObservableCollection<MedicalCaseDTO>();
            _patientInfo = new PatientDTO();
            _navigationStore = navigationStore;
            _viewModelRouter = viewModelRouter;
            _patientStore = patientStore;
            _medicalCaseStore = medicalCaseStore;

            Debug.WriteLine($"[PatientInfoViewModel] PatientStore.SelectedPatient at ctor: {_patientStore.SelectedPatient?.patientId}");
            Debug.WriteLine($"[PatientInfoViewModel] MedicalCaseStore initial count: {_medicalCaseStore.MedicalCaseList?.Count()}");

            MedicalCasesView = CollectionViewSource.GetDefaultView(MedicalCases);
            init();

        }

        void init()
        {
            Debug.WriteLine("[PatientInfoViewModel] init() called. Clearing MedicalCases collection.");

            MedicalCases.Clear();

            if (_medicalCaseStore.MedicalCaseList != null && _medicalCaseStore.MedicalCaseList.Count() > 0)
            {
                Debug.WriteLine($"[PatientInfoViewModel] Found {_medicalCaseStore.MedicalCaseList.Count()} cases in MedicalCaseStore. Adding to collection.");
                foreach (var medicalCase in _medicalCaseStore.MedicalCaseList)
                {
                    MedicalCases.Add(medicalCase);
                    Debug.WriteLine($"[PatientInfoViewModel] Added medical case id: {medicalCase.id}");
                }
            }
            else
            {
                Debug.WriteLine("[PatientInfoViewModel] No cases in MedicalCaseStore to add in init().");
            }

            Debug.WriteLine($"[PatientInfoViewModel] MedicalCases collection count after init: {MedicalCases.Count}");
        }

        public void ParameterInitialization(params object[] parameters)
        {
            Debug.WriteLine($"[PatientInfoViewModel] ParameterInitialization called with {parameters?.Length ?? 0} parameters.");
            if (parameters != null && parameters.Length > 0)
            {
                if (parameters[0].GetType() == typeof(PatientDTO))
                {
                    PatientInfo = (PatientDTO)parameters[0];
                    _patientStore.SelectedPatient = PatientInfo;
                    Debug.WriteLine($"[PatientInfoViewModel] ParameterInitialization set PatientInfo id: {PatientInfo?.patientId}");
                }
                else
                {
                    Debug.WriteLine($"[PatientInfoViewModel] ParameterInitialization: unexpected parameter type {parameters[0].GetType()}");
                }
            }
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("[PatientInfoViewModel] InitializeAsync called.");
            await LoadMedicalCases();
        }

        private async Task LoadMedicalCases()
        {
            Debug.WriteLine("[PatientInfoViewModel] LoadMedicalCases called.");
            try
            {
                MedicalCases.Clear();
                Debug.WriteLine($"[PatientInfoViewModel] Cleared MedicalCases collection. Count now: {MedicalCases.Count}");

                Debug.WriteLine($"[PatientInfoViewModel] PatientStore.SelectedPatient before Load: {_patientStore.SelectedPatient?.patientId}");

                await _medicalCaseStore.Load();

                Debug.WriteLine($"[PatientInfoViewModel] MedicalCaseStore returned {_medicalCaseStore.MedicalCaseList?.Count()} cases.");

                foreach (var medicalCase in _medicalCaseStore.MedicalCaseList)
                {
                    MedicalCases.Add(medicalCase);
                    Debug.WriteLine($"[PatientInfoViewModel] Added medical case id: {medicalCase.id} in LoadMedicalCases");
                }

                Debug.WriteLine($"[PatientInfoViewModel] MedicalCases collection count after LoadMedicalCases: {MedicalCases.Count}");

                OnPropertyChanged(nameof(HasCases));
                Debug.WriteLine($"[PatientInfoViewModel] HasCases property changed. HasCases: {HasCases}");
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error loading medical cases: {ex.Message}");
                Debug.WriteLine($"[PatientInfoViewModel] LoadMedicalCases exception: {ex}");
            }
        }

        [RelayCommand]
        private async Task CreateMedicalCase()
        {
            IsLoading = true;
            try
            {
                Debug.WriteLine("[PatientInfoViewModel] CreateMedicalCase command invoked.");
                await _viewModelRouter.AsyncNavigateTo("CreateMedicalCaseView", PatientInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to CreateMedicalCaseView: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task BackToDashboard()
        {
            IsLoading = true;
            try
            {
                Debug.WriteLine("[PatientInfoViewModel] BackToDashboard command invoked.");
                await _viewModelRouter.AsyncNavigateTo("DoctorDashboardView");
                _medicalCaseStore.ClearMedicalCases();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to DoctorDashboardView: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

    }

}
