using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Collections;
using Avalonia.Markup.Xaml.Templates;

namespace DAGlynEditor
{
    public class DAGlynEditor : SelectingItemsControl, IDisposable
    {
        #region Dependency Properties

        public static readonly StyledProperty<Point> ViewportLocationProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point>(
                nameof(ViewportLocation), Constants.ZeroPoint);

        public Point ViewportLocation
        {
            get => GetValue(ViewportLocationProperty);
            set => SetValue(ViewportLocationProperty, value);
        }

        public static readonly DirectProperty<DAGlynEditor, Point> MouseLocationProperty =
            AvaloniaProperty.RegisterDirect<DAGlynEditor, Point>(
                nameof(MouseLocation),
                o => o.MouseLocation);

        private Point _mouseLocation;

        public Point MouseLocation
        {
            get => _mouseLocation;
            private set => SetAndRaise(MouseLocationProperty, ref _mouseLocation, value);
        }

        public static readonly StyledProperty<bool> DisablePanningProperty =
            AvaloniaProperty.Register<DAGlynEditor, bool>(nameof(DisablePanning));

        public bool DisablePanning
        {
            get => GetValue(DisablePanningProperty);
            set => SetValue(DisablePanningProperty, value);
        }

        public static readonly DirectProperty<DAGlynEditor, bool> IsSelectingProperty =
            AvaloniaProperty.RegisterDirect<DAGlynEditor, bool>(
                nameof(IsSelecting),
                o => o.IsSelecting);

        private bool _isSelecting;

        public bool IsSelecting
        {
            get => _isSelecting;
            internal set => SetAndRaise(IsSelectingProperty, ref _isSelecting, value);
        }

        public static readonly StyledProperty<bool> EnableRealtimeSelectionProperty =
            AvaloniaProperty.Register<DAGlynEditor, bool>(
                nameof(EnableRealtimeSelection));

        public bool EnableRealtimeSelection
        {
            get => GetValue(EnableRealtimeSelectionProperty);
            set => SetValue(EnableRealtimeSelectionProperty, value);
        }

        public static readonly DirectProperty<DAGlynEditor, Rect> SelectedAreaProperty =
            AvaloniaProperty.RegisterDirect<DAGlynEditor, Rect>(
                nameof(SelectedArea),
                o => o.SelectedArea);

        private Rect _selectedArea;

        public Rect SelectedArea
        {
            get => _selectedArea;
            internal set => SetAndRaise(SelectedAreaProperty, ref _selectedArea, value);
        }

        public static readonly DirectProperty<DAGlynEditor, bool?> IsPreviewingSelectionProperty =
            AvaloniaProperty.RegisterDirect<DAGlynEditor, bool?>(
                nameof(IsPreviewingSelection),
                o => o.IsPreviewingSelection);

        private bool? _isPreviewingSelection;

        public bool? IsPreviewingSelection
        {
            get => _isPreviewingSelection;
            internal set => SetAndRaise(IsPreviewingSelectionProperty, ref _isPreviewingSelection, value);
        }

        public static readonly DirectProperty<DAGlynEditor, bool> IsPanningProperty =
            AvaloniaProperty.RegisterDirect<DAGlynEditor, bool>(
                nameof(IsPanning),
                o => o.IsPanning);

        private bool _isPanning = false;

        public bool IsPanning
        {
            get => _isPanning;
            protected internal set => SetAndRaise(IsPanningProperty, ref _isPanning, value);
        }

        // TODO 성능 이슈 있음.
        // 향후 Node, Connection 들이 저장될 collection 임.
        public static readonly AvaloniaProperty<AvaloniaList<object>> DAGItemsProperty =
            AvaloniaProperty.Register<DAGlynEditor, AvaloniaList<object>>(
                nameof(DAGItems));

        public AvaloniaList<object> DAGItems
        {
            get => GetValue(DAGItemsProperty) as AvaloniaList<object> ?? new AvaloniaList<object>();
            set => SetValue(DAGItemsProperty, value);
        }

        // 추가
        public static readonly StyledProperty<DataTemplate?> PendingConnectionTemplateProperty =
            AvaloniaProperty.Register<DAGlynEditor, DataTemplate?>(
                nameof(PendingConnectionTemplate));

        public DataTemplate? PendingConnectionTemplate
        {
            get => GetValue(PendingConnectionTemplateProperty);
            set => SetValue(PendingConnectionTemplateProperty, value);
        }

        public static readonly StyledProperty<object?> PendingConnectionProperty =
            AvaloniaProperty.Register<DAGlynEditor, object?>(
                nameof(PendingConnection));

