using MuseJet.Common.Models;
using MuseJet.GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MuseJet.GUI.Views
{
    /// <summary>
    /// Interaction logic for EditStationView.xaml
    /// </summary>
    public partial class EditStationView : Window
    {
        public MainWindowViewModel Root { get; set; }
        private EditStationViewModel ViewModel { get; set; }

        public EditStationView(MainWindowViewModel viewModel, Station stationToEdit)
        {
            InitializeComponent();
            Root = viewModel;
            ViewModel = new()
            {
                Name = stationToEdit.Name,
                Url = stationToEdit.Url,
            };
            this.DataContext = ViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Station stationToEdit = new() { Name = ViewModel.Name, Url = ViewModel.Url };
            Root.EditStation(stationToEdit);
            Close();
        }
    }
}
