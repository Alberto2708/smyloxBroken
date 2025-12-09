using CommunityToolkit.Mvvm.ComponentModel;
using SmyloxFirstUI.Model.Modelling;
using SmyloxFirstUI.Services;
using SmyloxFirstUI.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SmyloxFirstUI.Helpers.ModellingHelpers;
using System.Windows;
using g3;
using CommunityToolkit.Mvvm.Input;

namespace SmyloxFirstUI.ViewModel.Modelling
{
    public partial class CloseBaseViewModel : ViewModelBase, IParameterNavigationService
    {

        [ObservableProperty]
        private string _statusMessage = "Loaded models";
        [ObservableProperty]
        private JawMeshModel _upperJawMesh;

        [ObservableProperty]
        private JawMeshModel _lowerJawMesh;

        [ObservableProperty]
        private bool _isLoading;

        public CloseBaseViewModel()
        {
            
        }

        public void ParameterInitialization(params object[] parameters)
        {
            if (parameters != null && parameters.Length == 2)
            {
                if (parameters[0].GetType() == typeof(string))
                {
                    LoadUpper((string)parameters[0]);
                }

                if(parameters[1].GetType() == typeof(string))
                {
                    LoadLower((string)parameters[1]);
                }

            }

        }

        public void LoadUpper(string filepath)
        {
            try
            {
                var rawMesh = MeshIO.LoadStl(filepath);

                //upper -> gum extruded upwards
                var closed = GumCloser.CloseWithGum(
                    rawMesh,
                    isUpper: true,
                    gumHeight: 8.0,
                    baseOffset: 1.0,
                    radialInset: 0.4,
                    smoothIterations: 50,
                    edgeSmoothIterations: 50);

                var meshGeo = MeshConverters.ToMeshGeometry3D(closed);
                var model = MeshConverters.CreateGeometryModel(meshGeo, Colors.DeepSkyBlue);

                //UpperModelVisual.Content = model;

                UpperJawMesh = new JawMeshModel
                {
                    FilePath = filepath,
                    FileName = System.IO.Path.GetFileName(filepath),
                    Mesh = closed,
                    GeometryModel = model,
                    JawType = JawType.Upper
                };
            }

            catch (Exception ex)
            {
                // Handle exceptions (e.g., show a message to the user)
                MessageBox.Show(
                    $"Error loading upper STL:\n{ex.Message}",
                    "Load error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void LoadLower(string filepath)
        {
            try
            {
                var rawMesh = MeshIO.LoadStl(filepath);
                //lower -> gum extruded downwards
                var closed = GumCloser.CloseWithGum(
                    rawMesh,
                    isUpper: false,
                    gumHeight: 8.0,
                    baseOffset: 1.0,
                    radialInset: 0.4,
                    smoothIterations: 6,
                    edgeSmoothIterations: 12);

                MeshConverters.Translate(closed, new Vector3d(0, -5, 0));

                var meshGeo = MeshConverters.ToMeshGeometry3D(closed);
                var model = MeshConverters.CreateGeometryModel(meshGeo, Colors.OrangeRed);

                LowerJawMesh = new JawMeshModel
                {
                    FilePath = filepath,
                    FileName = System.IO.Path.GetFileName(filepath),
                    Mesh = closed,
                    GeometryModel = model,
                    JawType = JawType.Lower
                };

            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., show a message to the user)
                MessageBox.Show(
                    $"Error loading lower STL:\n{ex.Message}",
                    "Load error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ResetCammera()
        {
            StatusMessage = "Camera Reset";
        }



    }
}
