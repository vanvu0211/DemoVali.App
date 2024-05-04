using LiveCharts.Events;
using LiveCharts.Wpf;
using LiveCharts;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DemoCaseGui.Core.Application.ViewModels;

namespace DemoCaseGui
{
    /// <summary>
    /// Interaction logic for CaseMicroView.xaml
    /// </summary>
    public partial class CaseMicroView : UserControl
    {

       

        public CaseMicroView()
        {
            InitializeComponent();
            DataContext = new Core.Application.ViewModels.CaseMicroViewModel();
       
        }

        

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TrafficLightsView_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void Chart_Loaded(object sender, RoutedEventArgs e)
        {

        }

     
    }
}
