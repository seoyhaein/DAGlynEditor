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
        #region ViewportLocation 화면이동.

        public static readonly StyledProperty<Point> ViewportLocationProperty =
            AvaloniaProperty.Register<DAGlynEditorCanvas, Point>(nameof(ViewportLocation), default);

        public Point ViewportLocation
        {
            get => GetValue(ViewportLocationProperty);
            set => SetValue(ViewportLocationProperty, value);
        }

        #endregion

        public DAGlynEditorCanvas()
        {
            RenderTransform = new TranslateTransform();
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

            if (change.Property == ViewportLocationProperty && change.NewValue is Point pointValue &&
                RenderTransform is TranslateTransform translateTransform)
            {
                Debug.WriteLine($"Canvas ViewPort changed to: {change.NewValue}");
                translateTransform.X = -pointValue.X;
                translateTransform.Y = -pointValue.Y;
            }
        }

        // 라우트된 이벤트로 처리하지 않고 직접 처리함.
        // sender 도 조사할 필요가 있을까?
        // 일단 기록을 위해서 남겨둔다. 추후 수정 또는 삭제 예정
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
            double distance = ((Vector)(newLocation - ViewportLocation)).Length;
            double duration = distance / (BringIntoViewSpeed + (distance / 10)) * ViewportZoom;
            duration = Math.Max(0.1, Math.Min(duration, BringIntoViewMaxDuration));

            // 기존 PointTransition 찾기
            var existingTransition =
                Transitions.OfType<PointTransition>().FirstOrDefault(t => t.Property == ViewportLocationProperty);

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
                    Property = ViewportLocationProperty,
                    Duration = TimeSpan.FromSeconds(duration)
                };
                Transitions.Add(pointTransition);
            }
        }
    }
}