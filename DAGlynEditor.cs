using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace DAGlynEditor
{
    public class DAGlynEditor : SelectingItemsControl
    {
        public static double HandleRightClickAfterPanningThreshold { get; set; } = 12d;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #region 작업시작

        public static readonly StyledProperty<Point> ViewportLocationProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point>(nameof(ViewportLocation), default(Point));

        public Point ViewportLocation
        {
            get => GetValue(ViewportLocationProperty);
            set => SetValue(ViewportLocationProperty, value);
        }

        // readonly
        // MouseLocation 만들어줌. 
        // https://docs.avaloniaui.net/docs/next/guides/custom-controls/how-to-create-advanced-custom-controls#datavalidation-support
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

        // 이 속성 관련해서는 없어도 되는게 아닌지 생각해본다.
        // DisablePanningProperty
        // public static readonly DependencyProperty DisablePanningProperty = DependencyProperty.Register(nameof(DisablePanning), typeof(bool), typeof(NodifyEditor),
        // new FrameworkPropertyMetadata(BoxValue.False, OnDisablePanningChanged))
        public static readonly StyledProperty<bool> DisablePanningProperty =
            AvaloniaProperty.Register<DAGlynEditor, bool>(nameof(DisablePanning), false);

        public bool DisablePanning
        {
            get => GetValue(DisablePanningProperty);
            set => SetValue(DisablePanningProperty, value);
        }

        // readonly
        // IsPanning
        // public static readonly DependencyPropertyKey IsPanningPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsPanning), typeof(bool), typeof(NodifyEditor), new FrameworkPropertyMetadata(BoxValue.False));
        // public static readonly DependencyProperty IsPanningProperty = IsPanningPropertyKey.DependencyProperty;
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

        // readonly
        // protected static readonly DependencyPropertyKey IsSelectingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsSelecting), typeof(bool), typeof(NodifyEditor), new FrameworkPropertyMetadata(BoxValue.False, OnIsSelectingChanged));
        // public static readonly DependencyProperty IsSelectingProperty = IsSelectingPropertyKey.DependencyProperty;

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

        // public static readonly DependencyProperty EnableRealtimeSelectionProperty =
        // DependencyProperty.Register(nameof(EnableRealtimeSelection), typeof(bool), typeof(NodifyEditor), new FrameworkPropertyMetadata(BoxValue.False));

        public static readonly StyledProperty<bool> EnableRealtimeSelectionProperty =
            AvaloniaProperty.Register<DAGlynEditor, bool>(nameof(EnableRealtimeSelection), false);

        public bool EnableRealtimeSelection
        {
            get => GetValue(EnableRealtimeSelectionProperty);
            set => SetValue(EnableRealtimeSelectionProperty, value);
        }

        // protected static readonly DependencyPropertyKey SelectedAreaPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedArea), typeof(Rect), typeof(NodifyEditor), new FrameworkPropertyMetadata(BoxValue.Rect));
        // public static readonly DependencyProperty SelectedAreaProperty = SelectedAreaPropertyKey.DependencyProperty;

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

        // public static readonly DependencyPropertyKey IsPreviewingSelectionPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsPreviewingSelection), typeof(bool?), typeof(ItemContainer), new FrameworkPropertyMetadata(null));
        // public static readonly DependencyProperty IsPreviewingSelectionProperty = IsPreviewingSelectionPropertyKey.DependencyProperty;

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

        #endregion

        public DAGlynEditor()
        {
            InitializeSubscriptions();
            // state 초기화 시킴.
            _states.Push(GetInitialState());
        }

        static DAGlynEditor()
        {
            ViewportLocationProperty.Changed.Subscribe(OnViewportLocationChanged);
        }

        private static void OnViewportLocationChanged(AvaloniaPropertyChangedEventArgs e)
        {
            Debug.WriteLine($"ViewPort changed to: {e.NewValue}");
        }

        private void InitializeSubscriptions()
        {
            Observable.FromEventPattern<PointerPressedEventArgs>(
                    h => this.PointerPressed += h,
                    h => this.PointerPressed -= h)
                .Subscribe(args => HandlePointerPressed(args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PointerEventArgs>(
                    h => this.PointerMoved += h,
                    h => this.PointerMoved -= h)
                .Subscribe(args => HandlePointerMoved(args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PointerReleasedEventArgs>(
                    h => this.PointerReleased += h,
                    h => this.PointerReleased -= h)
                .Subscribe(args => HandlePointerReleased(args.EventArgs))
                .DisposeWith(_disposables);
        }

        // 이후 더 자세히 살펴보자 by seoy
        private void HandlePointerPressed(PointerPressedEventArgs args)
        {
            if (args.Pointer.Captured != null)
                args.Pointer.Capture(null);

            Focus();
            args.Pointer.Capture(this);

            UpdateMouseLocation(args);
            State.HandlePointerPressed(args);
            args.Handled = true;
        }

        private void HandlePointerMoved(PointerEventArgs args)
        {
            UpdateMouseLocation(args);
            State.HandlePointerMoved(args);
            args.Handled = true;
        }

        private void HandlePointerReleased(PointerReleasedEventArgs args)
        {
            UpdateMouseLocation(args);
            args.Pointer.Capture(null);
            State.HandlePointerReleased(args);
            args.Handled = true;
        }

        private void UpdateMouseLocation(PointerEventArgs args)
        {
            MouseLocation = args.GetPosition(this);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            // TODO 확인한다. 꼭. State.Enter(null)
            State.Enter(null);
        }

        // 컨테이너 생성
        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            // 아이템으로 컨테이너를 선택할 수 있도록 해야 할듯하다.
            // 컨테이너는 직접 Node 로 만들어주는 방향으로 간다.

            if (index == 0)
                return new MyRect();
            else if (index == 1)
                return new MyEllipse();
            else
                return new ItemContainer();

            //return new ItemContainer();
            //return new MyRect();
        }

        // 컨테이너 설정
        // 컨테이너를 바로 Node 로 설정해주는 것도 가능할 것으로 판단된다.
        protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
        {
            // 일단 임시 땜방 코드 테스트를 위해서.
            if (container is MyRect myRect && item is MyRectInfo info)
            {
                myRect.Location = info.Location;
                myRect.Width = info.W;
                myRect.Height = info.H;
                myRect.Fill = info.Br;
            }

            if (container is ItemContainer myContainer && item is MyRectInfo info1)
            {
                myContainer.Location = info1.Location;
                myContainer.Width = info1.W;
                myContainer.Height = info1.H;
                myContainer.Background = Brushes.Yellow;
                //myRect.Fill = info.Br;
            }

            if (container is MyEllipse myEll && item is MyRectInfo info2)
            {
                myEll.Location = info2.Location;
                myEll.Width = info2.W;
                myEll.Height = info2.H;
                myEll.Fill = info2.Br;
            }
        }

        #region State Handling

        private readonly Stack<EditorState> _states = new Stack<EditorState>();

        /// <summary>The current state of the editor.</summary>
        public EditorState State => _states.Peek();

        /// <summary>Creates the initial state of the editor.</summary>
        /// <returns>The initial state.</returns>
        protected virtual EditorState GetInitialState()
            => new EditorDefaultState(this);

        /// <summary>Pushes the given state to the stack.</summary>
        /// <param name="state">The new state of the editor.</param>
        /// <remarks>Calls <see cref="EditorState.Enter"/> on the new state.</remarks>
        public void PushState(EditorState state)
        {
            var prev = State;
            _states.Push(state);
            state.Enter(prev);
        }

        /// <summary>Pops the current <see cref="State"/> from the stack.</summary>
        /// <remarks>It doesn't pop the initial state. (see <see cref="GetInitialState"/>)
        /// <br />Calls <see cref="EditorState.Exit"/> on the current state.
        /// <br />Calls <see cref="EditorState.ReEnter"/> on the previous state.
        /// </remarks>
        public void PopState()
        {
            // Never remove the default state
            if (_states.Count > 1)
            {
                EditorState prev = _states.Pop();
                prev.Exit();
                State.ReEnter(prev);
            }
        }

        /// <summary>Pops all states from the editor.</summary>
        /// <remarks>It doesn't pop the initial state. (see <see cref="GetInitialState"/>)</remarks>
        public void PopAllStates()
        {
            while (_states.Count > 1)
            {
                PopState();
            }
        }

        #endregion
    }
}