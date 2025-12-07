using SmyloxFirstUI.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.Stores;

namespace SmyloxFirstUI.Services
{
    public class ModalNavigationService<TViewModel> : INavigationService where TViewModel : ViewModelBase
    {
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly Func<TViewModel> _createViewModel;

        public ModalNavigationService(ModalNavigationStore modalNavigationStore, Func<TViewModel> createViewModel)
        {
            _modalNavigationStore = modalNavigationStore;
            _createViewModel = createViewModel;
        }

        public async Task AsyncNavigation(params object[] parameters)
        {
            var viewModel = _createViewModel();

            if (parameters.Length > 0 && parameters != null)
            {
                if (viewModel is IParameterNavigationService paramViewModel)
                {
                    paramViewModel.ParameterInitialization(parameters);
                }
            }

            if (viewModel is IAsyncNavigationService asyncViewModel)
            {
                await asyncViewModel.InitializeAsync();
            }

            _modalNavigationStore.CurrentModalViewModel = viewModel;
        }

        public void Navigate(params object[] parameters)
        {
            var viewModel = _createViewModel();
            if (parameters.Length > 0 && parameters != null)
            {
                if (viewModel is IParameterNavigationService paramViewModel)
                {
                    paramViewModel.ParameterInitialization(parameters);
                }
            }
            _modalNavigationStore.CurrentModalViewModel = viewModel;
        }

        public void Close()
        {
            _modalNavigationStore?.Close();

        }


    }
}
