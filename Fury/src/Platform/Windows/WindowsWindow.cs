using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using Fury.Events;
using OpenTK.Graphics.OpenGL4;
using ImGuiNET;

namespace Fury
{
    public class WindowsWindow : NativeWindow, IWindow
    {
        private WindowData data;

        public WindowsWindow() : this(WindowProperties.Default) { }
        public WindowsWindow(WindowProperties props) : base(NativeWindowSettings.Default)
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

            TextInput += e =>
            {
                data.EventCallback(new TextInputEvent(e.Unicode));
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

        public int Width { get => data.Width; set => data.Width = value; }
        public int Height { get => data.Height; set => data.Height = value; }
        public bool Minimised => WindowState == OpenTK.Windowing.Common.WindowState.Minimized;

        public unsafe void* Handle => WindowPtr;

        public void OnUpdate()
        {
            if (lastVSync != VSync)
            {
                GLFW.SwapInterval(VSync ? 1 : 0);
                lastVSync = VSync;
            }

            ProcessEvents();
            unsafe { GLFW.SwapBuffers(WindowPtr); }
        }

        public void SetEventCallback(Action<Event> callback)
        {
            data.EventCallback = callback;
        }

        public object GetNativeWindow()
        {
            return this;
        }

        private bool lastVSync;
        public bool VSync = false;

        public struct WindowData
        {
            public string Title;
            public int Width, Height;
            public Action<Event> EventCallback;
        }
    }
}
