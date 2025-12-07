using SmyloxFirstUI.Helpers;
using SmyloxFirstUI.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.ViewModel.Assistant
{
    public class AssistantDashboardViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        public AssistantDashboardViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }
    }
}
