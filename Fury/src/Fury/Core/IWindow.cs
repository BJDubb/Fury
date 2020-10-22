﻿using System;
using System.Collections.Generic;
using System.Text;
using Fury.Events;

namespace Fury.Core
{
    public interface IWindow
    {
        public void OnUpdate();
        public void SwapBuffers();
        public object GetNativeWindow();
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public unsafe void* Handle { get; }
        public void Dispose();
        void SetEventCallback(Action<Event> onEvent);
    }

    public struct WindowProperties
    {
        public string Title;
        public int Width;
        public int Height;

        public static WindowProperties Default => new WindowProperties("Fury Engine");
        public WindowProperties(string title = "Fury Engine", int width = 1280, int height = 720)
        {
            Title = title;
            Width = width;
            Height = height;
        }
    }
}
