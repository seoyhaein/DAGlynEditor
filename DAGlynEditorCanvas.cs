using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.Media;
using System.Diagnostics;

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
            var children = Children;

            foreach (var child in children)
            {
                var locationProperty = child.GetType().GetProperty("Location");
                if (locationProperty != null)
                {
                    var locationValue = locationProperty.GetValue(child);
                    if (locationValue is Point location)
                    {
                        child.Arrange(new Rect(location, child.DesiredSize));
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