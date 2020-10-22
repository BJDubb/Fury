﻿using Fury.Core;
using Fury.Events;
using Fury.Math;
using Fury.Utils;

using ImGuiNET;

using OpenToolkit.Graphics.OpenGL4;

using System;
using System.Collections.Generic;

namespace Sandbox
{
    public class EditorLayer : Layer
    {

        private List<Log> logs = new List<Log>();

        private Application app;
        private int windowWidth;
        private int windowHeight;

        public EditorLayer() : base("EditorLayer") { }

        public override void OnAttach()
        {
            app = Application.GetApplication();
        }

        public override void OnDettach()
        {
            base.OnDettach();
        }

        public override void OnUpdate(double elapsed)
        {
        }

        private static bool useDocking = true;
        private static bool dockspaceOpen = true;

        private static bool showConsoleWindow = true;
        private static bool showDebugWindow = true;
        private static bool showDemoWindow;

        public override void OnImGuiRender()
        {
            var windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoBringToFrontOnFocus;
            var dockspaceFlags = ImGuiDockNodeFlags.None;

            var io = ImGui.GetIO();

            if (useDocking)
            {
                ImGuiViewportPtr viewport = ImGui.GetMainViewport();
                ImGui.SetNextWindowPos(viewport.Pos);
                ImGui.SetNextWindowSize(viewport.Size);
                ImGui.SetNextWindowViewport(viewport.ID);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

                ImGui.Begin("Dockspace Demo", ref dockspaceOpen, windowFlags);

                ImGui.PopStyleVar();
                ImGui.PopStyleVar();
                ImGui.PopStyleVar();
            }

            if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable))
            {
                var dockspaceID = ImGui.GetID("MyDockSpace");
                ImGui.DockSpace(dockspaceID, Vector2.Zero, dockspaceFlags);
            }

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Close")) Application.GetApplication().OnEvent(new WindowCloseEvent());
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Window"))
                {
                    if (ImGui.MenuItem("ImGui Demo", "", showDemoWindow)) showDemoWindow = !showDemoWindow;
                    if (ImGui.MenuItem("Console", "", showConsoleWindow)) showConsoleWindow = !showConsoleWindow;
                    if (ImGui.MenuItem("Debug", "", showDebugWindow)) showDebugWindow = !showDebugWindow;
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            //Debug Window
            if (showDebugWindow)
            {
                ImGui.Begin("Debug", ref showDebugWindow);
                ImGui.Text("Renderer Info:");
                ImGui.Text("Vendor: " + GL.GetString(StringName.Vendor));
                ImGui.Text("Renderer: " + GL.GetString(StringName.Renderer));
                ImGui.Text("Version: " + GL.GetString(StringName.Version));
                ImGui.Text($"Frame Time: {io.DeltaTime * 1000:F}ms");
                ImGui.Text($"FPS: {1 / io.DeltaTime:F}");
                ImGui.Separator();
                ImGui.Text("Window Info:");
                ImGui.Text("Title: " + app.GetWindow().Title);
                ImGui.Text("Width: " + app.GetWindow().Width);
                ImGui.Text("Height: " + app.GetWindow().Height);
                ImGui.Separator();
                ImGui.Text("Object Loader:");
                ImGui.Separator();
                ImGui.End();
            }

            //Console Window
            if (showConsoleWindow)
            {
                ImGui.Begin("Console", ref showConsoleWindow);

                Action<Event> logCallback = Application.GetApplication().OnEvent;

                if (ImGui.Button("Info Log Test")) logCallback(new ConsoleLoggedEvent(new Log("Info Log", Severity.Info)));
                ImGui.SameLine();
                if (ImGui.Button("Warn Log Test")) logCallback(new ConsoleLoggedEvent(new Log("Warn Log", Severity.Warn)));
                ImGui.SameLine();
                if (ImGui.Button("Error Log Test")) logCallback(new ConsoleLoggedEvent(new Log("Error Log", Severity.Error)));

                foreach (var log in logs)
                {
                    Vector4 col = Vector4.One;
                    switch (log.Severity)
                    {
                        case Severity.Info: col = Vector4.One; break;
                        case Severity.Warn: col = new Vector4(251f / 255f, 195f / 255f, 42 / 255f, 1); break;
                        case Severity.Error: col = new Vector4(255, 0, 0, 1); break;
                    }

                    ImGui.Text(log.Time.ToString(Logger.format));
                    ImGui.SameLine();
                    ImGui.TextColored(col, $"[{log.Severity}]");
                    ImGui.SameLine();
                    ImGui.Text(log.Message);
                }

                ImGui.End();
            }


            if (showDemoWindow) ImGui.ShowDemoWindow(ref showDemoWindow);

            //Dockspace End
            if (useDocking) ImGui.End();
        }

        public override void OnEvent(Event e)
        {
            EventDispatcher dispatcher = new EventDispatcher(e);

            dispatcher.Dispatch<WindowResizeEvent>(e, WindowResized);
            dispatcher.Dispatch<MouseMovedEvent>(e, MouseMoved);
            dispatcher.Dispatch<MouseButtonPressedEvent>(e, MouseButtonPressed);
            dispatcher.Dispatch<MouseButtonReleasedEvent>(e, MouseButtonReleased);
            dispatcher.Dispatch<MouseScrolledEvent>(e, MouseScrolled);
            dispatcher.Dispatch<KeyPressedEvent>(e, KeyPressed);
            dispatcher.Dispatch<KeyReleasedEvent>(e, KeyReleased);
            dispatcher.Dispatch<TextInputEvent>(e, TextInput);
            dispatcher.Dispatch<ConsoleLoggedEvent>(e, ConsoleLog);
        }

        private bool ConsoleLog(ConsoleLoggedEvent e)
        {
            logs.Add(e.Log);
            return false;
        }

        private bool MouseScrolled(MouseScrolledEvent e)
        {
            ImGui.GetIO().MouseWheel = e.YOffset;
            ImGui.GetIO().MouseWheelH = e.XOffset;
            return false;
        }

        private bool MouseButtonReleased(MouseButtonReleasedEvent e)
        {
            ImGui.GetIO().MouseDown[(int)e.Button] = false;
            return false;
        }

        private bool MouseButtonPressed(MouseButtonPressedEvent e)
        {
            ImGui.GetIO().MouseDown[(int)e.Button] = true;
            return false;
        }

        private bool KeyReleased(KeyReleasedEvent e)
        {
            var io = ImGui.GetIO();
            io.KeyCtrl = e.Control;
            io.KeyAlt = e.Alt;
            io.KeyShift = e.Shift;
            io.KeysDown[e.KeyCode] = false;
            return false;
        }

        private bool KeyPressed(KeyPressedEvent e)
        {
            var io = ImGui.GetIO();
            io.KeyCtrl = e.Control;
            io.KeyAlt = e.Alt;
            io.KeyShift = e.Shift;
            io.KeysDown[e.KeyCode] = true;
            return false;
        }

        private bool TextInput(TextInputEvent e)
        {
            ImGui.GetIO().AddInputCharacter((uint)e.Unicode);
            return false;
        }

        public bool WindowResized(WindowResizeEvent e)
        {
            windowHeight = e.Height;
            windowWidth = e.Width;
            return false;
        }

        public bool MouseMoved(MouseMovedEvent e)
        {
            ImGui.GetIO().MousePos = new Vector2(e.X, e.Y);
            return false;
        }

    }
}
