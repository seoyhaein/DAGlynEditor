using Avalonia;
using Avalonia.Input;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace DAGlynEditor
{
    /*
     * InConnector 의 경우는 line 이 들어가는 Connector 임, 즉 Target 임.
     * 
     */

    // ILocatable 은 테스트 용도로 넣었음. 테스트 끝난 후 삭제할 예정임. 
    // LocationProperty 삭제 예정

    public sealed class InConnector : Connector, ILocatable
    {
        protected override Type StyleKeyOverride => typeof(Connector);

        #region Dependency Properties

        public static readonly StyledProperty<Point> LocationProperty =
            AvaloniaProperty.Register<BaseNode, Point>(nameof(Location), Constants.ZeroPoint);

        public Point Location
        {
            get => GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }
        #endregion

        static InConnector()
        {
            // TODO 향후 이거 주석처리한다.
            // UI 바꿀때, Background 속성 변경.
            //BackgroundProperty.OverrideDefaultValue<InConnector>(new SolidColorBrush(Color.Parse("#4d4d4d")));
            //FocusableProperty.OverrideDefaultValue<InConnector>(true);
            FillProperty.OverrideDefaultValue<InConnector>(BrushResources.InConnectorDefaultFill);
        }

    }
}
