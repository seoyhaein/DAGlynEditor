using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;

// IDisposable 때문에 넣었지만 이건 테스트 해서 살펴봐야 할듯하다. 
// 일단 주석처리 해두었다.
using System;
using System.Diagnostics;

namespace DAGlynEditor
{
    public delegate void PreviewLocationChanged(Point newLocation);
    public class Connector : TemplatedControl
    {
        #region Routed Events
        public static readonly RoutedEvent<PendingConnectionEventArgs> PendingConnectionStartedEvent =
            RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(PendingConnectionStarted), RoutingStrategies.Bubble);

        public static readonly RoutedEvent<PendingConnectionEventArgs> PendingConnectionCompletedEvent =
            RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(PendingConnectionCompleted), RoutingStrategies.Bubble);

        public static readonly RoutedEvent<PendingConnectionEventArgs> PendingConnectionDragEvent =
            RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(PendingConnectionDrag), RoutingStrategies.Bubble);

        public event PendingConnectionEventHandler PendingConnectionStarted
        {
            add => AddHandler(PendingConnectionStartedEvent, value);
            remove => RemoveHandler(PendingConnectionStartedEvent, value);
        }

        public event PendingConnectionEventHandler PendingConnectionCompleted
        {
            add => AddHandler(PendingConnectionCompletedEvent, value);
            remove => RemoveHandler(PendingConnectionCompletedEvent, value);
        }

        public event PendingConnectionEventHandler PendingConnectionDrag
        {
            add => AddHandler(PendingConnectionDragEvent, value);
            remove => RemoveHandler(PendingConnectionDragEvent, value);
        }

        #endregion

        #region Constructor
        static Connector()
        {
            FocusableProperty.OverrideDefaultValue<Connector>(true);
        }

        #endregion

        #region Fields & Dependency Properties

        // Interactive 일단 바뀔 수 있음.
        protected Interactive? Thumb { get; private set; }
        private Point? _thumbCenter;
        public ItemContainer? Container { get; set; }

        public static readonly StyledProperty<Point> AnchorProperty =
            AvaloniaProperty.Register<Connector, Point>(nameof(Anchor));

        public Point Anchor
        {
            get => GetValue(AnchorProperty);
            set => SetValue(AnchorProperty, value);
        }

        public static readonly StyledProperty<object> HeaderProperty =
            AvaloniaProperty.Register<InputConnector, object>(nameof(Header));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly StyledProperty<DataTemplate> HeaderTemplateProperty =
            AvaloniaProperty.Register<InputConnector, DataTemplate>(nameof(HeaderTemplate));

        public DataTemplate HeaderTemplate
        {
            get => GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }
        #endregion

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            Thumb = e.NameScope.Find<Ellipse>("PART_CONNECTOR");
            Container = this.FindAncestorOfType<ItemContainer>();

            // TODO 추후 로그로
            if (Thumb == null)
            {
                throw new InvalidOperationException("Template is missing the required 'PART_CONNECTOR' element.");
            }
        }

        // 일단 참고용으로 넣어 둔다. 향후 삭제 예정
        /*protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (Container is string containerName)
            {
                var containerControl = this.FindAncestorOfType<Control>(containerName);
                if (containerControl != null)
                {
                    Container = containerControl;
                }
                else
                {
                    throw new InvalidOperationException($"No control found with the name {containerName}.");
                }
            }
        }*/

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                return;
            }

            Focus();
            e.Pointer.Capture(this);

            if (this.Equals(e.Pointer.Captured))
            {
                // 일단 여기서 Anchor 업데이트. 땜방코드
                if (Container == null) return;
                var check = UpdateAnchor();

                // TODO 추후 로그로
                if (!check) throw new InvalidOperationException("Failed to update the Anchor.");
                PendingConnectionStartedRaiseEvent();
                Debug.Print("OnPointerPressed");
                e.Handled = true;
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (this.Equals(e.Pointer.Captured))
            {
                if (Thumb != null)
                {
                    _thumbCenter = new Point(Thumb.Bounds.Width / 2, Thumb.Bounds.Height / 2);
                }

                PendingConnectionCompletedRaiseEvent();
                // capture 해제.
                e.Pointer.Capture(null);
                e.Handled = true;
            }
            Debug.Print("OnPointerReleased");
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (_thumbCenter.HasValue)
            {
                Vector? offset = e.GetPosition(Thumb) - _thumbCenter.Value;
                PendingConnectionDragRaiseEvent(offset);
            }
            e.Handled = true;
        }

        // DataContext 는 살펴보자.
        protected virtual void PendingConnectionStartedRaiseEvent()
        {
            var args = new PendingConnectionEventArgs(PendingConnectionStartedEvent, this, DataContext)
            {
                Anchor = Anchor,
            };

            RaiseEvent(args);
        }

        // 빈공란으로 향후 남겨두자.
        protected virtual void PendingConnectionDragRaiseEvent(Vector? offset)
        {
            if (offset == null) return;

            var args = new PendingConnectionEventArgs(PendingConnectionDragEvent, this, DataContext)
            {
                OffsetX = offset.Value.X,
                OffsetY = offset.Value.Y,
            };

            RaiseEvent(args);
        }

        protected virtual void PendingConnectionCompletedRaiseEvent()
        {
            // PendingConnectionEventArgs(DataContext) 관련해서 살펴봐야 함.
            var args = new PendingConnectionEventArgs(PendingConnectionCompletedEvent, this, DataContext)
            {
                //Anchor = Anchor,
            };
            RaiseEvent(args);
        }

        private bool UpdateAnchor()
        {
            if (Container == null || Thumb == null) return false;
            // 인라인
            Vector? ToVector(Size? size) => size.HasValue ? new Vector(size.Value.Width, size.Value.Height) : (Vector?)null;

            var thumbVector = ToVector(Thumb.Bounds.Size);
            var containerRenderedVector = ToVector(Container.Bounds.Size);
            var containerDesiredVector = ToVector(Container.DesiredSize);

            if (!thumbVector.HasValue || !containerRenderedVector.HasValue || !containerDesiredVector.HasValue) return false;

            var marginDifference = containerRenderedVector.Value - containerDesiredVector.Value;
            var adjustedPoint = (Point)thumbVector.Value / 2 - marginDifference / 2;

            var relativeLocation = Thumb.TranslatePoint(adjustedPoint, Container);
            if (!relativeLocation.HasValue) return false;

            Anchor = new Point(Container.Location.X + relativeLocation.Value.X, Container.Location.Y + relativeLocation.Value.Y);
            return true;
        }

        // 테스트 용도로 만들어 짐.
        public T? FindParent<T>(Visual control) where T : Visual
        {
            var parentOfType = this.FindAncestorOfType<T>();
            return parentOfType;
        }

        private Vector? ConvertFrom(Size? size)
        {
            return size.HasValue ? new Vector(size.Value.Width, size.Value.Height) : (Vector?)null;
        }

    }
}
