using System;
using Avalonia;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace DAGlynEditor
{
    public static class Extensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> action)
        {
            return observable.Subscribe(new AnonymousObserver<T>(action));
        }
    }
}

