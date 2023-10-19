using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DAGlynEditor
{
    public static class Extensions
    {
        /* public static T? Find<T>(this Avalonia.Controls.INameScope scope, string name) where T : AvaloniaObject
         {
             if (scope.Find(name) is T result)
             {
                 return result;
             }
             return null;
         }*/

        // 일단 참조.
        // https://github.com/grokys/Avalonia/blob/master/Avalonia/Media/VisualTreeHelper.cs
        /*public static T? GetParentOfType<T>(this AvaloniaObject child) where T : AvaloniaObject
        {
            AvaloniaObject? current = child;

            do
            {
                var p = GetVisual(current);
                if (p == default) return default;
                current = GetParent(p);

                //current = VisualTreeHelper.GetParent(current);
                if (current == default)
                {
                    return default;
                }

            } while (!(current is T));

            return (T)current;
        }*/

        /* private static Visual? GetVisual(AvaloniaObject reference)
       {
           return reference as Visual;
       }

       private static Visual? GetParent(Visual reference)
       {
           return reference.GetVisualParent();
       }*/

        // TODO 테스트 하지 않음. 10-18-23 by seoyhaein
        public static T? GetParentOfType<T>(this AvaloniaObject child) where T : AvaloniaObject
        {
            var current = child as Visual;

            while (current != null)
            {
                if (current is T target)
                {
                    return target;
                }
                current = current.GetVisualParent();
            }

            return default;
        }
        // TODO 테스트 하지 않음. 10-18-23 by seoyhaein
        // 중요 이건 사용할때 테스트 하면서 구현하자.
        /*public static bool CaptureMouseSafe(this Pointer element)
        {
            if (element.Captured || InputManager.Instance?.MouseDevice?.Captured == null)
            {
                return element.CaptureMouse();
            }
            return false;
        }*/


        #region Animation

        /*public static void StartAnimation(this Control animatableElement, AvaloniaProperty property, Point toValue, double animationDurationSeconds, Action? completedAction = null)
        {
            if (animatableElement.GetValue(property) is Point fromValue)
            {
                
            }

            var animation = new PointAnimation
            {
                From = fromValue,
                To = toValue,
                Duration = TimeSpan.FromSeconds(animationDurationSeconds),
            };

            // Avalonia에서는 Completed 이벤트가 없으므로 다른 방식으로 완료 처리를 해야 합니다.
            // 여기서는 Task.Delay를 사용하여 애니메이션이 완료되었는지 확인합니다.
            animatableElement.Transitions = new Transitions
    {
        new PropertyTransition
        {
            Property = property,
            Duration = animation.Duration,
            SpeedRatio = 1.0,
            Easing = new LinearEasing(),
            RepeatCount = null,
            AutoReverse = false,
            Animation = animation
        }
    };

            animatableElement.SetValue(property, toValue);

            if (completedAction != null)
            {
                Task.Delay(animation.Duration).ContinueWith(_ => completedAction());
            }
        }*/

        // 구현해야함. 10-18-23 by seoyhaein
        /* public static void StartAnimation(this UIElement animatableElement, DependencyProperty dependencyProperty, Point toValue, double animationDurationSeconds, EventHandler? completedEvent = null)
         {
             var fromValue = (Point)animatableElement.GetValue(dependencyProperty);

             PointAnimation animation = new PointAnimation
             {
                 From = fromValue,
                 To = toValue,
                 Duration = TimeSpan.FromSeconds(animationDurationSeconds)
             };

             animation.Completed += delegate (object? sender, EventArgs e)
             {
                 animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));
                 CancelAnimation(animatableElement, dependencyProperty);

                 completedEvent?.Invoke(sender, e);
             };

             animation.Freeze();

             animatableElement.BeginAnimation(dependencyProperty, animation);
         }

         public static void CancelAnimation(this UIElement animatableElement, DependencyProperty dependencyProperty)
             => animatableElement.BeginAnimation(dependencyProperty, null);*/

        #endregion
    }
}

