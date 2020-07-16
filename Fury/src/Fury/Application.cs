using Fury.Events;
using Fury.ImGUI;
using Fury.Utils;
using OpenToolkit.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenToolkit.Windowing.Desktop;

namespace Fury
{
    public class Application
    {
        private IWindow window;
        private LayerStack layerStack = new LayerStack();
        private ImGuiLayer imGuiLayer;

        private bool running = true;

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

            unsafe
            {
                imGuiLayer = new ImGuiLayer(window.Width, window.Height, window.Handle);
            }
        }

        private Stopwatch gameLoopWatch = new Stopwatch();

        public void Run()
        {
            Logger.Info("Welcome to the Fury Engine!");
            Logger.Info("Platform: " + RuntimeInformation.OSDescription);

            PushOverlay(imGuiLayer);

            GL.ClearColor(Color.Fuchsia); //OPENGL: Remove

            gameLoopWatch.Start();
            var elapsed = gameLoopWatch.Elapsed.TotalSeconds;

            while (running)
            {
                gameLoopWatch.Restart();

                GL.Clear(ClearBufferMask.ColorBufferBit); //OpenGL: Remove

                foreach (Layer layer in layerStack.Layers)
                {
                    layer.OnUpdate(elapsed);
                }

                imGuiLayer.Begin(elapsed);
                foreach (Layer layer in layerStack.Layers)
                {
                    layer.OnImGuiRender();
                }
                imGuiLayer.End();



                window.OnUpdate();
                window.SwapBuffers();

                elapsed = gameLoopWatch.Elapsed.TotalSeconds;
            }
        }

        public void OnEvent(Event e)
        {
            //Logger.Info($"[APP] {e.GetType().Name}: {e.ToString()}");
            EventDispatcher dispatcher = new EventDispatcher(e);
            dispatcher.Dispatch<WindowCloseEvent>(e, OnWindowClose);
            dispatcher.Dispatch<WindowResizeEvent>(e, OnWindowResize);

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

        public void PushOverlay(Layer layer) => layerStack.PushOverlay(layer);

        bool OnWindowClose(WindowCloseEvent e)
        {
            running = false;
            return true;
        }

        bool OnWindowResize(WindowResizeEvent e)
        {
            GL.Viewport(0, 0, e.Width, e.Height); //OPENGL: Remove
            return false;
        }
    }
}