        public object? PendingConnection
        {
            get => GetValue(PendingConnectionProperty);
            set => SetValue(PendingConnectionProperty, value);
        }

        // PendingConnection 개체와 연결할 속성들.
        // 이 속성값들은 connector 에서 넘어온 값들을 전달해주는 역활을 한다.
        public static readonly StyledProperty<Point?> SourceAnchorProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point?>(nameof(SourceAnchor));

        // 연결의 끝점
        public static readonly StyledProperty<Point?> TargetAnchorProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point?>(nameof(TargetAnchor));

        public Point? SourceAnchor
        {
            get => GetValue(SourceAnchorProperty);
            set => SetValue(SourceAnchorProperty, value);
        }

        public Point? TargetAnchor
        {
            get => GetValue(TargetAnchorProperty);
            set => SetValue(TargetAnchorProperty, value);
        }

        // PendingConnection visible 설정에 사용
        public static readonly StyledProperty<bool> IsVisiblePendingConnectionProperty =
            AvaloniaProperty.Register<DAGlynEditor, bool>(
                nameof(IsVisiblePendingConnection));

        public bool IsVisiblePendingConnection
        {
            get => GetValue(IsVisiblePendingConnectionProperty);
            set => SetValue(IsVisiblePendingConnectionProperty, value);
        }

        #endregion

        #region Constructors

        public DAGlynEditor()
        {
            ItemsSource = MyItems;
            InitializeSubscriptions();
            SetValue(DAGItemsProperty, new AvaloniaList<object>());
            // TODO 이거 확인하자. 꼭~~
            this.Unloaded += (_, _) => this.Dispose();
        }

        #endregion

        #region Fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private EventHandler<PendingConnectionEventArgs>? _connectionStartedHandler;
        private EventHandler<PendingConnectionEventArgs>? _connectionDragHandler;
        private EventHandler<PendingConnectionEventArgs>? _connectionCompleteHandler;

        // Panning 관련 포인터 위치 값 
        private Point _previousPointerPosition;

        private Point _currentPointerPosition;
        //private bool _isPanning;

        // 일단 이렇게 하고 이거 나중에 static constructor 에 넣자. 
        //private PendingConnection? _pendingConnection = new PendingConnection();

        #endregion

        #region Event Handlers

        private void InitializeSubscriptions()
        {
            _connectionStartedHandler = HandleConnectionStarted;
            _connectionDragHandler = HandleConnectionDrag;
            _connectionCompleteHandler = HandleConnectionComplete;

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

            // 이벤트 핸들러 등록
            AddHandler(Connector.PendingConnectionStartedEvent, _connectionStartedHandler);
            AddHandler(Connector.PendingConnectionDragEvent, _connectionDragHandler);
            AddHandler(Connector.PendingConnectionCompletedEvent, _connectionCompleteHandler);

            // 이벤트 핸들러 해제
            _disposables.Add(Disposable.Create(() =>
            {
                RemoveHandler(Connector.PendingConnectionStartedEvent, _connectionStartedHandler);
                RemoveHandler(Connector.PendingConnectionDragEvent, _connectionDragHandler);
                RemoveHandler(Connector.PendingConnectionCompletedEvent, _connectionCompleteHandler);
            }));
        }

        private void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
            if (args.GetCurrentPoint(this).Properties.IsRightButtonPressed && !DisablePanning)
            {
                args.Pointer.Capture(this);
                _previousPointerPosition = args.GetPosition(this);
                IsPanning = true;
                args.Handled = true;
            }
        }

