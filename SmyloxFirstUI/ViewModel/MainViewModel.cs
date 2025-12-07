using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.ViewModel.Base;
using SmyloxFirstUI.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using SmyloxFirstUI.Stores;

namespace SmyloxFirstUI.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;

        [ObservableProperty]
        private ViewModelBase _currentViewModel;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsOpen))]
        private ViewModelBase _currentModalViewModel;

        public bool IsOpen => _modalNavigationStore.IsOpen;

        public MainViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;

            CurrentViewModel = _navigationStore.CurrentViewModel;
            CurrentModalViewModel = _modalNavigationStore.CurrentModalViewModel;


            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _modalNavigationStore.CurrentModalViewModelChanged += OnCurrentModalViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModel = _navigationStore.CurrentViewModel;
        }

        private void OnCurrentModalViewModelChanged()
        {
            CurrentModalViewModel = _modalNavigationStore.CurrentModalViewModel;
        }

    }
}
