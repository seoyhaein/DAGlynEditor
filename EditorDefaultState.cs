using Avalonia.Input;
using static DAGlynEditor.SelectionHelper;

namespace DAGlynEditor
{
    public class EditorDefaultState : EditorState
    {
        public EditorDefaultState(DAGlynEditor editor) : base(editor)
        {
        }

        public override void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (e.Source == null) return;

            // EditorGestures.Select와 매치되는 경우
            if (EditorGestures.Select.Matches(e.Source, e))
            {
                SelectionType? selectionType = GetSelectionType(e);
                var selecting = new EditorSelectingState(Editor, selectionType);
                PushState(selecting);
            }
            // EditorGestures.Pan과 매치되고, Panning이 비활성화되지 않은 경우
            else if (!Editor.DisablePanning && EditorGestures.PanningStarted.Matches(e.Source, e))
            {
                PushState(new EditorPanningState(Editor));
            }
        }

        private static SelectionType? GetSelectionType(PointerEventArgs e)
        {
            if (e.Source == null) return null;

            // 각각의 선택 제스처에 대해 검사
            if (EditorGestures.Selection.Append.Matches(e.Source, e))
                return SelectionType.Append;
            if (EditorGestures.Selection.Invert.Matches(e.Source, e))
                return SelectionType.Invert;
            if (EditorGestures.Selection.Remove.Matches(e.Source, e))
                return SelectionType.Remove;

            // 기본적으로 Replace 타입 반환
            return SelectionType.Replace;
        }
    }
}