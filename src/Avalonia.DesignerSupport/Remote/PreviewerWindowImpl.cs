﻿using System;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Remote.Server;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Remote.Protocol;
using Avalonia.Remote.Protocol.Viewport;
using Avalonia.Threading;

namespace Avalonia.DesignerSupport.Remote
{
    class PreviewerWindowImpl : RemoteServerTopLevelImpl, IWindowImpl
    {
        private readonly IAvaloniaRemoteTransportConnection _transport;

        public PreviewerWindowImpl(IAvaloniaRemoteTransportConnection transport) : base(transport)
        {
            _transport = transport;
            ClientSize = new Size(1, 1);
        }

        public void Show(bool activate, bool isDialog)
        {
        }

        public void Hide()
        {
        }

        public void BeginMoveDrag(PointerPressedEventArgs e)
        {
        }

        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
        }

        public override double DesktopScaling => 1.0;
        public PixelPoint Position { get; set; }
        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Deactivated { get; set; }
        public Action Activated { get; set; }
        public Func<WindowCloseReason, bool> Closing { get; set; }
        public WindowState WindowState { get; set; }
        public Action<WindowState> WindowStateChanged { get; set; }
        public Size MaxAutoSizeHint { get; } = new Size(4096, 4096);

        protected override void OnMessage(IAvaloniaRemoteTransportConnection transport, object obj)
        {
            // In previewer mode we completely ignore client-side viewport size
            if (obj is ClientViewportAllocatedMessage alloc)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    RenderScaling = alloc.DpiX / 96.0;
                    RenderAndSendFrameIfNeeded();
                });
                return;
            }
            base.OnMessage(transport, obj);
        }
        
        public void Resize(Size clientSize, WindowResizeReason reason)
        {
            _transport.Send(new RequestViewportResizeMessage
            {
                Width = Math.Ceiling(clientSize.Width * RenderScaling),
                Height = Math.Ceiling(clientSize.Height * RenderScaling)
            });
            ClientSize = clientSize;
            RenderAndSendFrameIfNeeded();
        }

        public void Move(PixelPoint point)
        {
            
        }

        public void SetMinMaxSize(Size minSize, Size maxSize)
        {
        }

        public Action GotInputWhenDisabled { get; set; }        
        
        public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }

        public Thickness ExtendedMargins { get; } = new Thickness();

        public bool IsClientAreaExtendedToDecorations { get; }

        public Thickness OffScreenMargin { get; } = new Thickness();

        public bool NeedsManagedDecorations => false;

        public override object TryGetFeature(Type featureType)
        {
            if (featureType == typeof(IStorageProvider))
            {
                return new NoopStorageProvider();
            }

            if (featureType == typeof(IScreenImpl))
            {
                return new ScreenStub();
            }

            return base.TryGetFeature(featureType);
        }
        
        public void Activate()
        {
        }
        
        public void SetTitle(string title)
        {
        }

        public void SetSystemDecorations(SystemDecorations enabled)
        {
        }

        public void SetIcon(IWindowIconImpl icon)
        {
        }

        public void ShowTaskbarIcon(bool value)
        {
        }

        public void CanResize(bool value)
        {
        }

        public void SetTopmost(bool value)
        {
        }

        public void SetParent(IWindowImpl parent)
        {
        }

        public void SetEnabled(bool enable)
        {
        }

        public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
        {            
        }

        public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
        {            
        }

        public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
        {            
        }

        public void GetWindowsZOrder(Span<Window> windows, Span<long> zOrder) => throw new NotSupportedException();
    }
}
