using System;
using Avalonia;
using Avalonia.Controls;

namespace DAGlynEditor
{
   
    // 최대값을 설정해주면 된다.
    public class TemplateLayoutCanvas : Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            double maxWidth = 0.0;
            double maxHeight = 0.0;

            // TODO
            // 만약 w / h 설정되어 있으면 잘못된 방식으로 나타날텐데, 이문제를 어떻게 해결할지 생각해야함.
            // 고정적으로 코드에 넣어두면 안될 것 같은데....
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

            // TODO 화살표 사이즈가 커지거나 화살표 말고 다른 도형으로 대체했을 때는 사이즈를 조정해주거나 해야한다.
            // 사이즈를 자동으로 맞춰줘야하는 루틴이 필요하다.
            // 화살표를 화면에 다 담을려면 사이즈를 좀 확장해줘야 한다. 여기서 사이즈는 선분을 기준으로 잡기때문에 화살표 부분은 다 담을 수 없다.
            // 패딩 주어서 일단 주석처리 함.
            /*maxWidth += 20d;
            maxHeight += 20d;*/

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

                    //child.Arrange(new Rect(location.X, location.Y, child.DesiredSize.Width+20, child.DesiredSize.Height+20));
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