using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia;
using System.Diagnostics;
using Avalonia.Input;

namespace DAGlynEditor
{
    public class DAGlynEditor : SelectingItemsControl
    {
        #region Offset 화면이동.

        private Point _initialMousePosition;
        private Point _previousMousePosition;
        private Point _currentMousePosition;
        public static double HandleRightClickAfterPanningThreshold { get; set; } = 12d;

        public static readonly StyledProperty<Point> OffsetProperty =
            AvaloniaProperty.Register<DAGlynEditorCanvas, Point>(nameof(Offset), default);

        public Point Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
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
    }
}