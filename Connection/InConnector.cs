using Avalonia;
using Avalonia.Input;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Interactivity;

namespace DAGlynEditor
{
    public class InConnector : Connector
    {
        protected override Type StyleKeyOverride => typeof(Connector);
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public InConnector()
        {
            InitializeSubscriptions();
        }
        
        private void InitializeSubscriptions()
        {
            Observable.FromEventPattern<PointerPressedEventArgs>(
                    h => this.PointerPressed += h,
                    h => this.PointerPressed -= h)
                .Subscribe(args => HandlePointerPressed(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PointerEventArgs>(
                    h => this.PointerMoved += h,
                    h => this.PointerMoved -= h)
                .Where(_ => _isPointerPressed)
                .Subscribe(args => HandlePointerMoved(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);
            
        }
        
        private void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
            if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                return;
            }

            if (sender == null) return;

            Focus();
            
            // 필요에 따라서는 Capture 메서드를 static 으로 만들어야 하는 수도 있겠다.
            args.Pointer.Capture(this);
            
            if (this.Equals(args.Pointer.Captured))
            {
                // 일단 여기서 Anchor 업데이트. 땜방코드
                //if (Container == null) return;
                //var check = UpdateAnchor();

                // TODO 추후 로그로
                //if (!check) throw new InvalidOperationException("Failed to update the Anchor.");
                
                Debug.Print("OnPointerPressed");
                
            }
            args.Handled = true;
            _isPointerPressed = true;
        }
        // TODO
        // Move 될때, OutConnector 에 들어 갔는지 판단해야하고 들어갔으면 Capture 를 해제해준다.
        private void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
            if (sender == null) return;
            if (this.Equals(args.Pointer.Captured))
            {
                // 마우스 위치에서 OutConnector 발견되면(추가적으로 OutConnector 를 가지고 있는 Node 도 포함. 향후에 이렇게 정리)
                // Capture 를 OutConnector 로 넘기고, OutConnector 가 이벤트 발생할 수 있도록 한다.
                // 이때 OutConnector 가 이벤트가 발생하면, Caputure 는 null 로 세팅한다.
                // 만약 마우스 위치가 OutConnector(Node 포함) 다시 원래대로 InConnector 로 캡쳐를 돌려놓는다.
                
                // WPF 코드 여기서 부터 시작.
                /*var hitControl = VisualTree.HitTest(this, point);
                if (hitControl != null)
                {
                    // hitControl을 통해 마우스 아래에 있는 컨트롤 확인
                    // 예: if (hitControl is OutConnector)
                }*/
                
                // 현재 위치에 있는 모든 시각적 요소들을 찾습니다.

                if (myCanvas != null)
                {
                    var pointerPosition = args.GetPosition(myCanvas);
                    //var visualsAtPointer = myCanvas.GetVisualsAt(pointerPosition);
                    var myout =  this.FindControl<OutConnector>("myOut");
                    Point realPosition;
                    if (myout != null)
                    {
                        double left = Canvas.GetLeft(myout);
                        double top = Canvas.GetTop(myout);
                        
                        realPosition = new Point(Canvas.GetLeft(myout) + myout.Width / 2, 
                            Canvas.GetTop(myout) + myout.Height / 2);
                        //realPosition = new Point(left, top);
                    }
                    else
                    {
                        realPosition = new Point(200,200);
                    }
                    
                    //var realPosition = new Point(200,200);
                    var visualsAtPointer = myCanvas.GetVisualsAt(realPosition);
                    foreach (var visual in visualsAtPointer)
                    {
                        if (visual is OutConnector outConnector)
                        {
                            // 마우스 포인터 아래에 OutConnector가 있는 경우에 대한 처리
                            args.Pointer.Capture(outConnector);
                        } 
                        //else args.Pointer.Capture(this);

                        if (visual is InConnector inConnector)
                        {
                            Debug.WriteLine("test");
                        }

                    }
                }

                
                
                if (_thumbCenter.HasValue)
                {
                    Vector? offset = args.GetPosition(Thumb) - _thumbCenter.Value;
                }
            }
           

            args.Handled = true;
        }

        /*private void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
            if (sender == null) return;
            if (this.Equals(args.Pointer.Captured))
            {
                if (Thumb != null)
                {
                    _thumbCenter = new Point(Thumb.Bounds.Width / 2, Thumb.Bounds.Height / 2);
                }

                //CompletedRaiseEvent(args);
                // capture 해제.
                args.Pointer.Capture(null);
                args.Handled = true;
            }
        }*/
        
        /*private void InitializeSubscriptions()
        {
            Observable.FromEventPattern<PendingConnectionEventHandler, PendingConnectionEventArgs>(
                    h => this.PendingConnectionStarted += h,
                    h => this.PendingConnectionStarted -= h)
                .Subscribe(args => HandleStarted(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PendingConnectionEventHandler,PendingConnectionEventArgs>(
                    h => this.PendingConnectionDrag += h,
                    h => this.PendingConnectionDrag -= h)
                .Subscribe(args => HandleDrag(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PendingConnectionEventHandler, PendingConnectionEventArgs>(
                    h => this.PendingConnectionCompleted += h,
                    h => this.PendingConnectionCompleted -= h)
                .Subscribe(args => HandleCompleted(args.Sender, args.EventArgs))
                .DisposeWith(_disposables); 
        }*/

        /*private void HandleStarted(object? sender, PendingConnectionEventArgs e)
        {
            // sender 따라 처리   
            if (sender == null) return;
            if (!this.Equals(sender)) return;

            if (e.CapturedObject == null)
                Debug.WriteLine("startd captured is null");
            else Debug.WriteLine("started not null");
            
        }
        
        private void HandleDrag(object? sender, PendingConnectionEventArgs e)
        {
            if (e.CapturedObject == null)
                Debug.WriteLine("drag captured is null");
            else Debug.WriteLine("drag not null");
        }
        
        private void HandleCompleted(object? sender, PendingConnectionEventArgs e)
        {
            if (e.CapturedObject == null)
                Debug.WriteLine("com captured is null");
            else Debug.WriteLine("com not null");
        }*/
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
    }
}
