using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.GraphicsLibraryFramework;
using System;
using System.Runtime.InteropServices;
using Fury.Core;
using Fury.Events;
using Fury.Platform.Windows;
using OpenToolkit.Graphics.OpenGL4;

namespace Fury
{
    public class WindowsWindow : IWindow
    {
        private WindowData data;
        private GLFWWindow window;

        public WindowsWindow() : this(WindowProperties.Default) { }

        public WindowsWindow(WindowProperties props)
        {
            Init(props);
        }

        private void Init(WindowProperties props)
        {
            data.Title = props.Title;
            data.Width = props.Width;
            data.Height = props.Height;

            window = new GLFWWindow(NativeWindowSettings.Default) { Title = props.Title, Size = new OpenToolkit.Mathematics.Vector2i(props.Width, props.Height)};
            GL.Viewport(0, 0, Width, Height);

            window.Resize += e =>
            {
                data.Width = e.Width;
                data.Height = e.Height;
                data.EventCallback(new WindowResizeEvent(e.Width, e.Height));
            };

            window.Closing += e =>
            {
                data.EventCallback(new WindowCloseEvent());
            };

            window.KeyDown += e =>
            {
                data.EventCallback(new KeyPressedEvent(e.ScanCode));
            };

            window.KeyUp += e =>
            {
                data.EventCallback(new KeyReleasedEvent(e.ScanCode));
            };

            window.MouseDown += e =>
            {
                data.EventCallback(new MouseButtonPressedEvent(e.Button));
            };

            window.MouseUp += e =>
            {
                data.EventCallback(new MouseButtonReleasedEvent(e.Button));
            };

            window.MouseWheel += e =>
            {
                data.EventCallback(new MouseScrolledEvent(e.OffsetX, e.OffsetY));
            };

            window.MouseMove += e =>
            {
                data.EventCallback(new MouseMovedEvent(e.X, e.Y));
            };
        }


        public object GetNativeWindow()
        {
            return window;
        }

        public string Title { get => data.Title; set => data.Title = value; }
        public int Width { get => data.Width; set => data.Width = value; }
        public int Height { get => data.Height; set => data.Height = value; }

        public unsafe void* Handle => window.WindowPtr;

        public void OnUpdate()
        {
            window.ProcessEvents();
        }

        public void SetEventCallback(Action<Event> callback)
        {
            data.EventCallback = callback;
        }

        public unsafe void SwapBuffers()
        {
            GLFW.SwapBuffers(window.WindowPtr);
        }

        public void Dispose()
        {
            window.Dispose();
        }

        public struct WindowData
        {
            public string Title;
            public int Width, Height;
            public Action<Event> EventCallback;
        }
    }
}
