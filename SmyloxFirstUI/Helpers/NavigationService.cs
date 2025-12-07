using SmyloxFirstUI.Services;
using SmyloxFirstUI.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace SmyloxFirstUI.Helpers
{
    public class NavigationService<TViewModel> : INavigationService where TViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TViewModel> _createViewModel;

        public NavigationService(NavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }
        
        public void Navigate(params object[] parameters)
        {
            var viewModel = _createViewModel();

            if (parameters.Length > 0 && viewModel is IParameterNavigationService paramViewModel)
            {
                paramViewModel.ParameterInitialization(parameters);
            }
            _navigationStore.CurrentViewModel = viewModel;
        }

        public async Task AsyncNavigation(params object[] parameters)
        {
            var viewModel = _createViewModel();
            if (parameters.Length > 0 && viewModel is IParameterNavigationService paramViewModel)
            {
                paramViewModel.ParameterInitialization(parameters);
            }

            if (viewModel is IAsyncNavigationService asyncViewModel)
            {
                await asyncViewModel.InitializeAsync();
            }
            _navigationStore.CurrentViewModel = viewModel;
        }




    }
}
