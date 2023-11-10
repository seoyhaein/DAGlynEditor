using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia;
using System.Diagnostics;
using Avalonia.Input;
using System.Collections.Generic;

namespace DAGlynEditor
{
    public class DAGlynEditor : SelectingItemsControl
    {
        // 이것도 이름 수정해서 동일하게 작성할 예정임. 일단 동일하게 작성하고 추후 업데이트 시에 변경하는 방향으로 진행한다.
        #region Offset 화면이동.

        private Point _initialMousePosition;
        private Point _previousMousePosition;
        private Point _currentMousePosition;
        public static double HandleRightClickAfterPanningThreshold { get; set; } = 12d;
        
        //TODO ViewportLocation  으로 변경할 예정임. 변경후 작업 시작으로 넣어 둔다.
        public static readonly StyledProperty<Point> OffsetProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point>(nameof(Offset), default);

        public Point Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        #endregion
        
        # region 작업시작
        
        // readonly
        // MouseLocation 만들어줌. 
        public static readonly DirectProperty<DAGlynEditor, Point> MouseLocationProperty  =
            AvaloniaProperty.RegisterDirect<DAGlynEditor, Point>(
                nameof(MouseLocation),
                o => o.MouseLocation);

        private Point _mouseLocation;

        public Point MouseLocation
        {
            get => _mouseLocation; 
            private set => SetAndRaise(MouseLocationProperty, ref _mouseLocation, value); 
        }
        
        // DisablePanningProperty
        // public static readonly DependencyProperty DisablePanningProperty = DependencyProperty.Register(nameof(DisablePanning), typeof(bool), typeof(NodifyEditor),
        // new FrameworkPropertyMetadata(BoxValue.False, OnDisablePanningChanged))
        public static readonly StyledProperty<bool> DisablePanningProperty =
            AvaloniaProperty.Register<DAGlynEditor, bool>(nameof(DisablePanning), default);
        
        public bool DisablePanning
        {
            get => GetValue(DisablePanningProperty);
            set => SetValue(DisablePanningProperty, value);
        }
        
        // readonly
        // IsPanning
        // public static readonly DependencyPropertyKey IsPanningPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsPanning), typeof(bool), typeof(NodifyEditor), new FrameworkPropertyMetadata(BoxValue.False));
        // public static readonly DependencyProperty IsPanningProperty = IsPanningPropertyKey.DependencyProperty;
        public static readonly DirectProperty<DAGlynEditor, bool> IsPanningProperty  =
            AvaloniaProperty.RegisterDirect<DAGlynEditor, bool>(
                nameof(IsPanning),
                o => o.IsPanning);

        private bool _isPanning;

        public bool IsPanning
        {
            get => _isPanning; 
            internal set => SetAndRaise(IsPanningProperty, ref _isPanning, value); 
        }
        
        #endregion

        public DAGlynEditor()
        {
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerMoved += OnPointerMoved;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // 마우스 오른쪽 버튼 이벤트만 처리함.  
            // 오류가능성 있음. 추후 이벤트가 대체됨으로 그때 상세히 다룬다.
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                if (e.Pointer.Captured != null)
                {
                    e.Pointer.Capture(null);
                }

                e.Pointer.Capture(this);

                _initialMousePosition = e.GetPosition(this);
                _previousMousePosition = _initialMousePosition;
                _currentMousePosition = _initialMousePosition;
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                if (e.Pointer.Captured == this)
                {
                    // this가 Editor 였음. 잘생각해보자.
                    _currentMousePosition = e.GetPosition(this);
                    // 실제로 Editor.ViewportZoom 값을 측정해보자.
                    //Editor.ViewportLocation -= (_currentMousePosition - _previousMousePosition) / Editor.ViewportZoom;
                    Offset -= (_currentMousePosition - _previousMousePosition) / 1;
                    _previousMousePosition = _currentMousePosition;

                    //Offset = e.GetPosition(this);
                    //e.Handled = true;
                }
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
            {
                if (e.Pointer.Captured == this)
                {
                    double contextMenuTreshold =
                        HandleRightClickAfterPanningThreshold * HandleRightClickAfterPanningThreshold;
                    if (((Vector)(_currentMousePosition - _initialMousePosition)).SquaredLength > contextMenuTreshold)
                    {
                        e.Handled = true;
                        e.Pointer.Capture(null);
                    }

                    // Offset = e.GetPosition(this);
                    //e.Handled = true;
                }
            }
        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new MyRect();
        }

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

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == OffsetProperty)
            {
                // 여기에서 ViewPort 속성 값의 변화를 확인할 수 있습니다.
                Debug.WriteLine($"ViewPort changed to: {change.NewValue}");
            }
            //base.OnPropertyChanged(change);
        }

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