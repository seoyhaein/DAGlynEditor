﻿using System;
using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using System.Diagnostics;

namespace DAGlynEditor
{
    public class DAGlynEditorCanvas : OverlayLayer
    {
        #region Viewport 화면에 이동에 사용될 예정, 일단 구현만 먼저 확인한다. 추후 코드 최적화 함.
        public static readonly StyledProperty<Point> OffsetProperty =
           AvaloniaProperty.Register<DAGlynEditorCanvas, Point>(nameof(Offset), default);

        public Point Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        #endregion

        public DAGlynEditorCanvas()
        {
            RenderTransform = new TranslateTransform();
            Transitions = new Transitions
            {
                new PointTransition
                {
                    Property = OffsetProperty,
                    Duration = TimeSpan.FromSeconds(0.5)
                }
            };
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
        // TODO 이거 자체에 문제가 있는 듯하다.
        // RX 를 적용해야 할듯.
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == OffsetProperty && change.NewValue is Point pointValue && RenderTransform is TranslateTransform translateTransform)
            {
                Debug.WriteLine($"Canvas ViewPort changed to: {change.NewValue}");
                translateTransform.X = pointValue.X;
                translateTransform.Y = pointValue.Y;
            }
        }
    }
}
