using Avalonia.Input;

namespace DAGlynEditor
{
    // 이 클래스는 사용하지 않을 예정임.
    // 에러나는 부분은 일단 주석 처리함.
    public abstract class EditorState
    {
        public EditorState(DAGlynEditor editor)
        {
            Editor = editor;
        }

        protected DAGlynEditor Editor { get; }

        public virtual void HandlePointerPressed(PointerPressedEventArgs e) { }
        public virtual void HandlePointerMoved(PointerEventArgs e) { }
        public virtual void HandlePointerReleased(PointerReleasedEventArgs e) { }

        // Mouse Wheel 관련. 추후 추가. 자세히 조사 필요.
        /// <inheritdoc cref="NodifyEditor.OnMouseWheel(MouseWheelEventArgs)"/>
        //public virtual void HandleMouseWheel(MouseWheelEventArgs e) { }
        // Avalonia 에서는 PointerWheelEventArgs 가 존재함. 

        // AutoPanning 은 사용하지 않을 것임.
        //public virtual void HandleAutoPanning(PointerEventArgs e) { }

        // Key 관련해서도 자세히 조사 필요
        public virtual void HandleKeyUp(KeyEventArgs e) { }
        public virtual void HandleKeyDown(KeyEventArgs e) { }

        /// <summary>Called when <see cref="NodifyEditor.PushState(EditorState)"/> is called.</summary>
        /// <param name="from">The state we enter from (is null for root state).</param>
        public virtual void Enter(EditorState? from) { }

        /// <summary>Called when <see cref="NodifyEditor.PopState"/> is called.</summary>
        public virtual void Exit() { }

        /// <summary>Called when <see cref="NodifyEditor.PopState"/> is called.</summary>
        /// <param name="from">The state we re-enter from.</param>
        public virtual void ReEnter(EditorState from) { }

        // 아래는 일단 주석처리 함.
        /// <summary>Pushes a new state into the stack.</summary>
        /// <param name="newState">The new state.</param>
        //public virtual void PushState(EditorState newState) => Editor.PushState(newState);

        /// <summary>Pops the current state from the stack.</summary>
        //public virtual void PopState() => Editor.PopState();
    }
}
