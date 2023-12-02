using Avalonia.Input;

namespace DAGlynEditor
{
    public static class EditorGestures
    {
       // TODO: 중요. 향후 수정할 계획임. Any와 All 을 지우고 Any 로만 하는 방향으로 수정할 예정임.

        public static MultiGesture Select { get; } = new MultiGesture(MultiGesture.Match.Any, Selection.Append,
            Selection.Invert, Selection.Replace, Selection.Remove);

        public static PointerGesture PanningStarted { get; set; } =
               new PointerGesture(PointerUpdateKind.RightButtonPressed);

        public static PointerGesture PanningEnded { get; set; } =
                new PointerGesture(PointerUpdateKind.RightButtonReleased);

        /// <summary>The key modifier required to start zooming by mouse wheel.</summary>
        /// <remarks>Defaults to <see cref="ModifierKeys.None"/>.</remarks>
        public static KeyModifiers Zoom { get; set; } = KeyModifiers.None;

        public static class Selection
        {
            public static PointerGesture Replace { get; set; } =
                new PointerGesture(PointerUpdateKind.LeftButtonPressed);

            public static PointerGesture Remove { get; set; } =
                new PointerGesture(PointerUpdateKind.LeftButtonPressed, KeyModifiers.Alt);

            /// <summary>새로 선택된 항목을 이전 선택에 추가하는 제스처.</summary>
            /// <remarks>기본 설정은 <see cref="KeyModifiers.Shift"/> + <see cref="PointerUpdateKind.LeftButtonPressed"/>.</remarks>
            public static PointerGesture Append { get; set; } =
                new PointerGesture(PointerUpdateKind.LeftButtonPressed, KeyModifiers.Shift);

            /// <summary>선택된 항목을 반전하는 제스처.</summary>
            /// <remarks>기본 설정은 <see cref="KeyModifiers.Control"/> + <see cref="PointerUpdateKind.LeftButtonPressed"/>.</remarks>
            public static PointerGesture Invert { get; set; } =
                new PointerGesture(PointerUpdateKind.LeftButtonPressed, KeyModifiers.Control);

            /// <summary>현재 선택 작업을 취소하고 이전 선택으로 되돌리는 제스처.</summary>
            /// <remarks>기본 설정은 <see cref="Key.Escape"/>.</remarks>
            public static KeyGesture Cancel { get; set; } = new KeyGesture(Key.Escape);
        }


        /// <summary>Gesture used to zoom in.</summary>
        /// <remarks>Defaults to <see cref="ModifierKeys.Control"/>+<see cref="Key.OemPlus"/>.</remarks>
        public static MultiGesture ZoomIn { get; set; } = new MultiGesture(MultiGesture.Match.Any,
            new KeyGesture(Key.OemPlus, KeyModifiers.Control), new KeyGesture(Key.Add, KeyModifiers.Control));

        /// <summary>Gesture used to zoom out.</summary>
        /// <remarks>Defaults to <see cref="ModifierKeys.Control"/>+<see cref="Key.OemMinus"/>.</remarks>
        public static MultiGesture ZoomOut { get; set; } = new MultiGesture(MultiGesture.Match.Any,
            new KeyGesture(Key.OemMinus, KeyModifiers.Control), new KeyGesture(Key.Subtract, KeyModifiers.Control));

        /// <summary>Gesture used to move the editor's viewport location to (0, 0).</summary>
        /// <remarks>Defaults to <see cref="Key.Home"/>.</remarks>
        public static KeyGesture ResetViewportLocation { get; set; } = new KeyGesture(Key.Home);

        /// <summary>Gesture used to fit as many containers as possible into the viewport.</summary>
        /// <remarks>Defaults to <see cref="ModifierKeys.Shift"/>+<see cref="Key.Home"/>.</remarks>
        public static KeyGesture FitToScreen { get; set; } = new KeyGesture(Key.Home, KeyModifiers.Shift);