        private void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
            if (IsPanning)
            {
                _currentPointerPosition = args.GetPosition(this);
                ViewportLocation -=
                    (_currentPointerPosition - _previousPointerPosition) / 1; // Adjust division based on actual zoom level
                _previousPointerPosition = _currentPointerPosition;
                args.Handled = true;
            }
        }

        private void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
            if (IsPanning && args.Pointer.Captured == this)
            {
                args.Pointer.Capture(null);
                IsPanning = false;
                args.Handled = true;
            }
        }

        // _pendingConnection 에 직접 연결 불가능.
        // 이거 Editor 의 속성에 연결해서 이 것을 받는 방향으로 해야 할듯한데 고민해보자.
        private void HandleConnectionStarted(object? sender, PendingConnectionEventArgs args)
        {
            // TODO 추후 최적화 하자.
            if (sender == null) return;
            //if (sender is not OutConnector) return;

            /*
             * 아직 어떤 값들이 넘어오는지는 확인하지 않았다.
             */
            if (args.Source is OutConnector)
            {
                IsVisiblePendingConnection = true;

                if (args.Anchor.HasValue)
                {
                    SourceAnchor = args.Anchor.Value;
                    if (args.Offset.HasValue)
                        TargetAnchor = new Point(SourceAnchor.Value.X + args.Offset.Value.X,
                            SourceAnchor.Value.Y + args.Offset.Value.Y);
                    else TargetAnchor = SourceAnchor;
                }
                else
                {
                    SourceAnchor = null;
                    TargetAnchor = null;
                }

                args.Handled = true;
            }

            Debug.WriteLine("Ok!!!");
        }

        private void HandleConnectionDrag(object? sender, PendingConnectionEventArgs args)
        {
            /*if (args.Source is OutConnector)
            {
                if (_pendingConnection == null) return;

                if (_pendingConnection.IsVisible == false)
                    _pendingConnection.IsVisible = true;
                if (args.Offset.HasValue)
                {
                    _pendingConnection.TargetAnchor = (Point)args.Offset.Value;
                }
            }*/

            // Handled 관련해서는 일단 테스트 후 생각해보자.
            if (IsVisiblePendingConnection)
            {
                //e.Handled = true;

                if (args.Offset.HasValue)
                {
                    TargetAnchor = new Point(SourceAnchor.Value.X + args.Offset.Value.X,
                        SourceAnchor.Value.Y + args.Offset.Value.Y);
                }

                args.Handled = true;

                // TODO 아래 코드는 좀더 생각하자.
                //TargetAnchor = new Point(e.Anchor.X + e.OffsetX, e.Anchor.Y + e.OffsetY);

                /*if (Editor != null && (EnablePreview || EnableSnapping))
                {
                    // Look for a potential connector
                    FrameworkElement? connector = GetPotentialConnector(Editor, AllowOnlyConnectors);

                    // Update the connector's anchor and snap to it if snapping is enabled
                    if (EnableSnapping && connector is Connector target)
                    {
                        target.UpdateAnchor();
                        TargetAnchor = target.Anchor;
                    }

                    // If it's not the same connector
                    if (connector != _previousConnector)
                    {
                        if (_previousConnector != null)
                        {
                            SetIsOverElement(_previousConnector, false);
                        }

                        // And we have a connector
                        if (connector != null)
                        {
                            SetIsOverElement(connector, true);

                            // Update the preview target if enabled
                            if (EnablePreview)
                            {
                                PreviewTarget = connector.DataContext;
                            }
                        }

                        _previousConnector = connector;
                    }
                }*/
            }
        }

        private void HandleConnectionComplete(object? sender, PendingConnectionEventArgs args)
        {
            args.Handled = true;
            IsVisiblePendingConnection = false;
        }

        #endregion

        #region Methods

        private void UpdateMouseLocation(PointerEventArgs args)
        {
            MouseLocation = args.GetPosition(this);
        }

        // TODO Unload 와 관련 및 GC 관련 해서 생각해보자.
        public void Dispose()
        {
            _disposables.Dispose();
        }

        #endregion

        /*/// <inheritdoc />
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
        }*/

        // 테스트로 일단 이렇게 제작한다. 테스트 후 이후 내용 변경함.
        /// <inheritdoc />
        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            var testConnector = item as TestConnector;
            if (testConnector != null)
            {
                switch (testConnector.ConType)
                {
                    case ConnectorType.OutConnector:
                        return new OutConnector();
                    case ConnectorType.InConnector:
                        return new InConnector();
                    default:
                        break;
                }
            }

            return new Node();
        }

        // 컨테이너 설정
        // 컨테이너를 바로 Node 로 설정해주는 것도 가능할 것으로 판단된다.
        // 일단 주석으로 남겨 놓았는데, 움직임을 구현하기 위해서는 남겨놓는다.
        /// <inheritdoc />
        protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
        {
            if (container is Node myContainer)
                myContainer.Location = new Point(10, 10);

            if (container is InConnector inConnector && item is TestConnector inCon)
                inConnector.Location = inCon.Location;

            if (container is OutConnector outConnector && item is TestConnector outCon)
                outConnector.Location = outCon.Location;
        }

        #region Tests

        public ObservableCollection<Node> MyItems { get; set; } = new ObservableCollection<Node>
    {
        new Node()
    };

        // TODO 이걸로 속성하나 만들어야 할듯하다.
        // 그리고 여기에 Node 들을 추가하는 메서드들을 만들자.
        public AvaloniaList<Node> NodeItems { get; set; } = new AvaloniaList<Node>();

        #endregion
    }
}