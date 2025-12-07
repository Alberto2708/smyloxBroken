using SmyloxFirstUI.Model.Patient;
using System;
using System.Collections.Generic;
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
using SmyloxFirstUI.ViewModel.Doctor;

namespace SmyloxFirstUI.View.Doctor
{
    /// <summary>
    /// Interaction logic for DoctorDashboardView.xaml
    /// </summary>
    public partial class DoctorDashboardView : UserControl
    {
        public DoctorDashboardView()
        {
            InitializeComponent();
        }

        private void PatientCard_Click(object sender, MouseButtonEventArgs e)
        {

            if (sender is Border border && border.Tag is PatientDTO patient)
            {
                if (DataContext is DoctorDashboardViewModel viewModel)
                {
                    viewModel.SelectedPatient = patient;
                }
            }
        }
    }
}
