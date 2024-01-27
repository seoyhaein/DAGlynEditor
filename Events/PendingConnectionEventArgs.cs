using System;
using Avalonia;
using Avalonia.Interactivity;

namespace DAGlynEditor
{
    public delegate void PendingConnectionEventHandler(object? sender, PendingConnectionEventArgs e);

    // 클래스 설명을 추가 필요
    public class PendingConnectionEventArgs : RoutedEventArgs
    {
        // 기본 생성자에 대한 설명을 추가 필요
        public PendingConnectionEventArgs()
        {
        }

        // DataContext를 설정하는 생성자에 대한 설명 추가 필요
        public PendingConnectionEventArgs(object? dataContext)
        {
            SourceConnectorDataContext = dataContext;
        }
        
        public PendingConnectionEventArgs(RoutedEvent routedEvent, object? source, object? dataContext)
            : base(routedEvent, source ?? throw new ArgumentNullException(nameof(source)))
        {
            SourceConnectorDataContext = dataContext;
        }

        // 각 속성에 대한 설명을 추가
        public Point Anchor { get; set; } = new Point(); // 기본값 설정

        public object? SourceConnectorDataContext { get; }

        public object? TargetConnectorDataContext { get; set; }

        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        public bool Canceled { get; set; }

        public object? Sender { get; set; }
        
        public object? CapturedObject { get; set; }
    }
}