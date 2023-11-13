using Avalonia;
using Avalonia.Input;

namespace DAGlynEditor
{
    public class PointerGesture
    {
        private readonly KeyModifiers _keyModifiers;
        private readonly PointerUpdateKind _pointerUpdateKind;
        private readonly int _counter = 1; // 기본값으로 1을 설정

        public PointerGesture(PointerUpdateKind kind)
        {
            _pointerUpdateKind = kind;
            _keyModifiers = KeyModifiers.None;
        }

        public PointerGesture(PointerUpdateKind kind, int counter) : this(kind)
        {
            _counter = counter;
        }

        public PointerGesture(PointerUpdateKind kind, KeyModifiers modifiers) : this(kind)
        {
            _keyModifiers = modifiers;
        }

        public PointerGesture(PointerUpdateKind kind, KeyModifiers modifiers, int counter) : this(kind, modifiers)
        {
            _counter = counter;
        }

        public bool Matches(object source, PointerEventArgs eventArgs)
        {
            if (!(source is Visual targetElement))
                return false;

            var currentPoint = eventArgs.GetCurrentPoint(targetElement).Properties;
            bool modifiersMatch = (eventArgs.KeyModifiers & _keyModifiers) == _keyModifiers;
            bool pointerUpdateKindMatch = currentPoint.PointerUpdateKind == _pointerUpdateKind;

            bool counterMatch = true;
            if (eventArgs is PointerPressedEventArgs pressedEventArgs) 
                if (pressedEventArgs.ClickCount != _counter)
                    counterMatch = false;
            
            return modifiersMatch && pointerUpdateKindMatch && counterMatch;
        }
    }
}
