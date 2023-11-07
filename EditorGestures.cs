using System;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace DAGlynEditor
{
    public static class EditorGestures
    {
        // 테스트 용으로 작성하였음. 향후 수정할 계획임.
        public static PointerGesture Tester { get; set; } =
         new PointerGesture(PointerUpdateKind.LeftButtonPressed, (KeyModifiers.Control | KeyModifiers.Alt));
    }
}

