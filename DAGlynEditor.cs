using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia;
using System.Diagnostics;
using Avalonia.Input;
using System.Collections.Generic;
using System;
using Avalonia.Threading;

namespace DAGlynEditor
{
    public class DAGlynEditor : SelectingItemsControl
    {
        public static double HandleRightClickAfterPanningThreshold { get; set; } = 12d;
        private DAGlynEditorCanvas? _itemsHost;

        #region 작업시작

        public static readonly StyledProperty<Point> ViewportLocationProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point>(nameof(ViewportLocation), default);

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
            // TODO Rx 방식으로 변경 예정.
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerMoved += OnPointerMoved;
            this.Loaded += OnLoaded;

            // state 초기화 시킴.
            _states.Push(GetInitialState());
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            // TODO 확인한다. 꼭. State.Enter(null);
            // OnDisableAutoPanningChanged(DisableAutoPanning);
            State.Enter(null);

        }

        // OnPointerPressed 관련해서 살펴본다.
        private void OnLoaded(object? sender, EventArgs e)
        {
            if (_itemsHost == null)
                GetDAGlynEditorCanvas();
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // TODO 생각하기 ItemsHost가 아닌 this로 변경. this 로 잡는게 맞는지 확신이 서지않는다. 
            // 최종적으로 코드 최적화할때 _itemsHost 를 넣어서 하는 것 테스트 해본다.

            // TODO 다른 이벤트에서도 확인해줘야 한다.
            if (_itemsHost == null) 
                GetDAGlynEditorCanvas();
            
            
            if (e.Pointer.Captured != null)
                e.Pointer.Capture(null);

            Focus();
            e.Pointer.Capture(this);
            MouseLocation = e.GetPosition(this);
            State.HandlePointerPressed(e);

        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            // TODO 생각하기 ItemsHost가 아닌 this로 변경
            MouseLocation = e.GetPosition(this);
            State.HandlePointerMoved(e);
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            MouseLocation = e.GetPosition(this);
            State.HandlePointerReleased(e);
            e.Pointer.Capture(null);
            
            // TODO 여기서 하는것이 더 나을듯하고, 다른 이벤트 핸들러에서도 해주는 것이 나을듯하다. 일단 생각해보자.
            // e.Handled = true;
        }

        // 컨테이너 생성
        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new MyRect();
        }

        // 컨테이너 설정
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
        }
        // TODO 시간날때 이름 생각해보자.
        private void GetDAGlynEditorCanvas() 
        {
            _itemsHost = this.FindControl<DAGlynEditorCanvas>("PART_ItemsHost");
        }
       
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == ViewportLocationProperty)
            {
                // 여기에서 ViewPort 속성 값의 변화를 확인할 수 있습니다.
                Debug.WriteLine($"ViewPort changed to: {change.NewValue}");
            }
            else if (change.Property == DisablePanningProperty)
            {

                // var editor = (DAGlynEditor)change.Sender;
                Debug.WriteLine($"DisablePanning changed to: {change.NewValue}");
            }
            //base.OnPropertyChanged(change);
        }

        /*private static void OnDisablePanningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (NodifyEditor)d;
            editor.OnDisableAutoPanningChanged(editor.DisableAutoPanning || editor.DisablePanning);
        }*/

        // 일단 동일하게 작성함.
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