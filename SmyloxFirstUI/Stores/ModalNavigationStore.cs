using SmyloxFirstUI.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Stores
{
    public class ModalNavigationStore
    {
        private ViewModelBase _currentModalViewModel;

        public ViewModelBase CurrentModalViewModel
        {
            get => _currentModalViewModel;
            set
            {
                _currentModalViewModel = value;
                OnCurrentModalViewModelChanged();
            }
        }

        public bool IsOpen => _currentModalViewModel != null;

        public event Action CurrentModalViewModelChanged;

        public void Close()
        {
            CurrentModalViewModel = null;
        }

        private void OnCurrentModalViewModelChanged()
        {
            CurrentModalViewModelChanged?.Invoke();
        }
    }

        

    }
