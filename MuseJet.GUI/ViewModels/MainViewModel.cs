using MuseJet.GUI.Commands;
using MuseJet.GUI.Services;
using MuseJet.GUI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MuseJet.GUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationState _navigationState;
        private readonly NavigationService<PlayerViewModel> _goToPlayer;
        private readonly NavigationService<SearchViewModel> _goToSearch;

        public ViewModelBase CurrentViewModel => _navigationState.CurrentViewModel;

        public MainViewModel(NavigationState navigationState,
                             NavigationService<PlayerViewModel> goToPlayer,
                             NavigationService<SearchViewModel> goToSearch)
        {
            _navigationState = navigationState;
            _goToPlayer = goToPlayer;
            _goToSearch = goToSearch;

            ChangeViewModelCommand = new RelayCommand((obj) =>
            {
                NavDest parameter = (NavDest)obj;
                switch (parameter)
                {
                    case NavDest.Player:
                        goToPlayer.Navigate();
                        break;
                    case NavDest.Search:
                        goToSearch.Navigate();
                        break;
                }
            }, null);

            _navigationState.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public ICommand ChangeViewModelCommand { get; set; }
    }
}
