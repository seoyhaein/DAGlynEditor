using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace PendingConnection
{
    // TODO 값을 설정하지 못하지 않을까?
    // 추후 수정해야함. 일단, 에러때문에 여기다 나둠.
    public interface ILocatable
    {
        Point Location { get; }
    }

    // 최대값을 설정해주면 된다.
    public class TemplateLayoutCanvas : Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            double maxWidth = 0.0;
            double maxHeight = 0.0;

            foreach (var child in Children)
            {
                child.Measure(constraint);

                // 자식 컨트롤이 ILocatable 인터페이스를 구현하는 경우, Location 속성을 사용
                Point location = child is ILocatable locatableChild ? locatableChild.Location : new Point(0, 0);

                double childRight = location.X + child.DesiredSize.Width;
                double childBottom = location.Y + child.DesiredSize.Height;

                maxWidth = Math.Max(maxWidth, childRight);
                maxHeight = Math.Max(maxHeight, childBottom);
            }

            return new Size(maxWidth, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children)
            {
                // ILocatable 인터페이스를 구현하는지 확인
                if (child is ILocatable locatableChild)
                {
                    Point location = locatableChild.Location;
                    child.Arrange(new Rect(location, child.DesiredSize));
                }
                else
                {
                    // ILocatable을 구현하지 않는 경우, 기본 위치나 다른 로직을 사용하여 Arrange를 수행
                    // 기본 위치를 (0, 0)으로 설정
                    child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                }
            }

            return finalSize;
        }

    }
}

