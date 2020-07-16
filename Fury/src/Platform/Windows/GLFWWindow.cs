using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace Fury.Platform.Windows
{
    public class GLFWWindow : NativeWindow
    {
        public GLFWWindow(NativeWindowSettings settings) : base(settings) { }

        public new unsafe Window* WindowPtr => base.WindowPtr;
    }
}
