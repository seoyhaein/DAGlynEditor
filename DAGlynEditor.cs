using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.VisualTree;
using System.Diagnostics;

namespace DAGlynEditor
{
    public class DAGlynEditor : SelectingItemsControl
    {
        // https://docs.avaloniaui.net/docs/next/get-started/wpf/comparison-of-avalonia-with-wpf-and-uwp#binding
        /* private static DAGlynEditorCanvas _defaultPanel = new DAGlynEditorCanvas();

         private static readonly FuncTemplate<Panel?> DefaultPanel =
             new(() => _defaultPanel);*/

        public static readonly StyledProperty<Point> OffsetProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point>(nameof(Offset), default);

        public Point Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        static DAGlynEditor()
        {
            //ItemsPanelProperty.OverrideDefaultValue<DAGlynEditor>(DefaultPanel);
        }

        // 일단 테스트로 넣어둠.
        public DAGlynEditor()
        {
            // https://docs.avaloniaui.net/docs/next/guides/data-binding/binding-from-code#using-xaml-bindings-from-code
            // 인덱서를 사용해서 바인딩 시킴.
            //_defaultPanel[!ViewPortProperty] = this[!ViewPortProperty];
            //this[!ViewPortProperty] = _defaultPanel[!ViewPortProperty];
            //_defaultPanel의 ViewPort를 DAGlynEditor의 ViewPortProperty에 바인딩합니다.
            /*  var binding = new Binding("ViewPort")
              {
                  Source = this,
                  Mode = BindingMode.OneWayToSource
              };
              _defaultPanel.Bind(ViewPortProperty, binding);*/

            //StartDelay();
        }

        // ui 설정 일단 주석처리
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            //_defaultPanel[!ViewPortProperty] = this[!ViewPortProperty];
            //var itemHost = e.NameScope.Find<Controls>("PART_ItemsHost");
        }
        // 일단 컨테이너를 사각형으로 잡음.
        // 나머지 메서드는 추후 구현
        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new MyRect();
        }

        protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
        {
            /*if (container is MyRect myRect && item?.GetType().GetProperty("Location")?.GetValue(item) is Point location)
            {
                myRect.Location = location;
                
            }*/

            // 일단 임시 땜방 코드 테스트를 위해서.
            if (container is MyRect myRect && item is MyRectInfo info)
            {
                myRect.Location = info.Location;
                myRect.Width = info.W;
                myRect.Height = info.H;
                myRect.Fill = info.Br;
            }
        }

        async void StartDelay()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));

            // 여기에 실행하려는 코드를 추가합니다.
            //_defaultPanel.ViewPort = new Point(100, 100);
            //ViewPort = new Point(100, 100);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == OffsetProperty)
            {
                // 여기에서 ViewPort 속성 값의 변화를 확인할 수 있습니다.
                Debug.WriteLine($"ViewPort changed to: {change.NewValue}");
            }
            //base.OnPropertyChanged(change);
        }
    }
}
