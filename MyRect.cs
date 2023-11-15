using Avalonia;
using Avalonia.Controls.Shapes;

namespace DAGlynEditor;

public class MyRect : Rectangle
{
    // 테스트로 만듬.
    public static readonly StyledProperty<Point?> LocationProperty =
        AvaloniaProperty.Register<MyRect, Point?>(nameof(Location));

    public Point? Location
    {
        get => GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }
    
}

public class MyEllipse : Ellipse
{
    // 테스트로 만듬.
    public static readonly StyledProperty<Point?> LocationProperty =
        AvaloniaProperty.Register<MyRect, Point?>(nameof(Location));

    public Point? Location
    {
        get => GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }

}