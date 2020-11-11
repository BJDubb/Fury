using Fury;
using Fury.Events;
using Fury.Rendering;
using Fury.Utils;

using ImGuiNET;

using OpenTK.Graphics.OpenGL4;

using System;
using System.Collections.Generic;

using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;
using System.Numerics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sandbox
{
    public class EditorLayer : Layer
    {

        private List<Log> logs = new List<Log>();

        private Application app;
        private int windowWidth;
        private int windowHeight;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        VertexArray vertexArray;
        Shader shader;

        public OrthographicCamera orthoCamera;
        public PerspectiveCamera persCamera;

        public Camera selectedCamera;
        int selectedCameraIndex;

        Vector3 trianglePos;

        private bool EnableDeltaTime = true;

        public EditorLayer() : base("EditorLayer") { }

        public override void OnAttach()
        {
            app = Application.GetApplication();
            var window = app.GetWindow();

            orthoCamera = new OrthographicCamera(-16f, 16f, -9f, 9f);
            persCamera = new PerspectiveCamera(70, 16/9, 0.01f, 1000);

            selectedCamera = orthoCamera;

            float[] verticies =
            {
                -0.5f, -0.5f,  0.0f, /* Colors */ 1.0f, 0.0f, 0.0f,
                 0.5f, -0.5f,  0.0f, /* Colors */ 0.0f, 1.0f, 0.0f,
                 0.0f,  0.5f,  0.0f, /* Colors */ 0.0f, 0.0f, 1.0f,
            };

            uint[] indices =
            {
                0, 1, 2
            };

            shader = new Shader("Assets/Shaders/vertex.glsl", "Assets/Shaders/fragment.glsl", true);

            vertexBuffer = new VertexBuffer(verticies, verticies.Length * sizeof(float));
            indexBuffer = new IndexBuffer(indices, indices.Length * sizeof(uint));

            BufferLayout layout = new BufferLayout(
                new BufferElement("positions", ShaderDataType.Float3),
                new BufferElement("colors", ShaderDataType.Float3)
            );

            vertexBuffer.SetLayout(layout);

            vertexArray = new VertexArray();
            vertexArray.AddVertexBuffer(vertexBuffer);
            vertexArray.SetIndexBuffer(indexBuffer);
        }

        public override void OnDettach()
        {
            
        }

        public override void OnUpdate()
        {
            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
                selectedCamera.Position += new OpenTK.Mathematics.Vector3(1, 0, 0) * selectedCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
                selectedCamera.Position += new OpenTK.Mathematics.Vector3(-1, 0, 0) * selectedCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
                selectedCamera.Position += new OpenTK.Mathematics.Vector3(0, 0, 1) * selectedCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
                selectedCamera.Position += new OpenTK.Mathematics.Vector3(0, 0, -1) * selectedCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);

            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left))
                trianglePos += new Vector3(-1, 0, 0) * selectedCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right))
                trianglePos += new Vector3(1, 0, 0) * selectedCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up))
                trianglePos += new Vector3(0, 1, 0) * selectedCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down))
                trianglePos += new Vector3(0, -1, 0) * selectedCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);

            Renderer.Clear();

            Renderer.BeginScene(ref selectedCamera);



            Renderer.Submit(vertexArray, shader, OpenTK.Mathematics.Matrix4.CreateTranslation(trianglePos.ToVec3()));

            Renderer.EndScene();
        }

        private static bool useDocking = true;
        private static bool dockspaceOpen = true;

        private static bool showConsoleWindow = true;
        private static bool showDebugWindow = true;
        private static bool showDemoWindow;

        public override void OnImGuiRender()
        {
            var windowFlags = ImGuiWindowFlags.MenuBar 
                            | ImGuiWindowFlags.NoDocking 
                            | ImGuiWindowFlags.NoCollapse 
                            | ImGuiWindowFlags.NoTitleBar 
                            | ImGuiWindowFlags.NoResize 
                            | ImGuiWindowFlags.NoMove 
                            | ImGuiWindowFlags.NoNav 
                            | ImGuiWindowFlags.NoBringToFrontOnFocus;

            var dockspaceFlags = ImGuiDockNodeFlags.PassthruCentralNode;

            var io = ImGui.GetIO();

            if (useDocking)
            {
                ImGuiViewportPtr viewport = ImGui.GetMainViewport();
                ImGui.SetNextWindowPos(viewport.Pos);
                ImGui.SetNextWindowSize(viewport.Size);
                ImGui.SetNextWindowViewport(viewport.ID);
                ImGui.SetNextWindowBgAlpha(0);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

                ImGui.Begin("Dockspace", ref dockspaceOpen, windowFlags);

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
                    if (ImGui.MenuItem("Close")) app.OnEvent(new WindowCloseEvent());
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
                ImGui.Checkbox("VSync", ref (app.GetWindow().GetNativeWindow() as WindowsWindow).VSync);
                ImGui.Separator();

                string[] cameras = { "Orthographic", "Perspective" };

                ImGui.Text("Camera:");
                ImGui.Combo("Selected Camera", ref selectedCameraIndex, cameras, cameras.Length);

                selectedCamera = selectedCameraIndex == 0 ? orthoCamera : (Camera)persCamera;

                Vector3 pos = new Vector3(selectedCamera.Position.X, selectedCamera.Position.Y, selectedCamera.Position.Z);
                ImGui.DragFloat3("Camera Position", ref pos);
                selectedCamera.Position = new OpenTK.Mathematics.Vector3(pos.X, pos.Y, pos.Z);

                Vector3 rot = new Vector3(selectedCamera.Rotation.X, selectedCamera.Rotation.Y, selectedCamera.Rotation.Z);
                ImGui.DragFloat3("Camera Rotation", ref rot);
                selectedCamera.Rotation = new OpenTK.Mathematics.Vector3(rot.X, rot.Y, rot.Z);

                ImGui.DragFloat("Camera Speed", ref selectedCamera.Speed);

                ImGui.Checkbox("Enable deltaTime", ref EnableDeltaTime);

                if (ImGui.Button("Create New Window"))
                {
                    unsafe 
                    {
                        GLFW.WindowHint(WindowHintBool.Maximized, false);
                        var window = GLFW.CreateWindow(200, 600, "Test Window", null, null);
                        GLFW.SetWindowAttrib(window, WindowAttribute.Resizable, true);
                        GLFW.SetWindowAttrib(window, WindowAttribute.Floating, false);
                        GLFW.SetWindowAttrib(window, WindowAttribute.Decorated, true);
                    }
                }

                var clearColor = Renderer.GetClearColor();
                ImGui.ColorPicker4("Clear Color", ref clearColor);
                Renderer.SetClearColor(clearColor);

                ImGui.End();
            }

            //Console Window
            if (showConsoleWindow)
            {
                ImGui.Begin("Console", ref showConsoleWindow);

                Action<Event> logCallback = app.OnEvent;

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

        #region Events

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
            ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(e.Width, e.Height);
            return false;
        }

        public bool MouseMoved(MouseMovedEvent e)
        {
            ImGui.GetIO().MousePos = new Vector2(e.X, e.Y);
            return false;
        }

        #endregion
    }
}
