using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace DAGlynEditor
{
    public delegate void PreviewLocationChanged(Point newLocation);

    public class Connector : TemplatedControl
    {
        protected bool _isPointerPressed;
        
        #region Routed Events

        protected static readonly RoutedEvent<PendingConnectionEventArgs> PendingConnectionStartedEvent =
            RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(PendingConnectionStarted),
                RoutingStrategies.Bubble);

        public static readonly RoutedEvent<PendingConnectionEventArgs> PendingConnectionCompletedEvent =
            RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(PendingConnectionCompleted),
                RoutingStrategies.Bubble);

        public static readonly RoutedEvent<PendingConnectionEventArgs> PendingConnectionDragEvent =
            RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(PendingConnectionDrag),
                RoutingStrategies.Bubble);

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

        public Connector()
        {
            InitializeSubscriptions();
        }

        static Connector()
        {
            FocusableProperty.OverrideDefaultValue<Connector>(true);
        }

        #endregion

        #region Fields & Dependency Properties
        
        protected Control? Thumb { get; set; }
        protected Point? _thumbCenter;
        
        //Test
        protected Canvas? myCanvas { get; set; }
        // 구현 수정
        //public ItemContainer? Container { get; set; }

        public static readonly StyledProperty<Point> AnchorProperty =
            AvaloniaProperty.Register<Connector, Point>(nameof(Anchor));

        public Point Anchor
        {
            get => GetValue(AnchorProperty);
            set => SetValue(AnchorProperty, value);
        }

        public static readonly StyledProperty<object> HeaderProperty =
            AvaloniaProperty.Register<InConnector, object>(nameof(Header));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly StyledProperty<DataTemplate> HeaderTemplateProperty =
            AvaloniaProperty.Register<InConnector, DataTemplate>(nameof(HeaderTemplate));

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
            if (Thumb == null)
                throw new InvalidOperationException("Template is missing the required 'PART_CONNECTOR' element.");

            myCanvas = this.FindControl<Canvas>("MyCanvas");

        }

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private void InitializeSubscriptions()
        {
            Observable.FromEventPattern<PointerPressedEventArgs>(
                    h => this.PointerPressed += h,
                    h => this.PointerPressed -= h)
                .Subscribe(args => HandlePointerPressed(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PointerEventArgs>(
                    h => this.PointerMoved += h,
                    h => this.PointerMoved -= h)
                .Subscribe(args => HandlePointerMoved(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PointerReleasedEventArgs>(
                    h => this.PointerReleased += h,
                    h => this.PointerReleased -= h)
                .Subscribe(args => HandlePointerReleased(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);
        }

        private void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
            if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                return;
            }

            if (sender == null) return;

            Focus();
            args.Pointer.Capture(this);

            if (this.Equals(args.Pointer.Captured))
            {
                // 일단 여기서 Anchor 업데이트. 땜방코드
                //if (Container == null) return;
                //var check = UpdateAnchor();

                // TODO 추후 로그로
                //if (!check) throw new InvalidOperationException("Failed to update the Anchor.");
                StartedRaiseEvent(args);
                Debug.Print("OnPointerPressed");
                args.Handled = true;
            }
        }

        private void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
            if (sender == null) return;
            if (_thumbCenter.HasValue)
            {
                Vector? offset = args.GetPosition(Thumb) - _thumbCenter.Value;
                DragRaiseEvent(args, offset);
            }

            args.Handled = true;
        }

        private void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
            if (sender == null) return;
            if (this.Equals(args.Pointer.Captured))
            {
                if (Thumb != null)
                {
                    _thumbCenter = new Point(Thumb.Bounds.Width / 2, Thumb.Bounds.Height / 2);
                }

                CompletedRaiseEvent(args);
                // capture 해제.
                args.Pointer.Capture(null);
                args.Handled = true;
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        // DataContext 는 살펴보자.
        private void StartedRaiseEvent(PointerPressedEventArgs e)
        {
            var args = new PendingConnectionEventArgs(PendingConnectionStartedEvent, this, DataContext)
            {
                Anchor = Anchor,
                Sender = e.Source,
                CapturedObject = e.Pointer.Captured,
            };
            
            
            RaiseEvent(args);
        }

        // 빈공란으로 향후 남겨두자.
        private void DragRaiseEvent(PointerEventArgs e, Vector? offset)
        {
            if (offset == null) return;

            var args = new PendingConnectionEventArgs(PendingConnectionDragEvent, this, DataContext)
            {
                OffsetX = offset.Value.X,
                OffsetY = offset.Value.Y,
                Sender = e.Source,
                CapturedObject = e.Pointer.Captured,
            };

            RaiseEvent(args);
        }

        private void CompletedRaiseEvent(PointerReleasedEventArgs e)
        {
            // PendingConnectionEventArgs(DataContext) 관련해서 살펴봐야 함.
            var args = new PendingConnectionEventArgs(PendingConnectionCompletedEvent, this, DataContext)
            {
                Sender = e.Source,
                CapturedObject = e.Pointer.Captured,
                //Anchor = Anchor,
            };
            RaiseEvent(args);
        }
        
        // 일단 주석 처리함.
        /*
        private bool UpdateAnchor()
        {
            if (Container == null || Thumb == null) return false;

            // 인라인
            Vector? ToVector(Size? size) =>
                size.HasValue ? new Vector(size.Value.Width, size.Value.Height) : (Vector?)null;

            var thumbVector = ToVector(Thumb.Bounds.Size);
            var containerRenderedVector = ToVector(Container.Bounds.Size);
            var containerDesiredVector = ToVector(Container.DesiredSize);

            if (!thumbVector.HasValue || !containerRenderedVector.HasValue || !containerDesiredVector.HasValue)
                return false;

            var marginDifference = containerRenderedVector.Value - containerDesiredVector.Value;
            var adjustedPoint = (Point)thumbVector.Value / 2 - marginDifference / 2;

            var relativeLocation = Thumb.TranslatePoint(adjustedPoint, Container);
            if (!relativeLocation.HasValue) return false;

            Anchor = new Point(Container.Location.X + relativeLocation.Value.X,
                Container.Location.Y + relativeLocation.Value.Y);
            return true;
        }
        */
    }
}