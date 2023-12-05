using Avalonia;
using Avalonia.Input;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
        }

        private void HandleStarted(object? sender, PendingConnectionEventArgs e)
        {
            // sender 따라 처리   
        }
        
        private void HandleDrag(object? sender, PendingConnectionEventArgs e)
        {
            
        }
        
        private void HandleCompleted(object? sender, PendingConnectionEventArgs e)
        {
            
        }
        
    }
}
