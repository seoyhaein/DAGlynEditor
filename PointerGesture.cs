using Avalonia;
using Avalonia.Input;

namespace DAGlynEditor
{
    public class PointerGesture
    {
        private readonly KeyModifiers _keyModifiers;
        private readonly PointerUpdateKind _pointerUpdateKind;

        public PointerGesture(PointerUpdateKind kind, KeyModifiers modifiers)
        {
            _pointerUpdateKind = kind;
            _keyModifiers = modifiers;
        }

        public bool Match(object source, PointerEventArgs eventArgs)
        {
            if (!(source is Visual targetElement))
                return false;

            // 현재 포인트와 이벤트 상태를 변수에 저장하여 가독성을 높이고 중복 호출을 방지합니다.
            var currentPoint = eventArgs.GetCurrentPoint(targetElement).Properties;
            bool modifiersMatch = (eventArgs.KeyModifiers & _keyModifiers) == _keyModifiers;
            bool pointerUpdateKindMatch = currentPoint.PointerUpdateKind == _pointerUpdateKind;

            return modifiersMatch && pointerUpdateKindMatch;
        }
    }
}
