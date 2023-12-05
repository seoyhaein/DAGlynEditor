using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.Templates;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Input;
using Avalonia.Layout;

namespace DAGlynEditor
{
    // 일단 바로 Node 를 집어넣기 위해서 ICanvasItem 를 상속받는 것으로 수정함. ItemContainer 는 이것이 필요 없을 것 같기도 하다.이건 생각해보자.
    public class Node : TemplatedControl, ICanvasItem
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        #region Constructors

        public Node() { }

        public Node(Point? location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location), "Location cannot be null.");
            
            this.Location = (Point)location; 
        }

        // TODO Reactive 방식 추후 적용할지 고민해보자.
        /*static Node()
        {
            //FooterProperty.Changed.Subscribe(OnFooterChanged);
        }*/

        #endregion
        
        private void InitializeSubscriptions()
        {
            Observable.FromEventPattern<PointerPressedEventArgs>(
                    h => this.PointerPressed += h,
                    h => this.PointerPressed -= h)
                .Subscribe(args => HandlePointerPressed(args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PointerEventArgs>(
                    h => this.PointerMoved += h,
                    h => this.PointerMoved -= h)
                .Subscribe(args => HandlePointerMoved(args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PointerReleasedEventArgs>(
                    h => this.PointerReleased += h,
                    h => this.PointerReleased -= h)
                .Subscribe(args => HandlePointerReleased(args.EventArgs))
                .DisposeWith(_disposables);
        }
        
        private void HandlePointerPressed(PointerPressedEventArgs args)
        {
            Debug.WriteLine("pressed called");
        }

        private void HandlePointerMoved(PointerEventArgs args)
        {
            Debug.WriteLine("moved called");
        }

        private void HandlePointerReleased(PointerReleasedEventArgs args)
        {
            Debug.WriteLine("released called");
        }

        #region Dependency Properties
        
        public static readonly StyledProperty<Point> LocationProperty =
            AvaloniaProperty.Register<ItemContainer, Point>(nameof(Location));

        public Point Location
        {
            get => GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }
        
        /*
            protected internal static readonly DependencyPropertyKey HasFooterPropertyKey = DependencyProperty.RegisterReadOnly(nameof(HasFooter), typeof(bool), typeof(Node), new FrameworkPropertyMetadata(BoxValue.False));
            public static readonly DependencyProperty HasFooterProperty = HasFooterPropertyKey.DependencyProperty;
         */
        // Brush 를 사용할 때 Media 가 적절한지는 좀더 살펴보자.
        public static readonly StyledProperty<IBrush?> ContentBrushProperty =
                AvaloniaProperty.Register<Node, IBrush?>(nameof(ContentBrush));

        public IBrush? ContentBrush
        {
            get => GetValue(ContentBrushProperty);
            set => SetValue(ContentBrushProperty, value);
        }

        public static readonly StyledProperty<IBrush?> HeaderBrushProperty =
                AvaloniaProperty.Register<Node, IBrush?>(nameof(HeaderBrush));

        public IBrush? HeaderBrush
        {
            get => GetValue(HeaderBrushProperty);
            set => SetValue(HeaderBrushProperty, value);
        }

        public static readonly StyledProperty<IBrush?> FooterBrushProperty =
                AvaloniaProperty.Register<Node, IBrush?>(nameof(FooterBrush));

        public IBrush? FooterBrush
        {
            get => GetValue(FooterBrushProperty);
            set => SetValue(FooterBrushProperty, value);
        }

        public static readonly StyledProperty<DataTemplate> HeaderTemplateProperty =
                AvaloniaProperty.Register<Node, DataTemplate>(nameof(HeaderTemplate));

        public DataTemplate HeaderTemplate
        {
            get => GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly StyledProperty<object> HeaderProperty =
            AvaloniaProperty.Register<Node, object>(nameof(Header));
        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        // default method 잡아줘야함.
        public static readonly StyledProperty<DataTemplate> FooterTemplateProperty =
            AvaloniaProperty.Register<Node, DataTemplate>(nameof(FooterTemplate));

        public DataTemplate FooterTemplate
        {
            get => GetValue(FooterTemplateProperty);
            set => SetValue(FooterTemplateProperty, value);
        }

        public static readonly StyledProperty<object> FooterProperty =
            AvaloniaProperty.Register<Node, object>(nameof(Footer));
        public object Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }
        public static readonly StyledProperty<DataTemplate> InputConnectorTemplateProperty =
            AvaloniaProperty.Register<Node, DataTemplate>(nameof(InputConnectorTemplate));
        public DataTemplate InputConnectorTemplate
        {
            get => GetValue(InputConnectorTemplateProperty);
            set => SetValue(InputConnectorTemplateProperty, value);
        }

        public static readonly StyledProperty<DataTemplate> OutputConnectorTemplateProperty =
            AvaloniaProperty.Register<Node, DataTemplate>(nameof(OutputConnectorTemplate));
        public DataTemplate OutputConnectorTemplate
        {
            get => GetValue(InputConnectorTemplateProperty);
            set => SetValue(InputConnectorTemplateProperty, value);
        }
        // TODO 여기에서 테스트 용으로 default 값을 넣었지만, 향후 삭제할 예정임.
        public static readonly StyledProperty<IEnumerable> InputProperty =
            AvaloniaProperty.Register<Node, IEnumerable>(nameof(Input), defaultValue: new List<string> { "DefaultItem" });
        public IEnumerable Input
        {
            get => GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }
        // TODO 여기에서 테스트 용으로 default 값을 넣었지만, 향후 삭제할 예정임.
        public static readonly StyledProperty<IEnumerable> OutputProperty =
            AvaloniaProperty.Register<Node, IEnumerable>(nameof(Output), defaultValue: new List<string> { "DefaultItem" });
        public IEnumerable Output
        {
            get => GetValue(OutputProperty);
            set => SetValue(OutputProperty, value);
        }

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            AvaloniaProperty.Register<Node, VerticalAlignment>(nameof(VerticalContentAlignment), defaultValue: VerticalAlignment.Stretch);

        public VerticalAlignment VerticalContentAlignment
        {
            get => GetValue(VerticalContentAlignmentProperty);
            set => SetValue(VerticalContentAlignmentProperty, value);
        }

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            AvaloniaProperty.Register<Node, HorizontalAlignment>(nameof(HorizontalContentAlignment), defaultValue: HorizontalAlignment.Stretch);

        public HorizontalAlignment HorizontalContentAlignment
        {
            get => GetValue(HorizontalContentAlignmentProperty);
            set => SetValue(HorizontalContentAlignmentProperty, value);
        }

        #endregion

        #region Methods

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            switch (change.Property.Name)
            {
                case nameof(HeaderProperty):
                case nameof(FooterProperty):
                    break;
            }
        }

        #endregion
        
    }
}
