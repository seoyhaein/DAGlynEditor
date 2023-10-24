using System;
using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using System.Diagnostics;
using Avalonia.Input;
using System.ComponentModel;
using Avalonia.Remote.Protocol.Input;
using System.Linq;
using Avalonia.Controls;

namespace DAGlynEditor
{
    public class DAGlynEditorCanvas : OverlayLayer
    {
        #region Offset 화면이동.
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
                translateTransform.X = -pointValue.X;
                translateTransform.Y = -pointValue.Y;
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

        // 추후 수정한다.Move, Release 에 배치할까? Release 에는 안해도 될 것 같지만 생각해보자. 일단 배치하자.
        private void StartAnimation(Point newLocation)
        {
            if (RenderTransform == null)
            {
                RenderTransform = new TranslateTransform();
            }

            if (Transitions == null)
            {
                Transitions = new Transitions();
            }

            // 임시로 일단 ViewportZoom, BringIntoViewSpeed, BringIntoViewMaxDuration 를 넣어둠.
            double ViewportZoom = 1d;
            double BringIntoViewSpeed = 1d;
            double BringIntoViewMaxDuration = 1d;
            double distance = ((Vector)(newLocation - Offset)).Length;
            double duration = distance / (BringIntoViewSpeed + (distance / 10)) * ViewportZoom;
            duration = Math.Max(0.1, Math.Min(duration, BringIntoViewMaxDuration));

            // 기존 PointTransition 찾기
            var existingTransition = Transitions.OfType<PointTransition>().FirstOrDefault(t => t.Property == OffsetProperty);

            if (existingTransition != null)
            {
                // 이미 존재하는 PointTransition의 Duration 업데이트
                existingTransition.Duration = TimeSpan.FromSeconds(duration);
            }
            else
            {
                // 존재하지 않으면 새로운 PointTransition 생성 및 추가
                var pointTransition = new PointTransition
                {
                    Property = OffsetProperty,
                    Duration = TimeSpan.FromSeconds(duration)
                };
                Transitions.Add(pointTransition);
            }
        }

    }
}
