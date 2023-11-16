using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Platform;
using System;

namespace DAGlynEditor
{
    public enum ConnectionOffsetMode
    {
        /// <summary>
        /// No offset applied.
        /// </summary>
        None,

        /// <summary>
        /// The offset is applied in a circle around the point.
        /// </summary>
        Circle,

        /// <summary>
        /// The offset is applied in a rectangle shape around the point.
        /// </summary>
        Rectangle,

        /// <summary>
        /// The offset is applied in a rectangle shape around the point, perpendicular to the edges.
        /// </summary>
        Edge,
    }


    /// <summary>
    /// The direction in which a connection is oriented.
    /// </summary>
    /// 방향에 대한 정의를 한다.
    public enum ConnectionDirection
    {
        Forward,
        Backward
    }

    /// <summary>
    /// The end at which the arrow head is drawn.
    /// </summary>
    public enum ArrowHeadEnds
    {
        /// <summary>
        /// Arrow head at start.
        /// </summary>
        Start,

        /// <summary>
        /// Arrow head at end.
        /// </summary>
        End,

        /// <summary>
        /// Arrow heads at both ends.
        /// </summary>
        Both,

        /// <summary>
        /// No arrow head.
        /// </summary>
        None
    }

    public class Connection : Shape
    {
        #region feilds
        // 내부적으로 사용하는 변수를 이렇게 선언해 두었는데, 추후 수정 여부를 결정하자.
        // ReSharper disable once InconsistentNaming
        private const double _baseOffset = 100d;
        // ReSharper disable once InconsistentNaming
        private const double _offsetGrowthRate = 25d;

        /// <summary>
        /// Gets a vector that has its coordinates set to 0.
        /// </summary>
        protected static readonly Vector ZeroVector = new(0d, 0d);
        #endregion

        #region Dependency Properties
        // Point, Size 의 경우는 Avalonia.Point, Avalonia.Size 로 사용한다.  
        public static readonly StyledProperty<Point> SourceProperty =
            AvaloniaProperty.Register<Connection, Point>(nameof(Source), default);

        public static readonly StyledProperty<Point> TargetProperty =
            AvaloniaProperty.Register<Connection, Point>(nameof(Target), default);

        public static readonly StyledProperty<Size> SourceOffsetProperty =
            AvaloniaProperty.Register<Connection, Size>(nameof(SourceOffset), default);

        public static readonly StyledProperty<Size> TargetOffsetProperty =
            AvaloniaProperty.Register<Connection, Size>(nameof(TargetOffset), default);

        public static readonly StyledProperty<ConnectionOffsetMode> OffsetModeProperty =
            AvaloniaProperty.Register<Connection, ConnectionOffsetMode>(nameof(OffsetMode), default);

        public static readonly StyledProperty<ConnectionDirection> DirectionProperty =
            AvaloniaProperty.Register<Connection, ConnectionDirection>(nameof(Direction), default);

        public static readonly StyledProperty<ArrowHeadEnds> ArrowHeadEndsProperty =
            AvaloniaProperty.Register<Connection, ArrowHeadEnds>(nameof(ArrowEnds), ArrowHeadEnds.End);

        public static readonly StyledProperty<double> SpacingProperty =
            AvaloniaProperty.Register<Connection, double>(nameof(Spacing), 30);

        public static readonly StyledProperty<Size> ArrowSizeProperty =
            AvaloniaProperty.Register<Connection, Size>(nameof(ArrowSize), defaultValue: BoxValue.ArrowSize);

        public Point Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public Point Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public Size SourceOffset
        {
            get => GetValue(SourceOffsetProperty);
            set => SetValue(SourceOffsetProperty, value);
        }

        public Size TargetOffset
        {
            get => GetValue(TargetOffsetProperty);
            set => SetValue(TargetOffsetProperty, value);
        }

        public ConnectionOffsetMode OffsetMode
        {
            get => GetValue(OffsetModeProperty);
            set => SetValue(OffsetModeProperty, value);
        }

