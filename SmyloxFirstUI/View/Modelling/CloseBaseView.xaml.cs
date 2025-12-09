using SmyloxFirstUI.ViewModel.Modelling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmyloxFirstUI.View.Modelling
{
    /// <summary>
    /// Interaction logic for CloseBaseView.xaml
    /// </summary>
    public partial class CloseBaseView : UserControl
    {

        public CloseBaseView()
        {
            InitializeComponent();
        }

        private void BtnResetCamera_Click(object sender, RoutedEventArgs e)
        {
            View3D.ResetCamera();
            View3D.ZoomExtents();
            TxtStatus.Text = "Camera reset.";
        }
    }
}