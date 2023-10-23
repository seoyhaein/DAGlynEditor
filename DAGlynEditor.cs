
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia;
using System.Diagnostics;

namespace DAGlynEditor
{
    public class DAGlynEditor : SelectingItemsControl
    {
        public static readonly StyledProperty<Point> OffsetProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point>(nameof(Offset), default);

        public Point Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
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
