using Avalonia;
using Avalonia.Input;

namespace DAGlynEditor;

public class EditorPanningState : EditorState
{
    private Point _initialMousePosition;
    private Point _previousMousePosition;
    private Point _currentMousePosition;

    public EditorPanningState(DAGlynEditor editor) : base(editor)
    {
    }

    /// <inheritdoc />
    public override void Exit()
        => Editor.IsPanning = false;

    /// <inheritdoc />
    public override void Enter(EditorState? from)
    {
        if (from == null) return;
        // TODO check 확인할 것. MouseLocation
        _initialMousePosition = Editor.MouseLocation;
        _previousMousePosition = _initialMousePosition;
        _currentMousePosition = _initialMousePosition;
        Editor.IsPanning = true;
    }

    /// <inheritdoc />
    public override void HandlePointerMoved(PointerEventArgs e)
    {
        _currentMousePosition = e.GetPosition(Editor);
        Editor.ViewportLocation -= (_currentMousePosition - _previousMousePosition) / 1;
        // TODO 확인할 것.
        //Editor.ViewportLocation -= (_currentMousePosition - _previousMousePosition) / Editor.ViewportZoom;
        _previousMousePosition = _currentMousePosition;
    }

    /// <inheritdoc />
    public override void HandlePointerReleased(PointerReleasedEventArgs e)
    {
        if (e.Source == null) return;
        if (EditorGestures.PanningEnded.Matches(e.Source, e))
        {
            double contextMenuTreshold = DAGlynEditor.HandleRightClickAfterPanningThreshold * DAGlynEditor.HandleRightClickAfterPanningThreshold;
            if (((Vector)(_currentMousePosition - _initialMousePosition)).SquaredLength > contextMenuTreshold)
            {
                e.Handled = true;
            }

            PopState();
        }
        else if (EditorGestures.Select.Matches(e.Source, e) && Editor.IsSelecting)
        {
            PopState();
            // Cancel selection and continue panning
            if (Editor.State is EditorSelectingState && !Editor.DisablePanning)
            {
                PopState();
                PushState(new EditorPanningState(Editor));
            }
        }
    }
}