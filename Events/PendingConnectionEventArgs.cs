using Avalonia;
using Avalonia.Interactivity;

namespace DAGlynEditor
{
    public delegate void PendingConnectionEventHandler(object? sender, PendingConnectionEventArgs e);

    public class PendingConnectionEventArgs : RoutedEventArgs
    {
        #region Constructors
        public PendingConnectionEventArgs()
        {
        }

        public PendingConnectionEventArgs(object? dataContext)
         => SourceConnectorDataContext = dataContext;

        public PendingConnectionEventArgs(RoutedEvent routedEvent, object? source, object? dataContext)
            : base(routedEvent, source as Interactive)
        {
            SourceConnectorDataContext = dataContext;
        }
        #endregion

        #region fields
        public Point Anchor { get; set; }

        public object? SourceConnectorDataContext { get; }

        public object? TargetConnectorDataContext { get; set; }

        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        public bool Canceled { get; set; }
        #endregion
    }
}