        /// <summary>Gestures used by the <see cref="BaseConnection"/>.</summary>
        public static class Connection
        {
            /// <summary>Gesture to call the <see cref="BaseConnection.SplitCommand"/> command.</summary>
            /// <remarks>Defaults to <see cref="MouseAction.LeftDoubleClick"/>.</remarks>
            // 여기서 부터. 생성자를 만들고 있는중.
            public static PointerGesture Split { get; set; } =
                new PointerGesture(PointerUpdateKind.LeftButtonPressed, 2);

            /// <summary>Gesture to call the <see cref="BaseConnection.DisconnectCommand"/> command.</summary>
            /// <remarks>Defaults to <see cref="ModifierKeys.Alt"/>+<see cref="MouseAction.LeftClick"/>.</remarks>
            public static PointerGesture Disconnect { get; set; } =
                new PointerGesture(PointerUpdateKind.LeftButtonPressed, KeyModifiers.Alt);
        }

        /// <summary>Gestures used by the <see cref="Connector"/>.</summary>
        public static class Connector
        {
            /// <summary>Gesture to call the <see cref="Nodify.Connector.DisconnectCommand"/>.</summary>
            /// <remarks>Defaults to <see cref="ModifierKeys.Alt"/>+<see cref="MouseAction.LeftClick"/>.</remarks>
            public static PointerGesture Disconnect { get; set; } =
                new PointerGesture(PointerUpdateKind.LeftButtonPressed, KeyModifiers.Alt);

            /// <summary>Gesture to start and complete a pending connection.</summary>
            /// <remarks>Defaults to <see cref="MouseAction.LeftClick"/>.</remarks>
            public static PointerGesture Connect { get; set; } =
                new PointerGesture(PointerUpdateKind.LeftButtonPressed);

            /// <summary>Gesture to cancel the pending connection.</summary>
            /// <remarks>Defaults to <see cref="MouseAction.RightClick"/> or <see cref="Key.Escape"/>.</remarks>
            public static MultiGesture CancelAction { get; set; } = new MultiGesture(MultiGesture.Match.Any,
                new PointerGesture(PointerUpdateKind.RightButtonPressed), new KeyGesture(Key.Escape));
        }

        /// <summary>Gestures used by the <see cref="ItemContainer"/>.</summary>
        public static class ItemContainer
        {
            /// <summary>Gesture to select the container using a <see cref="Selection"/> strategy.</summary>
            /// <remarks>Defaults to <see cref="MouseAction.RightClick"/> or any of the <see cref="Selection"/> gestures.</remarks>
            public static MultiGesture Select { get; set; } = new MultiGesture(MultiGesture.Match.Any,
                Selection.Replace,
                Selection.Remove, Selection.Append, Selection.Invert,
                new PointerGesture(PointerUpdateKind.RightButtonPressed));

            /// <summary>Gesture to start and complete a dragging operation.</summary>
            /// <remarks>Using a <see cref="Selection"/> strategy to drag from a new selection. 
            /// <br /> Defaults to any of the <see cref="Selection"/> gestures.
            /// </remarks>
            public static MultiGesture Drag { get; set; } = new MultiGesture(MultiGesture.Match.Any, Selection.Replace,
                Selection.Remove, Selection.Append, Selection.Invert);

            /// <summary>Gesture to cancel the dragging operation.</summary>
            /// <remarks>Defaults to <see cref="MouseAction.RightClick"/> or <see cref="Key.Escape"/>.</remarks>
            public static MultiGesture CancelAction { get; set; } = new MultiGesture(MultiGesture.Match.Any,
                new PointerGesture(PointerUpdateKind.RightButtonPressed), new KeyGesture(Key.Escape));
        }

        // 이건 향후 삭제할 예정임.
        /// <summary>Gestures for the <see cref="GroupingNode"/>.</summary>
        public static class GroupingNode
        {
            /// <summary>The key modifier that will toggle between <see cref="GroupingMovementMode"/>s.</summary>
            /// <remarks>The modifier must be allowed by the <see cref="ItemContainer.Drag"/> gesture.
            /// <br /> Defaults to <see cref="ModifierKeys.Shift"/>.
            /// </remarks>
            public static KeyModifiers SwitchMovementMode { get; set; } = KeyModifiers.Shift;
        }
    }
}