using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
                Location = new Point(0, 0),
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
            },
            new MyRectInfo
            {
                Location = new Point(-200, -200),
                W = 150,
                H = 150,
                Br = Brushes.Yellow
            }
        };

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            var tester = this.FindControl<DAGlynEditor>("EditorTester");
            if (tester != null)
            {
                tester.Offset = new Point(100, 200);
            }
        }
    }

    public class MyRectInfo
    {
        public Point Location { get; set; }
        public double W { get; set; }
        public double H { get; set; }
        public IImmutableSolidColorBrush Br { get; set; }
    }
}