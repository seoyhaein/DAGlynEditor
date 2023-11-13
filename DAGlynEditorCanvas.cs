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

        // TODO(중요) 이거 자체에 문제가 있는 듯하다.
        // RX 를 적용해야 할듯.
        // TODO Animation 을 추가해보는 것 생각하자. 
        // 이 부분을 개선하는 것은 중요하다.
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
    }
}