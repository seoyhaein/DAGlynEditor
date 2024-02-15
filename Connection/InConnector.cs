using Avalonia;
using Avalonia.Input;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace DAGlynEditor
{
    /*
 * InConnector 의 경우는 line 이 들어가는 Connector 임, 즉 Target 임.
 * 
 */

    public sealed class InConnector : Connector
    {
        protected override Type StyleKeyOverride => typeof(Connector);

        static InConnector()
        {
            // TODO 향후 이거 주석처리한다.
            // UI 바꿀때, Background 속성 변경.
            //BackgroundProperty.OverrideDefaultValue<InConnector>(new SolidColorBrush(Color.Parse("#4d4d4d")));
            //FocusableProperty.OverrideDefaultValue<InConnector>(true);
            FillProperty.OverrideDefaultValue<OutConnector>(new SolidColorBrush(Color.Parse("#4d4d4d")));
        }

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
