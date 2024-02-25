using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace DAGlynEditor
{
    /*
  * 새롭게 Reload 할 경우 위치에 대한 변경을 해줘야 한다.
  * Location 이 최종적으로 변할때 이것을 기록하고 UI 변경사항을 남겨야 한다.
  * InValid~~ Transform 은 시각적인 위치만 변경시킨다. 다시 reload 할 경우 원래대로 돌아온다.
  */

    /// <summary>
    /// 속성 변경에 따른 메서드로 처리하는게 나을지 고민해야함. DAGlynEditorCanvas 와 비교해봐야 함.
    /// 중복되는 기능들은 통합할 필요 있음.
    /// </summary>

    public class Node : BaseNode
    {
        #region Dependency Properties

        public static readonly StyledProperty<Control?> ParentControlProperty =
            AvaloniaProperty.Register<BaseNode, Control?>(nameof(ParentControl));

        public Control? ParentControl
        {
            get => GetValue(ParentControlProperty);
            set => SetValue(ParentControlProperty, value);
        }
        #endregion

        #region fields

        // Node 의 움직임을 위해
        private TranslateTransform _translateTransform = new TranslateTransform();
        private readonly IDisposable _disposable;

        #endregion

        //TODO Node 삭제되는 것도 신경써야 한다.
        #region Constructor

        public Node()
        {
            // 초기 설정에서 TranslateTransform 객체를 RenderTransform으로 설정
            this.RenderTransform = _translateTransform;
            //TODO 살펴보자. dispose
            _disposable = ParentControlProperty.Changed.Subscribe(HandleParentControlChanged);
        }

        public Node(Point location) : this()
        {
            this.Location = location;
        }

        #endregion

        #region Evnet Handlers

        protected override void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
            if (this.ParentControl == null)
                throw new InvalidOperationException("Node cannot move because a DAGlynEditorCanvas parent is not found.");

            // 불필요한 조건 검사 제거
            if (args.GetCurrentPoint(this).Properties.IsLeftButtonPressed && sender != null)
            {
                Focus();
                args.Pointer.Capture(this);
                Debug.Print("Dragging Start");

                InitialPointerPosition = args.GetPosition(ParentControl);
                PreviousPointerPosition = InitialPointerPosition;
                this.IsDragging = true;
                args.Handled = true;
            }
        }

        protected override void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
            if (this.ParentControl == null)
                throw new InvalidOperationException("Node cannot move because a DAGlynEditorCanvas parent is not found.");

            if (sender == null || !this.IsDragging || !this.Equals(args.Pointer.Captured)) return;

            Debug.Print("Dragging...");
            CurrentPointerPosition = args.GetPosition(ParentControl);
            var delta = CurrentPointerPosition - PreviousPointerPosition;

            // 드래그 임계값 검사
            if (((Vector)delta).SquaredLength > Constants.AppliedThreshold)
            {
                NodeMove(this.Location + delta); // SetLocation 메소드를 통한 위치 업데이트
                PreviousPointerPosition = CurrentPointerPosition;
                args.Handled = true;
            }
        }

        protected override void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
            if (this.ParentControl == null)
                throw new InvalidOperationException("Node cannot move because a DAGlynEditorCanvas parent is not found.");

            if (sender != null && this.Equals(args.Pointer.Captured) && this.IsDragging)
            {
                Debug.Print("Finish");
                args.Pointer.Capture(null);
                this.IsDragging = false;
                args.Handled = true;
            }
        }

        private void HandleParentControlChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is DAGlynEditorCanvas editorCanvas)
            {
                this.ParentControl = editorCanvas;
            }
            else
            {
                var parentControl = this.GetParentVisualOfType<DAGlynEditorCanvas>();
                if (parentControl != null)
                {
                    this.ParentControl = parentControl;
                }
                else
                {
                    this.ParentControl = null;
                }
            }
        }

        #endregion

        #region Methods

        private void NodeMove(Point point)
        {
            Location = point;
            _translateTransform.X = point.X;
            _translateTransform.Y = point.Y;
        }

        #endregion

        /// <inheritdoc />
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            this.ParentControl = this.GetParentVisualOfType<DAGlynEditorCanvas>();
        }

        public bool CanNodeMove()
        {
            var parentControl = this.GetParentVisualOfType<DAGlynEditorCanvas>();
            if (parentControl != null)
            {
                this.ParentControl = parentControl;
                return true;
            }
            else
            {
                this.ParentControl = null;
                return false;
            }
        }

        public void SetLocation(Point location)
        {
            this.Location = location;
        }

        // TODO 살펴보자.
        protected override void Dispose(bool disposing)
        {
            _disposable.Dispose();
            base.Dispose(disposing);
        }
    }
}
