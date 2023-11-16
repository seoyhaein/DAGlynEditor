using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Diagnostics;
using static DAGlynEditor.EditorGestures;

namespace DAGlynEditor
{
    public delegate void ContainerEventArgsEventHandler(object? sender, ContainerEventArgs e);

    public interface ICanvasItem
    {
        // 이것만 구현하면됨. 아래 두개는 상속받는 클래스에서 이미 구현되어 있음.
        Point Location { get; }

        Size DesiredSize { get; }

        void Arrange(Rect rect);
    }

    // 일단 테스트 용도로 간단하게 작성한다.
    // ListBoxItem 참고한다.
    public class ItemContainer : ContentControl, ICanvasItem
    {

        #region Dependency Properties

        public static readonly StyledProperty<Point> LocationProperty =
            AvaloniaProperty.Register<ItemContainer, Point>(nameof(Location));

        public Point Location
        {
            get => GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }
        #endregion

        #region Constructor

        static ItemContainer()
        {
            //FocusableProperty.OverrideDefaultValue<ItemContainer>(true);
        }

        public ItemContainer() 
        {
            // 향후 Rx 로 바꿀 예정.
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerMoved += OnPointerMoved;

            // 여기서 Rx 로 바꿀 예정.
            ContainerDragStarted += OnContainerDragStarted;
            ContainerDragging += OnContainerDragging;
            ContainerDragCompleted += OnContainerDragCompleted;
        }
        #endregion

        #region event handler

        //TODO 이벤트가 처리 되지 않는 것은 다른데서 처리 된것 같다. 이거 파악하자.
        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            RaiseContainerDragStartedEvent(e);
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            RaiseContainerDraggingEvent(e);
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            RaiseContainerDragCompletedEvent(e);
        }

        private void OnContainerDragStarted(object? sender, ContainerEventArgs e)
        {
            Debug.WriteLine(e.EventName);
        }

        private void OnContainerDragging(object? sender, ContainerEventArgs e)
        {
            Debug.WriteLine(e.EventName);
        }

        private void OnContainerDragCompleted(object? sender, ContainerEventArgs e)
        {
            Debug.WriteLine(e.EventName);
        }

        #endregion

        #region 이벤트 만들어주기
        private void RaiseContainerDragStartedEvent(PointerPressedEventArgs e)
        {
            var args = new ContainerEventArgs(ContainerDragStartedEvent, e, "dragstarted");
            if (args == null)
            {
                Debug.WriteLine("args is null");
                return;
            }
                
            RaiseEvent(args);
        }

        private void RaiseContainerDraggingEvent(PointerEventArgs e)
        {
            var args = new ContainerEventArgs(ContainerDraggingEvent, e, "dragging");
            if (args == null)
            {
                Debug.WriteLine("args is null");
                return;
            }
            RaiseEvent(args);
        }

        private void RaiseContainerDragCompletedEvent(PointerReleasedEventArgs e)
        {
            var args = new ContainerEventArgs(ContainerDragCompletedEvent, e, "dragcompleted");
            if (args == null)
            {
                Debug.WriteLine("args is null");
                return;
            }
            RaiseEvent(args);
        }
        #endregion

        #region Event 등록 및 핸들러 등록
        public static readonly RoutedEvent<ContainerEventArgs> ContainerDragStartedEvent =
            RoutedEvent.Register<ItemContainer, ContainerEventArgs>(nameof(ContainerDragStarted), RoutingStrategies.Bubble);

        public static readonly RoutedEvent<ContainerEventArgs> ContainerDragCompletedEvent =
            RoutedEvent.Register<ItemContainer, ContainerEventArgs>(nameof(ContainerDragCompleted), RoutingStrategies.Bubble);

        public static readonly RoutedEvent<ContainerEventArgs> ContainerDraggingEvent =
            RoutedEvent.Register<ItemContainer, ContainerEventArgs>(nameof(ContainerDragging), RoutingStrategies.Bubble);

        public event ContainerEventArgsEventHandler ContainerDragStarted
        {
            add => AddHandler(ContainerDragStartedEvent, value);
            remove => RemoveHandler(ContainerDragStartedEvent, value);
        }

        public event ContainerEventArgsEventHandler ContainerDragCompleted
        {
            add => AddHandler(ContainerDragCompletedEvent, value);
            remove => RemoveHandler(ContainerDragCompletedEvent, value);
        }

        public event ContainerEventArgsEventHandler ContainerDragging
        {
            add => AddHandler(ContainerDraggingEvent, value);
            remove => RemoveHandler(ContainerDraggingEvent, value);
        }
        #endregion
    }
}
