using System;
using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using System.Diagnostics;
using Avalonia.Input;
using System.ComponentModel;
using Avalonia.Remote.Protocol.Input;

namespace DAGlynEditor
{
    public class DAGlynEditorCanvas : OverlayLayer
    {
        #region Viewport 화면에 이동에 사용될 예정, 일단 구현만 먼저 확인한다. 추후 코드 최적화 함.
        public static readonly StyledProperty<Point> OffsetProperty =
           AvaloniaProperty.Register<DAGlynEditorCanvas, Point>(nameof(Offset), default);

        public Point Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        #endregion

        public DAGlynEditorCanvas()
        {
            RenderTransform = new TranslateTransform();
            Transitions = new Transitions
            {
                new PointTransition
                {
                    Property = OffsetProperty,
                    Duration = TimeSpan.FromSeconds(0.5)
                }
            };

            // 이벤트 직점 처리. Dispose 도 생각해보자.
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerMoved += OnPointerMoved;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var child = Children;

            if (child.Count > 0)
            {
                var locationProperty = child[0].GetType().GetProperty("Location");

                if (locationProperty != null)
                {
                    foreach (var item in child)
                    {
                        var locationValue = locationProperty.GetValue(item);
                        if (locationValue is Point location)
                        {
                            item.Arrange(new Rect(location, item.DesiredSize));
                        }
                    }
                }
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (var child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return default;
        }
        // TODO 이거 자체에 문제가 있는 듯하다.
        // RX 를 적용해야 할듯.
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == OffsetProperty && change.NewValue is Point pointValue && RenderTransform is TranslateTransform translateTransform)
            {
                Debug.WriteLine($"Canvas ViewPort changed to: {change.NewValue}");
                translateTransform.X = pointValue.X;
                translateTransform.Y = pointValue.Y;
            }
        }
        // 라우트된 이벤트로 처리하지 않고 직접 처리함.
        // sender 도 조사할 필요가 있을까?
        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // 마우스 오른쪽 버튼 이벤트만 처리함.   
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                if (e.Pointer.Captured != null) 
                {
                    e.Pointer.Capture(null);
                }

                e.Pointer.Capture(this);

                Offset = e.GetPosition(this);
                e.Handled = true;
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                if (e.Pointer.Captured == this)
                {
                    Offset = e.GetPosition(this);
                    e.Handled = true;
                }
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e) 
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
            {
                if (e.Pointer.Captured == this)
                {
                    Offset = e.GetPosition(this);
                    e.Handled = true;

                    e.Pointer.Capture(null);
                }
            }
        }

        // 일단 여기서 마우스 이벤트를 처리하는 방향으로 한다. 왜 밖으로 빼는지을 이해가 안된다.
       /* protected override void OnPointerPressed(PointerPressedEventArgs e)
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
               
            }
            e.Handled = true;
        }*/
    }
}
