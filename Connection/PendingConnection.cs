using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;

namespace DAGlynEditor
{
    public class PendingConnection : ContentControl, IDisposable
    {
        #region Dependency Properties

        // 연결의 시작점
        public static readonly StyledProperty<Point> SourceAnchorProperty =
            AvaloniaProperty.Register<PendingConnection, Point>(nameof(SourceAnchor), default(Point));

        // 연결의 끝점
        public static readonly StyledProperty<Point> TargetAnchorProperty =
            AvaloniaProperty.Register<PendingConnection, Point>(nameof(TargetAnchor), default(Point));

        // 연결 시작의 Connector
        public static readonly StyledProperty<object?> SourceConnectorProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(SourceConnector));

        // 연결 끝의 Connector
        public static readonly StyledProperty<object?> TargetConnectorProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(TargetConnector));

        // 미리보기 활성화 여부 정의
        public static readonly StyledProperty<bool> EnablePreviewProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnablePreview), false);

        // 미리보기 대상 객체 정의
        public static readonly StyledProperty<object?> PreviewTargetProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(PreviewTarget));

        // 선의 두꼐 정의
        // https://docs.avaloniaui.net/docs/guides/custom-controls/defining-properties
        public static readonly StyledProperty<double> StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner<PendingConnection>();


        // 커넥터만 허용 여부 정의.
        // TODO 확인한다.
        /*public static readonly DependencyProperty AllowOnlyConnectorsProperty =
            DependencyProperty.Register(nameof(AllowOnlyConnectors), typeof(bool), typeof(PendingConnection),
                new FrameworkPropertyMetadata(BoxValue.True, OnAllowOnlyConnectorsChanged));*/

        public static readonly StyledProperty<bool> AllowOnlyConnectorsProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(AllowOnlyConnectors), true);

        // 스냅핑 활성화 여부 정의
        public static readonly StyledProperty<bool> EnableSnappingProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnableSnapping), false);

        // 연결 방향 정의.
        public static readonly StyledProperty<ConnectionDirection> DirectionProperty =
            Connection.DirectionProperty.AddOwner<PendingConnection>();

        // Fill 과 Stroke 를 동시 설정
        public static readonly StyledProperty<IBrush?> SetFillAndStrokeProperty =
            AvaloniaProperty.Register<PendingConnection, IBrush?>(nameof(SetFillAndStroke), defaultValue: null);

        #endregion

        #region Properties

        public Point SourceAnchor
        {
            get => GetValue(SourceAnchorProperty);
            set => SetValue(SourceAnchorProperty, value);

        }

        public Point TargetAnchor
        {
            get => GetValue(TargetAnchorProperty);
            set => SetValue(TargetAnchorProperty, value);
        }

        public object? SourceConnector
        {
            get => GetValue(SourceConnectorProperty);
            set => SetValue(SourceConnectorProperty, value);
        }

        public object? TargetConnector
        {
            get => GetValue(TargetConnectorProperty);
            set => SetValue(TargetConnectorProperty, value);
        }

        public bool EnablePreview
        {
            get => GetValue(EnablePreviewProperty);
            set => SetValue(EnablePreviewProperty, value);
        }

        public object? PreviewTarget
        {
            get => GetValue(PreviewTargetProperty);
            set => SetValue(PreviewTargetProperty, value);
        }

        public bool EnableSnapping
        {
            get => GetValue(EnableSnappingProperty);
            set => SetValue(EnableSnappingProperty, value);
        }

        public bool AllowOnlyConnectors
        {
            get => GetValue(AllowOnlyConnectorsProperty);
            set => SetValue(AllowOnlyConnectorsProperty, value);
        }

        public double StrokeThickness
        {
            get => GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public ConnectionDirection Direction
        {
            get => GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public IBrush? SetFillAndStroke
        {
            get => GetValue(SetFillAndStrokeProperty);
            set => SetValue(SetFillAndStrokeProperty, value);
        }

        #endregion

        #region Fields

        private IDisposable? _fillAndStrokeSubscription;

        #endregion

        public PendingConnection()
        {
            // TODO 어떤 것이 더 나은지 살펴보자.
            // GetPropertyChangedObservable(AllowOnlyConnectorsProperty).Subscribe(OnAllowOnlyConnectorsChanged);
            AllowOnlyConnectorsProperty.Changed.Subscribe(OnAllowOnlyConnectorsChanged);

            _fillAndStrokeSubscription = SetFillAndStrokeProperty.Changed.Subscribe(value =>
            {
                if (value.Sender is Connection connection)
                {
                    var brush = value.GetNewValue<IBrush?>(); // value.NewValue 대신 GetNewValue<IBrush?>() 사용
                    connection.Fill = brush;
                    connection.Stroke = brush;
                }
            });

            // TODO axaml 에서 생성한 경우 Dispose 할 수 없는데 이렇게 하면 될까?
            this.Unloaded += (sender, e) => this.Dispose();
        }

        private static void OnAllowOnlyConnectorsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // 속성이 변경될 때 실행할 코드
            Debug.WriteLine("Hello world");
        }

        // TODO axaml 에서 사용시 Dispose 하는 방법에 대해서 생각해보기.
        public void Dispose()
        {
            _fillAndStrokeSubscription?.Dispose();
        }
    }
}