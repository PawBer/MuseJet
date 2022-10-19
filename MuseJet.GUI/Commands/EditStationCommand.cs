using MuseJet.Common.Models;
using MuseJet.GUI.ViewModels;
using MuseJet.GUI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MuseJet.GUI.Commands
{
    public class EditStationCommand : ICommand
    {
        public MainWindowViewModel Root { get; set; }

        public EditStationCommand(MainWindowViewModel viewModel)
        {
            Root = viewModel;
        }

        public bool CanExecute(object? parameter) => !Root.CurrentStation.Name.Equals("Select a Station");

        public void Execute(object? parameter)
        {
            if (parameter is not Station) return;
            EditStationView editView = new(Root, (Station)parameter);
            editView.ShowDialog();
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
