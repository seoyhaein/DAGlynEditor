using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace DAGlynEditor
{
    /*
     * OutConnector 의 경우는 line 이 나가는 Connector 임, 즉 Source 임.
     * 
     */

    /// <summary>
    /// HandlePointerMoved
    /// 지금 Canvas 로 잡았는데, 이부분은 수정해야함.
    /// 공통된 부분 깔끔하게 정리해야함. 
    /// </summary>

    // TODO 코드 정리 필요. 
    public sealed class OutConnector : Connector
    {
        protected override Type StyleKeyOverride => typeof(Connector);

        #region Constructor

        static OutConnector()
        {
            // TODO 향후 이거 주석처리한다.
            // UI 바꿀때, Background 속성 변경. 그냥 Background 를 받아서 할까 아니면 지금처럼 Fill 을 만들어 줄까????
            //BackgroundProperty.OverrideDefaultValue<OutConnector>(new SolidColorBrush(Color.Parse("#4d4d4d")));
            //FocusableProperty.OverrideDefaultValue<OutConnector>(true);
            FillProperty.OverrideDefaultValue<OutConnector>(new SolidColorBrush(Color.Parse("#2e2e2e")));
        }

        #endregion

        #region Fields

        // Connector 밖으로 한번이라도 나가면 true 가 된다.
        private bool _outSideOutConnector = false;

        #endregion

        #region Evnet Handlers

        protected override void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
            if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;

            if (sender == null) return;

            Focus();
            args.Pointer.Capture(this);

            if (this.Equals(args.Pointer.Captured))
            {
                Debug.Print("Pointer Pressed");
                this.PreviousConnector = this;
                this.IsPointerPressed = true; // 마우스 눌림 상태 설정
                args.Handled = true; // 이벤트 전파를 막음.
            }
        }

        protected override void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
            if (sender == null || !this.IsPointerPressed || this.PreviousConnector == null) return;

            // TODO 일단 Canvas 라고 가정한다.
            var parent = this.GetParentVisualOfType<Canvas>();
            if (parent == null) return;
            var currentPosition = args.GetPosition(parent);

            // 마우스 이동중 새로운 Connector 에 들어가면 null 이 아님.
            var elementUnderPointer = parent.GetElementUnderMouse<OutConnector>(currentPosition);

            // 한번이라도 OutConnector 를 나가면.

            if (elementUnderPointer == null && this.IsPointerPressed)
            {
                // 1. 정상적인 Moved
                //TODO 여기서 Caputed 와 비교할 필요가 있을까? 일단 해줌. 아래 조건문을 할 필요가 없을 듯 하다.
                if (this.PreviousConnector.Equals(args.Pointer.Captured) && this.Equals(args.Pointer.Captured))
                {
                    _outSideOutConnector = true;
                    Debug.Print("정상 Moved");
                }
            }

            // 2` 자신으로 돌아온 경우
            // OutConnector 내에서의 이동도 생각해야한다.
            if (elementUnderPointer != null && elementUnderPointer.Equals(this.PreviousConnector) && this.IsPointerPressed)
            {
                if (_outSideOutConnector && this.PreviousConnector.Equals(args.Pointer.Captured))
                    Debug.Print("그냥 외곽에 있다가 다시 들어간 경우");

                if (!_outSideOutConnector && this.PreviousConnector.Equals(args.Pointer.Captured))
                    Debug.Print("OutConnector 내에서의 마우스 이동.");
            }

            // 2`` 다른 Connector로 들어온 경우
            if (elementUnderPointer != null && !elementUnderPointer.Equals(this.PreviousConnector) && this.IsPointerPressed)
            {
                //if (this.PreviousConnector.Equals(args.Pointer.Captured))
                this.PreviousConnector = elementUnderPointer;
                Debug.Print("다른 Connector로 들어온 경우");
            }

            // 2``` 다른 Connector 에 들어 갔다가 다시 자신으로 돌아온 경우
            if (elementUnderPointer != null && elementUnderPointer.Equals(this.PreviousConnector) && this.IsPointerPressed)
            {
                if (!this.PreviousConnector.Equals(args.Pointer.Captured))
                    Debug.Print("다른 Connector 에 들어 갔다가 다시 자신으로 돌아온 경우");
            }

            args.Handled = true;
        }

        protected override void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
            if (sender == null) return;
            // TODO PreviousConnector 검사 필요한지 생각해보자.
            // 두가지를 생각해야 한다. InConnector 에서 Release 되었는지 아닌지.
            if (this.Equals(args.Pointer.Captured) && this.IsPointerPressed)
            {
                // TODO 일단 Canvas 라고 가정한다.
                var parent = this.GetParentVisualOfType<Canvas>();
                if (parent == null) return;
                var currentPosition = args.GetPosition(parent);

                // 마우스 이동중 새로운 Connector 에 들어가면 null 이 아님.
                var elementUnderPointer = parent.GetElementUnderMouse<Connector>(currentPosition);

                if (elementUnderPointer is InConnector okConnector)
                {
                    okConnector.Focus();
                    Debug.Print(" InConnector Pointer Released");
                    this.IsPointerPressed = false; // 마우스 눌림 상태 해제
                    args.Pointer.Capture(null);
                    args.Handled = true;
                    PreviousConnector = null;
                    _outSideOutConnector = false;
                }
                else
                {
                    Debug.Print("Not InConnectorPointer Released");
                    this.IsPointerPressed = false; // 마우스 눌림 상태 해제
                    args.Pointer.Capture(null);
                    args.Handled = true;
                    PreviousConnector = null;
                    _outSideOutConnector = false;
                }
            }
        }

        #endregion

    }
}