using System;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace DAGlynEditor
{
    public static class EditorGestures
    {
        // Avalonia에서는 MouseAction 대신 PointerUpdateKind를 사용할 수 있습니다.
        // 하지만, 이 예제에서는 이전 코드와 동일하게 MouseAction을 사용하였습니다.

        // Selection 클래스는 Avalonia에 적합하게 수정되어야 합니다.
        public static class Selection
        {
            public static PointerUpdateKind DefaultPointerUpdateKind { get; set; } = PointerUpdateKind.LeftButtonPressed;

            // 이 부분은 Avalonia의 구현에 맞게 수정해야 합니다.
            // Avalonia에서는 MouseGesture 대신 KeyGesture 또는 다른 입력 처리 방법을 사용할 수 있습니다.
            // 예를 들어, 포인터 이벤트를 처리하는 람다나 메소드를 사용할 수 있습니다.

            public static KeyGesture Replace { get; set; } = new KeyGesture(Key.None); // 수정 필요
            public static KeyGesture Remove { get; set; } = new KeyGesture(Key.None, KeyModifiers.Alt); // 수정 필요
            public static KeyGesture Append { get; set; } = new KeyGesture(Key.None, KeyModifiers.Shift); // 수정 필요
            public static KeyGesture Invert { get; set; } = new KeyGesture(Key.None, KeyModifiers.Control); // 수정 필요
            public static KeyGesture Cancel { get; set; } = new KeyGesture(Key.Escape);
        }

        // 나머지 클래스들도 Avalonia의 입력 시스템에 맞추어 수정해야 합니다.
        // Avalonia에서는 MultiKeyGesture나 이와 유사한 것을 직접 구현해야 할 수도 있습니다.
        // ...

        // 예시로, 다음과 같이 단일 KeyGesture만 지정합니다.
        public static KeyGesture Select { get; } = new KeyGesture(Key.None); // 수정 필요
        public static KeyGesture Pan { get; set; } = new KeyGesture(Key.None); // 수정 필요
        public static KeyModifiers Zoom { get; set; } = KeyModifiers.None;
        public static KeyGesture ZoomIn { get; set; } = new KeyGesture(Key.None); // 수정 필요
        public static KeyGesture ZoomOut { get; set; } = new KeyGesture(Key.None); // 수정 필요
        public static KeyGesture ResetViewportLocation { get; set; } = new KeyGesture(Key.Home);
        public static KeyGesture FitToScreen { get; set; } = new KeyGesture(Key.Home, KeyModifiers.Shift);
        // ...

        // GroupingNode 클래스와 다른 클래스들도 수정 필요
        // ...
    }
}

