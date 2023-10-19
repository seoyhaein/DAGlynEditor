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

namespace DAGlynEditor
{
    public class DAGlynEditor : SelectingItemsControl
    {
        private static DAGlynEditorCanvas _defaultPanel = new DAGlynEditorCanvas();
        
        private static readonly FuncTemplate<Panel?> DefaultPanel =
            new(() => _defaultPanel);
        
        public static readonly StyledProperty<Point> ViewPortProperty =
            AvaloniaProperty.Register<DAGlynEditor, Point>(nameof(ViewPort), default);
    
        public Point ViewPort
        {
            get => GetValue(ViewPortProperty);
            set => SetValue(ViewPortProperty, value);
        }
        
        static DAGlynEditor()
        {
            ItemsPanelProperty.OverrideDefaultValue<DAGlynEditor>(DefaultPanel);
        }
        
        // 일단 테스트로 넣어둠.
        public DAGlynEditor() 
        {
            // _defaultPanel의 ViewPort를 DAGlynEditor의 ViewPortProperty에 바인딩합니다.
            /*var binding = new Binding("ViewPort")
            {
                Source = this,
                Mode = BindingMode.OneWayToSource
            };*/
            // https://docs.avaloniaui.net/docs/next/guides/data-binding/binding-from-code#using-xaml-bindings-from-code
            //BindingOperations.SetBinding(_defaultPanel, DAGlynEditorCanvas.ViewPortProperty, binding);

            StartDelay();
        }

        // ui 설정 일단 주석처리
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
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
            _defaultPanel.ViewPort = new Point(100, 100);
        }
    }
}
