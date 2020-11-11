using System;
using System.Collections.Generic;
using System.Text;
using Fury.Events;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Fury
{
    public class MacOSWindow : NativeWindow, IWindow
    {
        private WindowData data;

        public MacOSWindow() : this(WindowProperties.Default) { }

        public MacOSWindow(WindowProperties props) : base(NativeWindowSettings.Default)
        {
            Init(props);
        }

        private void Init(WindowProperties props)
        {
            data.Title = props.Title;
            data.Width = props.Width;
            data.Height = props.Height;

            Title = props.Title;
            Size = new OpenTK.Mathematics.Vector2i(props.Width, props.Height);
            GL.Viewport(0, 0, Width, Height);

            Resize += e =>
            {
                data.Width = e.Width;
                data.Height = e.Height;
                data.EventCallback(new WindowResizeEvent(e.Width, e.Height));
            };

            Closing += e =>
            {
                data.EventCallback(new WindowCloseEvent());
            };

            KeyDown += e =>
            {
                data.EventCallback(new KeyPressedEvent((int)e.Key, e.Control, e.Alt, e.Shift));
            };

            KeyUp += e =>
            {
                data.EventCallback(new KeyReleasedEvent((int)e.Key, e.Control, e.Alt, e.Shift));
            };

            MouseDown += e =>
            {
                data.EventCallback(new MouseButtonPressedEvent(e.Button));
            };

            MouseUp += e =>
            {
                data.EventCallback(new MouseButtonReleasedEvent(e.Button));
            };

            MouseWheel += e =>
            {
                data.EventCallback(new MouseScrolledEvent(e.OffsetX, e.OffsetY));
            };

            MouseMove += e =>
            {
                data.EventCallback(new MouseMovedEvent(e.X, e.Y));
            };
        }


        public object GetNativeWindow()
        {
            return this;
        }

        public new string Title { get => data.Title; set => data.Title = value; }
        public int Width { get => data.Width; set => data.Width = value; }
        public int Height { get => data.Height; set => data.Height = value; }
        public bool Minimised => WindowState == OpenTK.Windowing.Common.WindowState.Minimized;

        public unsafe void* Handle => WindowPtr;

        public void OnUpdate()
        {
            ProcessEvents();
            unsafe { GLFW.SwapBuffers(WindowPtr); }
        }

        public void SetEventCallback(Action<Event> callback)
        {
            data.EventCallback = callback;
        }

        public unsafe void SwapBuffers()
        {
            GLFW.SwapBuffers(WindowPtr);
        }

        public struct WindowData
        {
            public string Title;
            public int Width, Height;
            public Action<Event> EventCallback;
        }
    }
}
