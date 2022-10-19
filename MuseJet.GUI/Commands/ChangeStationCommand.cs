using MuseJet.Common.Models;
using MuseJet.GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MuseJet.GUI.Commands
{
    public class ChangeStationCommand : ICommand
    {
        public MainWindowViewModel Root { get; set; }

        public ChangeStationCommand(MainWindowViewModel viewModel)
        {
            Root = viewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is not Station) return;
            Root.ChangeStation((Station)parameter);
        }
    }
}
