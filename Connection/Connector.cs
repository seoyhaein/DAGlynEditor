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
    public class Connector : TemplatedControl, IDisposable
    {
        #region Constructor

        public Connector()
        {
            InitializeSubscriptions();
        }

        /*static Connector()
        {
            FocusableProperty.OverrideDefaultValue<Connector>(true);
        }*/

        #endregion

        #region Routed Events

        public static readonly RoutedEvent<PendingConnectionEventArgs> PendingConnectionStartedEvent =
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

        #region Fields & Dependency Properties

        // TODO Anchor 가 필요한지 일단 모르겠지만 넣어둠.
        public static readonly StyledProperty<Point> AnchorProperty =
            AvaloniaProperty.Register<Connector, Point>(nameof(Anchor));

        public Point Anchor
        {
            get => GetValue(AnchorProperty);
            set => SetValue(AnchorProperty, value);
        }

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        protected bool IsPointerPressed;
        protected Connector? PreviousConnector;

        #endregion

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

        protected virtual void HandlePointerPressed(object? sender, PointerPressedEventArgs args) { }
        protected virtual void HandlePointerMoved(object? sender, PointerEventArgs args) { }
        protected virtual void HandlePointerReleased(object? sender, PointerReleasedEventArgs args) { }

        // DataContext 는 살펴보자.
        /*private void StartedRaiseEvent(object? sender)
        {
            var args = new PendingConnectionEventArgs(PendingConnectionStartedEvent, this, DataContext)
            {
                Anchor = Anchor,
                Sender = sender,
            };

            RaiseEvent(args);
        }

        // 빈공란으로 향후 남겨두자.
        private void DragRaiseEvent(object? sender ,Vector? offset)
        {
            if (offset == null) return;

            var args = new PendingConnectionEventArgs(PendingConnectionDragEvent, this, DataContext)
            {
                OffsetX = offset.Value.X,
                OffsetY = offset.Value.Y,
                Sender = sender,
            };

            RaiseEvent(args);
        }

        private void CompletedRaiseEvent(object? sender)
        {
            // PendingConnectionEventArgs(DataContext) 관련해서 살펴봐야 함.
            var args = new PendingConnectionEventArgs(PendingConnectionCompletedEvent, this, DataContext)
            {
                Sender = sender,
                //Anchor = Anchor,
            };
            RaiseEvent(args);
        }*/

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}