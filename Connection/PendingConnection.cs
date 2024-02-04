using System.Diagnostics;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System.Reactive.Linq;
using Avalonia.Data;
using System;
using Avalonia.Controls.Primitives;

namespace DAGlynEditor
{
    /*  이후 삭제
     * IsHitTestVisible
    Background"
    "Foreground"
    "Stroke"
    BorderBrush"
    EnablePreview"
    StrokeThickness"
    BorderThickness"
    Visibility"
    StrokeDashArray"
    Padding"
    "EnableSnapping"

     */

    /*
     *내부 Canvas width height 변경 시키지 말고 외부 width, height 변경 시켜 보는 것을 일단 해봄.
     * 
     */
    public class PendingConnection : ContentControl
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

        // 선의 점선 배열 정의.
        public static readonly StyledProperty<AvaloniaList<double>?> StrokeDashArrayProperty =
            Shape.StrokeDashArrayProperty.AddOwner<PendingConnection>();

        // 선의 색상을 정의합니다.
        public static readonly StyledProperty<IBrush?> StrokeProperty =
            Shape.StrokeProperty.AddOwner<PendingConnection>();

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
        // TODO 확인한다.
        public static readonly StyledProperty<ConnectionDirection> DirectionProperty =
            Connection.DirectionProperty.AddOwner<PendingConnection>();

        // 연결의 가시성 정의.
        // TODO 확인한다. 바인드 모드 일단 주석처리함.
        // 일단 생각해봐야 한다. Content 는 visible 을 false 하고 connection 
        /*public new static readonly StyledProperty<bool> IsVisibleProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(IsVisible), false,false, BindingMode.TwoWay);*/

        // TODO 확인하기
        /*private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var connection = (PendingConnection)d;
            connection.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
        }*/

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

        public AvaloniaList<double>? StrokeDashArray
        {
            get => GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }

        public IBrush? Stroke
        {
            get => GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        /*public new bool IsVisible
        {
            get => base.IsVisible;
            set => SetValue(IsVisibleProperty, value);
        }*/

        public ConnectionDirection Direction
        {
            get => GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        #endregion

        // 해당 인스턴스는 여러 생길 수도 있고 삭제될 수도 있다. 그런데 static 으로 하면 app 이 살아 있을때까지 있을 수 있기때문에
        // 인스턴스가 사라져도 남아 있게 된다.
        /*static PendingConnection()
        {
            // TODO 어떤 것이 더 나은지 살펴보자.
            // GetPropertyChangedObservable(AllowOnlyConnectorsProperty).Subscribe(OnAllowOnlyConnectorsChanged);
            AllowOnlyConnectorsProperty.Changed.Subscribe(OnAllowOnlyConnectorsChanged);
             
        }*/

        #region Fields

        private Control? _canvas;
        private Connection? _connection;

        #endregion

        public PendingConnection()
        {
            // TODO 어떤 것이 더 나은지 살펴보자.
            // GetPropertyChangedObservable(AllowOnlyConnectorsProperty).Subscribe(OnAllowOnlyConnectorsChanged);
            AllowOnlyConnectorsProperty.Changed.Subscribe(OnAllowOnlyConnectorsChanged);
            // XAML 설정에 좀 코드가 길어져서 어기에 설정하였음.
            SetValue(StrokeDashArrayProperty, new AvaloniaList<double> { 4, 4 });
        }

        private static void OnAllowOnlyConnectorsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // 속성이 변경될 때 실행할 코드
            Debug.WriteLine("Hello world");
        }

        // TODO throw 구문 추후 확인. 다른 방법 확인해보자.
        // 버그 있음. Canvas 크기 설정 문제.
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            _canvas = e.NameScope.Find<Canvas>("PART_Canvas");
            if (_canvas == null)
                throw new InvalidOperationException("Template is missing the required 'PART_Canvas' element.");

            _connection = e.NameScope.Find<Connection>("PART_Connection");
            if (_connection == null)
                throw new InvalidOperationException("Template is missing the required 'PART_Connection' element.");

            // TODO 일단 이렇게 처리했는데, 추후 코드를 수정할 필요도 있어 보인다.
            //if (double.IsNaN(_canvas.Width) || double.IsNaN(_canvas.Height))
            //{
            //InvalidateVisual();
            //_canvas.Width = _connection.Target.X - _connection.Source.X + 20;
            //_canvas.Height = _connection.Target.Y - _connection.Source.Y + 20;
            //}

            base.OnApplyTemplate(e);
        }
        // TODO 코드 생각해보기.
        // https://docs.avaloniaui.net/docs/guides/custom-controls/defining-properties
        // 컨트롤의 렌더링 로직
        /*public override void Render(DrawingContext context)
        {
            base.Render(context);
            // 여기에 렌더링 로직 구현
            if (_canvas == null || _connection == null) return;
            
            _canvas.Width = _connection.Target.X - _connection.Source.X + 20;
            _canvas.Height = _connection.Target.Y - _connection.Source.Y + 20;
            
        }*/

        public void ShowOrHide(bool show)
        {
            IsVisible = show;
        }
    }
}