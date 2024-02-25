using System;
using Avalonia;
using Avalonia.Interactivity;

namespace DAGlynEditor
{
    // 클래스 설명을 추가 필요
    public class PendingConnectionEventArgs : RoutedEventArgs
    {
        public PendingConnectionEventArgs(RoutedEvent routedEvent, Connector? sourceConnector)
            : base(routedEvent)
        {
            SourceConnector = sourceConnector;
        }

        public PendingConnectionEventArgs(RoutedEvent routedEvent, Connector? sourceConnector, Point? anchor)
            : base(routedEvent)
        {
            SourceConnector = sourceConnector;
            Anchor = anchor;
        }

        // TODO 여기서 Connector? sourceConnector 는 필요없을 듯한데 일단 남겨둔다.
        public PendingConnectionEventArgs(RoutedEvent routedEvent, Connector? sourceConnector, Vector? offset)
            : base(routedEvent)
        {
            SourceConnector = sourceConnector;
            Offset = offset;
        }

        public Connector? SourceConnector { get; set; }
        // 시작점
        public Point? Anchor { get; set; }
        // 이동 거리
        public Vector? Offset { get; set; }
    }
}