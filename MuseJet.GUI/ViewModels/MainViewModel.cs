using MuseJet.GUI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseJet.GUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationState _navigationState;

        public ViewModelBase CurrentViewModel => _navigationState.CurrentViewModel;

        public MainViewModel(NavigationState navigationState)
        {
            _navigationState = navigationState;

            _navigationState.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
