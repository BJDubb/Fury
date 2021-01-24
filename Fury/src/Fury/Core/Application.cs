using Fury;
using Fury.Events;
using Fury.Utils;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Fury.Rendering;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using Fury.GUI;
using System.Threading;
using OpenTK.Windowing.Desktop;

namespace Fury
{
    public class Application
    {
        private IWindow window;
        private LayerStack layerStack = new LayerStack();

        private bool running = true;
        double lastFrameTime;

        private Thread updateThread;

        public Application()
        {
            Core.GetPlatform();

            window = WindowFactory.CreateWindow();
            if (window == null)
            {
                Logger.Info("Exiting Environment...");
                Environment.Exit(1);
            }

            window.SetEventCallback(OnEvent);
        }

        public void Run()
        {
            Logger.Info("Welcome to the Fury Engine!");
            Logger.Info("Platform: " + RuntimeInformation.OSDescription);

            ImGuiController.Init(window);

            while (running)
            {
                double time = GLFW.GetTime();
                float elapsed = (float)(time - lastFrameTime);
                Time.deltaTime = elapsed;
                lastFrameTime = time;

                if (!window.Minimised)
                {

                    foreach (Layer layer in layerStack.Layers)
                    {
                        layer.OnUpdate();
                    }

                    ImGuiController.Begin(window);
                    foreach (Layer layer in layerStack.Layers)
                    {
                        layer.OnImGuiRender();
                    }
                    ImGuiController.End();

                }

                window.OnUpdate();
            }
        }

        public void OnEvent(Event e)
        {
            EventDispatcher dispatcher = new EventDispatcher(e);
            dispatcher.Dispatch<WindowCloseEvent>(e, OnWindowClose);
            dispatcher.Dispatch<WindowResizeEvent>(e, OnWindowResize);
            dispatcher.Dispatch<ConsoleLoggedEvent>(e, OnConsoleLog);

            foreach (Layer layer in layerStack.Layers)
            {
                if (e.Handled) break;
                layer.OnEvent(e);
            }
        }

        public static Application GetApplication()
        {
            return EntryPoint.app;
        }

        public IWindow GetWindow()
        {
            return window;
        }

        public void PushLayer(Layer layer) => layerStack.PushLayer(layer);

        bool OnWindowClose(WindowCloseEvent e)
        {
            running = false;
            return true;
        }

        bool OnWindowResize(WindowResizeEvent e)
        {
            Renderer.Resize(0, 0, e.Width, e.Height);
            return false;
        }

        bool OnConsoleLog(ConsoleLoggedEvent arg)
        {
            switch (arg.Log.Severity)
            {
                case Severity.Info:
                    Logger.Info(arg.Log.Message);
                    break;
                case Severity.Warn:
                    Logger.Warn(arg.Log.Message);
                    break;
                case Severity.Error:
                    Logger.Error(arg.Log.Message);
                    break;
            }
            return false;
        }
    }
}

