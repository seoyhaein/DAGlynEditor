using Avalonia.Input;
using static DAGlynEditor.SelectionHelper;

namespace DAGlynEditor;

public class EditorSelectingState : EditorState
{
    // nullable 로 수정함
    private readonly SelectionType? _type;
    private bool _canceled;

    /// <summary>The selection helper.</summary>
    protected SelectionHelper Selection { get; }
    
    public EditorSelectingState(DAGlynEditor editor, SelectionType? type) : base(editor)
    {
        Selection = new SelectionHelper(editor);
        _type = type;
    }
    
    /// <inheritdoc />
    public override void Enter(EditorState? from)
    {
        _canceled = false;
        // 일단 주석처리
        //Selection.Start(Editor.MouseLocation, _type);
    }

    /// <inheritdoc />
    public override void Exit()
    {
       // 일단 주석처리
        /* if (_canceled)
        {
            Selection.Abort();
        }
        else
        {
            Selection.End();
        }*/
    }

    /// <inheritdoc />

    // 주석처리
    /* public override void HandlePointerMoved(PointerEventArgs e)
         => Selection.Update(Editor.MouseLocation);*/

    public override void HandlePointerMoved(PointerEventArgs e) 
    {
        return;
    }

    /// <inheritdoc />
    public override void HandlePointerPressed(PointerPressedEventArgs e)
    {
        if (e.Source == null) return;

        if (!Editor.DisablePanning && EditorGestures.PanningStarted.Matches(e.Source, e))
        {
            PushState(new EditorPanningState(Editor));
        }
    }
    // TODO 여기서는 Cancel 을 다시 정의해야함. 일단 true 로 세팅해둠.
    /// <inheritdoc />
    public override void HandlePointerReleased(PointerReleasedEventArgs e)
    {
        if (e.Source == null) return;
        //bool canCancel = EditorGestures.Selection.Cancel.Matches(e);
        bool canCancel = true;
        bool canComplete = EditorGestures.Select.Matches(e.Source, e);
        if (canCancel || canComplete)
        {
            _canceled = !canComplete && canCancel;
            PopState();
        }
    }
    // TODO AutoPanning 을 사용하지 않을 예정임. 일단 체크하기 위해서.
    /// <inheritdoc />
    /*public override void HandleAutoPanning(MouseEventArgs e)
        => HandleMouseMove(e);*/

    public override void HandleKeyUp(KeyEventArgs e)
    {
        //if (e.Source == null) return;
        if (EditorGestures.Selection.Cancel.Matches(e))
        {
            _canceled = true;
            PopState();
        }
    }
}