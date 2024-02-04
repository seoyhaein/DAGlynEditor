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
    /*  ���� ����
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
     *���� Canvas width height ���� ��Ű�� ���� �ܺ� width, height ���� ���� ���� ���� �ϴ� �غ�.
     * 
     */
    public class PendingConnection : ContentControl
    {
        #region Dependency Properties

        // ������ ������
        public static readonly StyledProperty<Point> SourceAnchorProperty =
            AvaloniaProperty.Register<PendingConnection, Point>(nameof(SourceAnchor), default(Point));

        // ������ ����
        public static readonly StyledProperty<Point> TargetAnchorProperty =
            AvaloniaProperty.Register<PendingConnection, Point>(nameof(TargetAnchor), default(Point));

        // ���� ������ Connector
        public static readonly StyledProperty<object?> SourceConnectorProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(SourceConnector));

        // ���� ���� Connector
        public static readonly StyledProperty<object?> TargetConnectorProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(TargetConnector));

        // �̸����� Ȱ��ȭ ���� ����
        public static readonly StyledProperty<bool> EnablePreviewProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnablePreview), false);

        // �̸����� ��� ��ü ����
        public static readonly StyledProperty<object?> PreviewTargetProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(PreviewTarget));

        // ���� �β� ����
        // https://docs.avaloniaui.net/docs/guides/custom-controls/defining-properties
        public static readonly StyledProperty<double> StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner<PendingConnection>();

        // ���� ���� �迭 ����.
        public static readonly StyledProperty<AvaloniaList<double>?> StrokeDashArrayProperty =
            Shape.StrokeDashArrayProperty.AddOwner<PendingConnection>();

        // ���� ������ �����մϴ�.
        public static readonly StyledProperty<IBrush?> StrokeProperty =
            Shape.StrokeProperty.AddOwner<PendingConnection>();

        // Ŀ���͸� ��� ���� ����.
        // TODO Ȯ���Ѵ�.
        /*public static readonly DependencyProperty AllowOnlyConnectorsProperty =
            DependencyProperty.Register(nameof(AllowOnlyConnectors), typeof(bool), typeof(PendingConnection),
                new FrameworkPropertyMetadata(BoxValue.True, OnAllowOnlyConnectorsChanged));*/

        public static readonly StyledProperty<bool> AllowOnlyConnectorsProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(AllowOnlyConnectors), true);

        // ������ Ȱ��ȭ ���� ����
        public static readonly StyledProperty<bool> EnableSnappingProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnableSnapping), false);

        // ���� ���� ����.
        // TODO Ȯ���Ѵ�.
        public static readonly StyledProperty<ConnectionDirection> DirectionProperty =
            Connection.DirectionProperty.AddOwner<PendingConnection>();

        // ������ ���ü� ����.
        // TODO Ȯ���Ѵ�. ���ε� ��� �ϴ� �ּ�ó����.
        // �ϴ� �����غ��� �Ѵ�. Content �� visible �� false �ϰ� connection 
        /*public new static readonly StyledProperty<bool> IsVisibleProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(IsVisible), false,false, BindingMode.TwoWay);*/

        // TODO Ȯ���ϱ�
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

        // �ش� �ν��Ͻ��� ���� ���� ���� �ְ� ������ ���� �ִ�. �׷��� static ���� �ϸ� app �� ��� ���������� ���� �� �ֱ⶧����
        // �ν��Ͻ��� ������� ���� �ְ� �ȴ�.
        /*static PendingConnection()
        {
            // TODO � ���� �� ������ ���캸��.
            // GetPropertyChangedObservable(AllowOnlyConnectorsProperty).Subscribe(OnAllowOnlyConnectorsChanged);
            AllowOnlyConnectorsProperty.Changed.Subscribe(OnAllowOnlyConnectorsChanged);
             
        }*/

        #region Fields

        private Control? _canvas;
        private Connection? _connection;

        #endregion

        public PendingConnection()
        {
            // TODO � ���� �� ������ ���캸��.
            // GetPropertyChangedObservable(AllowOnlyConnectorsProperty).Subscribe(OnAllowOnlyConnectorsChanged);
            AllowOnlyConnectorsProperty.Changed.Subscribe(OnAllowOnlyConnectorsChanged);
            // XAML ������ �� �ڵ尡 ������� ��⿡ �����Ͽ���.
            SetValue(StrokeDashArrayProperty, new AvaloniaList<double> { 4, 4 });
        }

        private static void OnAllowOnlyConnectorsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // �Ӽ��� ����� �� ������ �ڵ�
            Debug.WriteLine("Hello world");
        }

        // TODO throw ���� ���� Ȯ��. �ٸ� ��� Ȯ���غ���.
        // ���� ����. Canvas ũ�� ���� ����.
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            _canvas = e.NameScope.Find<Canvas>("PART_Canvas");
            if (_canvas == null)
                throw new InvalidOperationException("Template is missing the required 'PART_Canvas' element.");

            _connection = e.NameScope.Find<Connection>("PART_Connection");
            if (_connection == null)
                throw new InvalidOperationException("Template is missing the required 'PART_Connection' element.");

            // TODO �ϴ� �̷��� ó���ߴµ�, ���� �ڵ带 ������ �ʿ䵵 �־� ���δ�.
            //if (double.IsNaN(_canvas.Width) || double.IsNaN(_canvas.Height))
            //{
            //InvalidateVisual();
            //_canvas.Width = _connection.Target.X - _connection.Source.X + 20;
            //_canvas.Height = _connection.Target.Y - _connection.Source.Y + 20;
            //}

            base.OnApplyTemplate(e);
        }
        // TODO �ڵ� �����غ���.
        // https://docs.avaloniaui.net/docs/guides/custom-controls/defining-properties
        // ��Ʈ���� ������ ����
        /*public override void Render(DrawingContext context)
        {
            base.Render(context);
            // ���⿡ ������ ���� ����
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