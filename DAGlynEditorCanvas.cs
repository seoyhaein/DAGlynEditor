using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.Media;

namespace DAGlynEditor
{
    public class DAGlynEditorCanvas : OverlayLayer
    {
        #region ViewportLocation 화면이동.

        public static readonly StyledProperty<Point> ViewportLocationProperty =
            AvaloniaProperty.Register<DAGlynEditorCanvas, Point>(nameof(ViewportLocation), default(Point));

        public Point ViewportLocation
        {
            get => GetValue(ViewportLocationProperty);
            set => SetValue(ViewportLocationProperty, value);
        }

        #endregion

        public DAGlynEditorCanvas()
        {
            RenderTransform = new TranslateTransform();
            this.GetPropertyChangedObservable(ViewportLocationProperty).Subscribe(OnViewportLocationChanged);
        }

        private void OnViewportLocationChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is Point pointValue)
            {
                if (RenderTransform is TranslateTransform translateTransform)
                {
                    translateTransform.X = -pointValue.X;
                    translateTransform.Y = -pointValue.Y;
                }
            }
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
    }
}