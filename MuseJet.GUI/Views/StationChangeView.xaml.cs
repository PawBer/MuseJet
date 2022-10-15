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
    /// Interaction logic for StationChangeView.xaml
    /// </summary>
    public partial class StationChangeView : Window
    {
        public MainWindowViewModel RootViewModel { get; set; } 

        public StationChangeView(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            RootViewModel = viewModel;
        }

        public void button_SubmitStation(object sender, EventArgs e)
        {
            string name = NameTextBox.Text;
            string url = UrlTextBox.Text;
            Station station = new() { Name = name, Url = url };
            RootViewModel.ChangeStation(station);
            RootViewModel.CurrentStation = station;
            Close();
        }
    }
}
