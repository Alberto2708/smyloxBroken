using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmyloxFirstUI.Helpers;
using SmyloxFirstUI.Model.Patient;
using SmyloxFirstUI.Model.MedicalCase;
using SmyloxFirstUI.Services;
using SmyloxFirstUI.Stores;
using SmyloxFirstUI.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using SmyloxFirstUI.Model.Doctor;
using System.Diagnostics;

namespace SmyloxFirstUI.ViewModel.Modal
{
    public partial class CreateMedicalCaseViewModel : ViewModelBase, IParameterNavigationService, IAsyncNavigationService
    {
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly SessionService _sessionService;
        private readonly MedicalCaseService _medicalCaseService;
        private readonly ViewModelRouter _viewModelRouter;
        private readonly DoctorStore _doctorStore;

        [ObservableProperty]
        private string _category;

        [ObservableProperty]
        private string _diagnostic;

        [ObservableProperty]
        private PatientDTO _patientInfo;

        [ObservableProperty]
        private DoctorDTO _doctorInfo;

        [ObservableProperty]
        private string _maxilarFilePath;

        [ObservableProperty]
        private string _mandibularFilePath;

        // Add these properties for UI feedback
        [ObservableProperty]
        private bool _hasMaxilarFile;

        [ObservableProperty]
        private bool _hasMandibularFile;

        public CreateMedicalCaseViewModel(
            ModalNavigationStore modalNavigationStore,
            SessionService sessionService,
            MedicalCaseService medicalCaseService,
            ViewModelRouter viewModelRouter,
            DoctorStore doctorStore)
        {
            _modalNavigationStore = modalNavigationStore;
            _sessionService = sessionService;
            _medicalCaseService = medicalCaseService;
            _viewModelRouter = viewModelRouter;
            _doctorStore = doctorStore;
            init();
        }

        void init()
        {
            if (_doctorStore.Doctor != null)
            {
                DoctorInfo = _doctorStore.Doctor;
            }
        }

        public async Task InitializeAsync()
        {

            await LoadDoctorInfo();
        }

        private async Task LoadDoctorInfo()
        {
            try
            {
                await _doctorStore.Load();
                DoctorInfo = _doctorStore.Doctor;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log error, show message to user)
                System.Diagnostics.Debug.WriteLine($"Error loading doctor info: {ex.Message}");
            }
        }

        public void ParameterInitialization(object[] parameters)
        {

            if (parameters != null && parameters.Length > 0)
            {
                if (parameters[0] is PatientDTO patientDTO)
                {
                    PatientInfo = patientDTO;
                }
            }
        }

        [RelayCommand]
        void SelectMaxilarFile()
        {
            var filepath = OpenStlFileDialog();
            if (!string.IsNullOrEmpty(filepath))
            {
                MaxilarFilePath = filepath;
                HasMaxilarFile = true;
                Debug.WriteLine($"Maxilar file selected: {filepath}");
                Debug.WriteLine($"HasMaxilarFile: {HasMaxilarFile}");
            }
        }

        [RelayCommand]
        void SelectMandibularFile()
        {
            var filepath = OpenStlFileDialog();
            if (!string.IsNullOrEmpty(filepath))
            {
                MandibularFilePath = filepath;
                HasMandibularFile = true;
                Debug.WriteLine($"Mandibular file selected: {filepath}");
                Debug.WriteLine($"HasMandibularFile: {HasMandibularFile}");
            }
        }

        private string OpenStlFileDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "STL Files (*.stl)|*.stl",
                Title = "Select an STL File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        [RelayCommand]
        async Task CreateMedicalCase()
        {
            CreateMedicalCase createMedicalCase = new CreateMedicalCase
            {
                patientId = PatientInfo.patientId,
                doctorId = _sessionService.UserId,
                assistantId = null,
                category = Category,
                diagnostic = Diagnostic
            };

            Debug.WriteLine($"Using {PatientInfo.patientId}");
            Debug.WriteLine($"Using {_sessionService.UserId}");
            Debug.WriteLine($"Using {Category}");
            Debug.WriteLine($"Using {Diagnostic}");

            var medicalCase = await _medicalCaseService.PostCreateMedicalCase(createMedicalCase);

            if (medicalCase == null)
            {
                // Show error message
                return;
            }

            try
            {
                var (maxilarPath, mandibularPath) = await OrganizeSTlFiles();
                Debug.WriteLine("STL files organized successfully.");
                GoToCloseBase(maxilarPath, mandibularPath);
                Close();
            }

            catch (Exception ex)
            {
                // Handle exceptions (e.g., show error message)

                System.Diagnostics.Debug.WriteLine($"Error organizing STL files: {ex.Message}");
            }

        }

        void GoToCloseBase(string maxilarPath, string mandibularPath)
        {
            _viewModelRouter.NavigateTo("CloseBaseView", maxilarPath, mandibularPath);
        }

        private async Task<(string? MaxilarPath, string? MandibularPath)> OrganizeSTlFiles()
        {
            string baseDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmyloxFolders");

            string doctorFolder = _doctorStore.Doctor != null
                ? $"{_doctorStore.Doctor.firstName}_{_doctorStore.Doctor.lastName}"
                : "UnknownDoctor";

            string patientFolder = $"{PatientInfo.firstName}_{PatientInfo.lastName}";

            string categoryFolder = Category.Replace(" ", "_");

            string targetDirectory = Path.Combine(baseDirectory, "cases", doctorFolder, patientFolder, categoryFolder);

            Directory.CreateDirectory(targetDirectory);

            string? maxilarPath = null;
            string? mandibularPath = null;


            if (!string.IsNullOrEmpty(MaxilarFilePath) && File.Exists(MaxilarFilePath))
            {
                string maxilarFileName = $"maxilar_initial_file.stl";
                string maxilarTargetPath = Path.Combine(targetDirectory, maxilarFileName);
                maxilarPath = maxilarTargetPath;
                await Task.Run(() => File.Copy(MaxilarFilePath, maxilarTargetPath, overwrite: true));
            }

            if (!string.IsNullOrEmpty(MandibularFilePath) && File.Exists(MandibularFilePath))
            {
                string mandibularFileName = $"mandibular_initial_file.stl";
                string mandibularTargetPath = Path.Combine(targetDirectory, mandibularFileName);
                mandibularPath = mandibularTargetPath;
                await Task.Run(() => File.Copy(MandibularFilePath, mandibularTargetPath, overwrite: true));
            }

            Debug.WriteLine($"STL files copied to: {targetDirectory}");

            return (maxilarPath, mandibularPath);
        }



        [RelayCommand]
        void Close()
        {
            _modalNavigationStore.Close();
        }




    }
}