        public ConnectionDirection Direction
        {
            get => GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public ArrowHeadEnds ArrowEnds
        {
            get => GetValue(ArrowHeadEndsProperty);
            set => SetValue(ArrowHeadEndsProperty, value);
        }

        public double Spacing
        {
            get => GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        public Size ArrowSize
        {
            get => GetValue(ArrowSizeProperty);
            set => SetValue(ArrowSizeProperty, value);
        }
        #endregion

        #region Constructors

        //TODO 다른 컨스트럭터도 필요할듯.
        static Connection()
        {
            // 초기값 설정
            StrokeThicknessProperty.OverrideDefaultValue<Connection>(3);
            StrokeProperty.OverrideDefaultValue<Connection>(Brushes.Black);
            FillProperty.OverrideDefaultValue<Connection>(Brushes.Black);

            // TODO 8/12/23 
            // notion 에 일단 기록해두었지만, 향후 확장성을 생각했을때 어떻게 할지를 확인하고 결정해야 한다. TODO 로 남겨 놓는다.
            // SpacingProperty.OverrideDefaultValue<Connection>(30);

            // 속성값이 변할때 마다 렌더링이 다시 될 수 있도록 하는 코드를 작성한다.
            // AffectsGeometry
            AffectsGeometry<Connection>(
                SourceProperty,
                TargetProperty,
                SourceOffsetProperty,
                TargetOffsetProperty,
                OffsetModeProperty,
                DirectionProperty,
                ArrowHeadEndsProperty,
                SpacingProperty,
                ArrowSizeProperty
                );
        }
        #endregion

        // 일단 Avalonia 형식으로 변환 하였음.
        protected override Geometry CreateDefiningGeometry()
        {
            var geometry = new StreamGeometry();

            using var context = geometry.Open();
            // FillRule을 EvenOdd로 설정
            context.SetFillRule(FillRule.EvenOdd);

            // 오프셋 계산 및 소스와 타겟 점 업데이트
            var (sourceOffset, targetOffset) = GetOffset();
            var source = Source + sourceOffset;
            var target = Target + targetOffset;

            // 선 그리기
            DrawLineGeometry(context, source, target);

            // 화살표 그리기 (화살표 크기가 0이 아닌 경우) None, Default 는 생략했음.
            if (ArrowSize.Width != 0d && ArrowSize.Height != 0d)
            {
                switch (ArrowEnds)
                {
                    case ArrowHeadEnds.Start:
                        DrawArrowGeometry(context, source, target, ConnectionDirection.Backward);

                        break;
                    case ArrowHeadEnds.End:
                        DrawArrowGeometry(context, source, target, ConnectionDirection.Forward);

                        break;
                    case ArrowHeadEnds.Both:
                        DrawArrowGeometry(context, source, target, ConnectionDirection.Forward);
                        DrawArrowGeometry(context, target, source, ConnectionDirection.Backward);

                        break;
                }
            }

            return geometry;
        }

        // 내부적으로 사용하는 메서드들

        // DrawLineGeometry
        // Connection.cs 에서 가져옴.
        // Avalonia 로 수정해줘야 함.
        // CreateDefiningGeometry 에서 사용할 예정임.
        // 일단 테스트 용도로 public 으로 하였음.
        private void DrawLineGeometry(IGeometryContext context, Point source, Point target)
        {
            var direction = Direction == ConnectionDirection.Forward ? 1d : -1d;
            var spacing = new Vector(Spacing * direction, 0d);
            var arrowOffset = new Vector(ArrowSize.Width * direction, 0d);
            var endPoint = Spacing > 0 ? target - arrowOffset : target;

            context.BeginFigure(source, false);
            context.LineTo(source + spacing);
            context.LineTo(endPoint - spacing);
            context.LineTo(endPoint);
            context.EndFigure(false);
        }

        private void DrawArrowGeometry(IGeometryContext context, Point source, Point target, ConnectionDirection arrowDirection = ConnectionDirection.Forward)
        {
            var (from, to) = GetArrowHeadPoints(source, target, arrowDirection);

            context.BeginFigure(target, true);
            context.LineTo(from);
            context.LineTo(to);
            // Stroke 색상 가져오기
            //var strokeColor = Stroke is SolidColorBrush strokeBrush ? strokeBrush.Color : Colors.Black;

            // Fill 속성에 Stroke 색상 적용
            //context.Fill(new SolidColorBrush(strokeColor));
            context.EndFigure(true);
        }
        private (Point From, Point To) GetArrowHeadPoints(Point source, Point target, ConnectionDirection arrowDirection)
        {
            var headWidth = ArrowSize.Width;
            var headHeight = ArrowSize.Height;
            Point from;
            Point to;

            // Spacing이 1보다 작은 경우, 화살표의 머리 부분을 각도를 사용하여 계산합니다.
            if (Spacing < 1d)
            {
                var delta = source - target;
                var angle = Math.Atan2(delta.Y, delta.X);
                var sinT = Math.Sin(angle);
                var cosT = Math.Cos(angle);

                from = new Point(target.X + (headWidth * cosT - headHeight * sinT), target.Y + (headWidth * sinT + headHeight * cosT));
                to = new Point(target.X + (headWidth * cosT + headHeight * sinT), target.Y - (headHeight * cosT - headWidth * sinT));
            }
            // Spacing이 1보다 큰 경우, 화살표의 머리 부분을 방향에 따라 계산합니다.
            else
            {
                var direction = arrowDirection == ConnectionDirection.Forward ? 1d : -1d;
                from = new Point(target.X - headWidth * direction, target.Y + headHeight);
                to = new Point(target.X - headWidth * direction, target.Y - headHeight);
            }

            return (from, to);
        }
        private (Vector SourceOffset, Vector TargetOffset) GetOffset()
        {
            Vector delta = Target - Source;
            Vector delta2 = Source - Target;

            return OffsetMode switch
            {
                ConnectionOffsetMode.Rectangle => (GetRectangleModeOffset(delta, SourceOffset), GetRectangleModeOffset(delta2, TargetOffset)),
                ConnectionOffsetMode.Circle => (GetCircleModeOffset(delta, SourceOffset), GetCircleModeOffset(delta2, TargetOffset)),
                ConnectionOffsetMode.Edge => (GetEdgeModeOffset(delta, SourceOffset), GetEdgeModeOffset(delta2, TargetOffset)),
                ConnectionOffsetMode.None => (ZeroVector, ZeroVector),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Vector GetEdgeModeOffset(Vector delta, Size offset)
        {
            var xOffset = Math.Min(Math.Abs(delta.X) / 2d, offset.Width) * Math.Sign(delta.X);
            var yOffset = Math.Min(Math.Abs(delta.Y) / 2d, offset.Height) * Math.Sign(delta.Y);

            return new Vector(xOffset, yOffset);
        }

        // WPF 에서 LengthSquared 를 avalonia 에서 SquaredLength 로 변경함.
        private static Vector GetCircleModeOffset(Vector delta, Size offset)
        {
            if (delta.SquaredLength > 0d)
            {
                delta.Normalize();
            }

            return new Vector(delta.X * offset.Width, delta.Y * offset.Height);
        }
        // WPF 에서 LengthSquared 를 avalonia 에서 SquaredLength 로 변경함.
        // WPF 에서 delta.LengthSquared는 벡터의 길이를 제곱한 값을 반환
        // 세부적인 것은 확인하지 않았다. 따라서 추후 수정하면서 확인해야 한다.
        private static Vector GetRectangleModeOffset(Vector delta, Size offset)
        {
            if (delta.SquaredLength > 0d)
            {
                // WPF 에서 사용하는 Normalize() 메서드를 그대로 사용했다.
                delta.Normalize();
            }

            var angle = Math.Atan2(delta.Y, delta.X);
            //var result = new Vector();

            if (offset.Width * 2d * Math.Abs(delta.Y) < offset.Height * 2d * Math.Abs(delta.X))
            {
                var x = Math.Sign(delta.X) * offset.Width;
                var y = Math.Tan(angle) * x;
                return new Vector(x, y);

                //result.X = Math.Sign(delta.X) * offset.Width;
                //result.Y = Math.Tan(angle) * result.X;
            }
            else
            {
                var y = Math.Sign(delta.Y) * offset.Height;
                var x = 1.0d / Math.Tan(angle) * y;
                return new Vector(x, y);
            }
            //return result;
        }
    }
}
