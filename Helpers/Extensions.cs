using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace DAGlynEditor
{
    public static class Extensions
    {
        // TODO 이 메서드 확인하기.
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> action)
        {
            return observable.Subscribe(new AnonymousObserver<T>(action));
        }

        [Conditional("DEBUG")]
        public static void Log(bool condition, string format, params object[] args)
        {
            if (condition)
            {
                string output = DateTime.Now.ToString("hh:MM:ss") + ": " + string.Format(format, args); //+ Environment.NewLine + Environment.StackTrace;
                //Console.WriteLine(output);
                Debug.WriteLine(output);
            }
        }

        public static T? GetParentControlOfType<T>(this Control child) where T : Control
        {
            var current = child;

            while (current != null)
            {
                if (current is T target)
                {
                    return target;
                }
                current = current.GetVisualParent() as Control;
            }

            return default;
        }

        public static T? GetParentVisualOfType<T>(this Visual child) where T : Visual
        {
            var current = child;

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

        public static T? GetElementUnderMouse<T>(this Visual container, Point pointerPosition) where T : Visual
        {
            foreach (var visual in container.GetVisualDescendants())
            {
                if (visual.Bounds.Contains(pointerPosition) && visual is T foundElement)
                {
                    return foundElement;
                }
            }

            return null;
        }

    }
}

