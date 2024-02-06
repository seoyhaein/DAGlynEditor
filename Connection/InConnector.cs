using Avalonia;
using Avalonia.Input;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Interactivity;

namespace DAGlynEditor
{
    public class InConnector : Connector
    {
        protected override Type StyleKeyOverride => typeof(Connector);

        private void HandleStarted(object? sender, PendingConnectionEventArgs e)
        {
            // sender 따라 처리   
        }

        private void HandleDrag(object? sender, PendingConnectionEventArgs e)
        {
        }

        private void HandleCompleted(object? sender, PendingConnectionEventArgs e)
        {
        }
    }
}
