using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using App;

namespace Client
{
    public partial class FrequentDict : Window
    {
        public FrequentDict(Dictionary<char, double> dataSource)
        {
            InitializeComponent();

            var i = 1;
            dataGridFreqDict.ItemsSource =
                (from pair in dataSource
                 orderby pair.Value descending
                 select new { Номер = i++, Буква = pair.Key, Частота = pair.Value }).ToList();

            i = 1;
            dataGridPrimaryFreqDict.ItemsSource =
                (from pair in Settings.PrimaryDict
                 orderby pair.Value descending
                 select new { Номер = i++, Буква = pair.Key, Частота = pair.Value }).ToList();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

