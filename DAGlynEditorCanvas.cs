using Avalonia.Controls.Primitives;
using Avalonia;

namespace DAGlynEditor
{
    public class DAGlynEditorCanvas : OverlayLayer
    {
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
    }
}
