using Avalonia.Input;
using Avalonia.Interactivity;

namespace DAGlynEditor
{

    // TODO 컨스트럭터 일단 수정 필요.
    // RoutedEvent routedEvent 이걸 직접 넣는 거 말고
    // PointerEventArgs e 여기서 가져오는 것은 없을까?
    public class ContainerEventArgs : RoutedEventArgs
    {
        //private string? _eventName;
        public string? EventName { get; set; }

        public ContainerEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
            EventName = "default";
        }

        public ContainerEventArgs(RoutedEvent routedEvent, PointerEventArgs e, string eventName) : base(routedEvent)
        {
            EventName = eventName;
        }
    }
}
