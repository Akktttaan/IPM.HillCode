using System.Collections.Generic;
using System.Linq;
using System.Windows;
using App;
using LiveCharts;
using LiveCharts.Wpf;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Graphics.xaml
    /// </summary>
    public partial class Graphics : Window
    {
        public SeriesCollection SeriesCollection { get; set; }
        public Graphics(double[] frequencies)
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Зашифрованный текст",
                    Values = new ChartValues<double>(frequencies)
                }
            };

            DataContext = this;
        }
    }
}
