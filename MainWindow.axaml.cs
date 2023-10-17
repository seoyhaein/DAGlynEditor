using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace DAGlynEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        public ObservableCollection<MyRectInfo> MyItems { get; set; } = new ObservableCollection<MyRectInfo>
        {
            new MyRectInfo
            {
                Location = new Point(10, 10),
                W = 100,
                H = 100,
                Br = Brushes.Red
            },
            new MyRectInfo
            {
                Location = new Point(20, 20),
                W = 150,
                H = 150,
                Br = Brushes.Blue
            }
        };
    }

    public class MyRectInfo
    {
        public Point Location { get; set; }
        public double W { get; set; }
        public double H { get; set; }
        public IImmutableSolidColorBrush  Br { get; set; }
    }
}