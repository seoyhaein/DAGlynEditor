using Avalonia;
using Avalonia.Input;
using System;
using System.Diagnostics;

namespace DAGlynEditor
{
    public class InputConnector : Connector
    {
        protected override Type StyleKeyOverride => typeof(Connector);

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            Debug.Print("InputConnector Pointer Pressed");
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            Debug.Print("InputConnector Pointer Released");
            base.OnPointerReleased(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            Debug.Print("InputConnector Pointer Moved");
            base.OnPointerMoved(e);
        }

        protected override void PendingConnectionStartedRaiseEvent()
        {
            var args = new PendingConnectionEventArgs(PendingConnectionStartedEvent, this, DataContext)
            {
                Anchor = Anchor,
                // 여기에서 다른 필드 또는 프로퍼티를 설정할 수 있습니다.
            };

            RaiseEvent(args);
        }

        protected override void PendingConnectionDragRaiseEvent(Vector? offset)
        {
            if (offset == null) return;

            var args = new PendingConnectionEventArgs(PendingConnectionDragEvent, this, DataContext)
            {
                OffsetX = offset.Value.X,
                OffsetY = offset.Value.Y,
            };

            RaiseEvent(args);
        }

        protected override void PendingConnectionCompletedRaiseEvent()
        {
            // PendingConnectionEventArgs(DataContext) 관련해서 살펴봐야 함.
            var args = new PendingConnectionEventArgs(PendingConnectionCompletedEvent, this, DataContext)
            {
                //Anchor = Anchor,
            };
            RaiseEvent(args);
        }
    }
}
