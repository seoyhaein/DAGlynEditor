using Avalonia;
using Avalonia.Input;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace DAGlynEditor
{
    public class OutConnector : Connector
    {
        protected override Type StyleKeyOverride => typeof(Connector);
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public OutConnector()
        {
            InitializeSubscriptions();
        }

        private void InitializeSubscriptions()
        {
            Observable.FromEventPattern<PointerReleasedEventArgs>(
                    h => this.PointerReleased += h,
                    h => this.PointerReleased -= h)
                .Subscribe(args => HandlePointerReleased(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);
        }
        
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

            Debug.WriteLine("Do not Drag start");
            
            /*if (e.CapturedObject == null)
                Debug.WriteLine("startd captured is null");
            else Debug.WriteLine("started not null");#1#
            
        }
        
        private void HandleDrag(object? sender, PendingConnectionEventArgs e)
        {
            if (sender == null) return;
            if (!this.Equals(sender)) return;

            Debug.WriteLine("Do not Dragging");
            
            /*if (e.CapturedObject == null)
                Debug.WriteLine("drag captured is null");
            else Debug.WriteLine("drag not null");#1#
        }
        
        private void HandleCompleted(object? sender, PendingConnectionEventArgs e)
        {
            if (sender == null) return;
            if (!this.Equals(sender)) return;
            // Handle 등 살펴 봐야 함.
            Debug.WriteLine("ok");
        }*/
        
        private void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
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
               
            }
            _isPointerPressed = false;
            args.Handled = true;
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}