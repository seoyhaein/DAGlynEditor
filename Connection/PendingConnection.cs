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
        public static readonly StyledProperty<ConnectionDirection> DirectionProperty =
            Connection.DirectionProperty.AddOwner<PendingConnection>();

        // Fill �� Stroke �� ���� ����
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
            // TODO � ���� �� ������ ���캸��.
            // GetPropertyChangedObservable(AllowOnlyConnectorsProperty).Subscribe(OnAllowOnlyConnectorsChanged);
            AllowOnlyConnectorsProperty.Changed.Subscribe(OnAllowOnlyConnectorsChanged);

            _fillAndStrokeSubscription = SetFillAndStrokeProperty.Changed.Subscribe(value =>
            {
                if (value.Sender is Connection connection)
                {
                    var brush = value.GetNewValue<IBrush?>(); // value.NewValue ��� GetNewValue<IBrush?>() ���
                    connection.Fill = brush;
                    connection.Stroke = brush;
                }
            });

            // TODO axaml ���� ������ ��� Dispose �� �� ���µ� �̷��� �ϸ� �ɱ�?
            this.Unloaded += (sender, e) => this.Dispose();
        }

        private static void OnAllowOnlyConnectorsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // �Ӽ��� ����� �� ������ �ڵ�
            Debug.WriteLine("Hello world");
        }

        // TODO axaml ���� ���� Dispose �ϴ� ����� ���ؼ� �����غ���.
        public void Dispose()
        {
            _fillAndStrokeSubscription?.Dispose();
        }
    }
}