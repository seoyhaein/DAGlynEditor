using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.Controls.Templates;

namespace DAGlynEditor
{
    public class DAGlynEditor : SelectingItemsControl
    {
        // https://github.com/AvaloniaUI/Avalonia/issues/9497
        private static readonly FuncTemplate<Panel?> DefaultPanel =
            new(() => new DAGlynEditorCanvas());

        static DAGlynEditor()
        {
            ItemsPanelProperty.OverrideDefaultValue<DAGlynEditor>(DefaultPanel);
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
        
    }
}
