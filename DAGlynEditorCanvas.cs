using System;
using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;

namespace DAGlynEditor
{
    public class DAGlynEditorCanvas : OverlayLayer
    {
        # region Viewport 화면에 이동에 사용될 예정, 일단 구현만 먼저 확인한다. 추후 코드 최적화 함.
        public static readonly StyledProperty<Point> ViewPortProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point>(nameof(ViewPort), default);
    
        public Point ViewPort
        {
            get => GetValue(ViewPortProperty);
            set => SetValue(ViewPortProperty, value);
        }
        
        private Transitions SetupAnimation()
        {
            var pointAnimation = new Transitions
            {
                new PointTransition
                {
                    Property = ViewPortProperty, 
                    Duration = TimeSpan.FromSeconds(0.5)
                }
            };

            return pointAnimation;
        }
        
        #endregion
        
        public DAGlynEditorCanvas()
        {
            RenderTransform = new TranslateTransform();
            Transitions = SetupAnimation();
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
        
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ViewPortProperty && change.NewValue is Point pointValue && RenderTransform is TranslateTransform translateTransform)
            {
                translateTransform.X = pointValue.X;
                translateTransform.Y = pointValue.Y;
            }
        }
    }
}
