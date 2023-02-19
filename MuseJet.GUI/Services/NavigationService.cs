using MuseJet.GUI.Stores;
using MuseJet.GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseJet.GUI.Services
{
    public enum NavDest
    {
        Player,
        Search
    }

    public class NavigationService<T> where T : ViewModelBase
    {
        private readonly NavigationState _navigationStore;
        private readonly Func<T> _createViewModel;

        public NavigationService(NavigationState navigationStore, Func<T> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = _createViewModel();
        }
    }
}
