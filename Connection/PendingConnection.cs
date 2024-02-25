using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;

namespace DAGlynEditor
{
    public sealed class PendingConnection : ContentControl, IDisposable
    {
        #region Dependency Properties
        // TODO �̸� �����Ѵ�.
        // ������ ������
        public static readonly StyledProperty<Point> SourceAnchorProperty =
            AvaloniaProperty.Register<PendingConnection, Point>(nameof(SourceAnchor));

        // ������ ����
        public static readonly StyledProperty<Point> TargetAnchorProperty =
            AvaloniaProperty.Register<PendingConnection, Point>(nameof(TargetAnchor));

        // ���� ������ Connector
        public static readonly StyledProperty<object?> SourceConnectorProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(SourceConnector));

        // ���� ���� Connector
        public static readonly StyledProperty<object?> TargetConnectorProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(TargetConnector));

        // �̸����� Ȱ��ȭ ���� ����
        public static readonly StyledProperty<bool> EnablePreviewProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnablePreview));

        // �̸����� ��� ��ü ����
        public static readonly StyledProperty<object?> PreviewTargetProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(PreviewTarget));

        // ���� �β� ����
        // https://docs.avaloniaui.net/docs/guides/custom-controls/defining-properties
        public static readonly StyledProperty<double> StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner<PendingConnection>();

        // ������ Ȱ��ȭ ���� ����
        public static readonly StyledProperty<bool> EnableSnappingProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnableSnapping));

        // ���� ���� ����.
        public static readonly StyledProperty<ConnectionDirection> DirectionProperty =
            Connection.DirectionProperty.AddOwner<PendingConnection>();

        // Fill �� Stroke �� ���� ����
        public static readonly StyledProperty<IBrush?> SetFillAndStrokeProperty =
            AvaloniaProperty.Register<PendingConnection, IBrush?>(nameof(SetFillAndStroke), defaultValue: null);

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
        // TODO �����ϱ� readonly �� �ʿ��ұ�?
        private readonly IDisposable _disposable;

        #endregion

        #region Constructors

        public PendingConnection()
        {
            _disposable = SetFillAndStrokeProperty.Changed.Subscribe(SetFillAndStrokePropertyChanged);
            // TODO axaml ���� ���� Dispose �ϴ� ����� ���ؼ� �����غ���.
            this.Unloaded += (_, _) => this.Dispose();
        }

        #endregion

        #region Methods

        private void SetFillAndStrokePropertyChanged(AvaloniaPropertyChangedEventArgs value)
        {
            if (value.Sender is Connection connection)
            {
                var brush = value.GetNewValue<IBrush?>(); // value.NewValue ��� GetNewValue<IBrush?>() ���
                connection.Fill = brush;
                connection.Stroke = brush;
            }
        }

        public void Dispose()
        {
            // �����Ǵ� �ڿ� ����
            _disposable.Dispose();
        }

        #endregion
    }
}