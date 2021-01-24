using Fury;
using Fury.Events;
using Fury.Rendering;
using Fury.Utils;

using ImGuiNET;

using OpenTK.Graphics.OpenGL4;

using System;
using System.Collections.Generic;

using System.Numerics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;

namespace FuryEditor
{
    public class EditorLayer : Layer
    {

        private Application app;
        private int windowWidth;
        private int windowHeight;

        Vector2 viewportSize;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        VertexArray vertexArray;
        Shader shader;
        Framebuffer framebuffer;

        Texture2D checkerboard;
        Texture2D cherno;

        public OrthographicCamera orthoCamera;



        //public Camera selectedCamera;

        int selectedCameraIndex = 1;

        Vector3 squarePos;
        float squareSpeed = 1;

        private bool EnableDeltaTime = true;

        public EditorLayer() : base("EditorLayer") { }

        public override void OnAttach()
        {
            app = Application.GetApplication();
            var window = app.GetWindow();

            Renderer.SetClearColor(Color.White.ToVec4());

            orthoCamera = new OrthographicCamera(-16f, 16f, -9f, 9f);
            //persCamera = new PerspectiveCamera(45, 16f/9f, 0.01f, 1000);

            //selectedCamera = persCamera;
            //persCamera.Position = new OpenTK.Mathematics.Vector3(0, 0, -1);

            float[] verticies =
            {
                -0.5f, -0.5f,  0.0f, /* Colors  0.0f, 0.0f, 0.0f, */ /* Tex Coords */ 0.0f, 0.0f,
                 0.5f,  0.5f,  0.0f, /* Colors  0.0f, 0.0f, 0.0f, */ /* Tex Coords */ 1.0f, 1.0f,
                -0.5f,  0.5f,  0.0f, /* Colors  0.0f, 0.0f, 0.0f, */ /* Tex Coords */ 0.0f, 1.0f,
                 0.5f, -0.5f,  0.0f, /* Colors  0.0f, 0.0f, 0.0f, */ /* Tex Coords */ 1.0f, 0.0f,
            };

            uint[] indices =
            {
                0, 1, 2,
                0, 3, 1
            };

            shader = new Shader("Assets/Shaders/vertex.glsl", "Assets/Shaders/fragment.glsl", true);

            vertexBuffer = new VertexBuffer(verticies, verticies.Length * sizeof(float));
            indexBuffer = new IndexBuffer(indices, indices.Length * sizeof(uint));

            BufferLayout layout = new BufferLayout(
                new BufferElement("positions", ShaderDataType.Float3),
                new BufferElement("texCoords", ShaderDataType.Float2)
                //new BufferElement("colors", ShaderDataType.Float3)
            );

            vertexBuffer.SetLayout(layout);

            vertexArray = new VertexArray();
            vertexArray.AddVertexBuffer(vertexBuffer);
            vertexArray.SetIndexBuffer(indexBuffer);

            checkerboard = new Texture2D("Assets/Textures/Checkerboard.png");
            cherno = new Texture2D("Assets/Textures/ChernoLogo.png");

            shader.SetInt("uTexture", 0);

            vendorString = GL.GetString(StringName.Vendor);
            rendererString = GL.GetString(StringName.Renderer);
            versionString = GL.GetString(StringName.Version);

            FramebufferData fbData = new FramebufferData(1280, 720);
            framebuffer = new Framebuffer(fbData);

            viewportSize = new Vector2(framebuffer.FramebufferData.Width, framebuffer.FramebufferData.Height);
        }

        public override void OnDettach()
        {
            
        }

        public override void OnUpdate()
        {
            var spec = framebuffer.FramebufferData;
            if (spec.Width > 0 && spec.Height > 0 && viewportSize != new Vector2(spec.Width, spec.Height))
            {
                framebuffer.Resize((int)viewportSize.X, (int)viewportSize.Y);
                var aspectRatio = viewportSize.X / viewportSize.Y;
                var zoomLevel = 0.5f;
                orthoCamera.SetProjection(-aspectRatio * zoomLevel, aspectRatio * zoomLevel, -zoomLevel, zoomLevel);
            }

            framebuffer.Bind();

            if (Input.IsKeyPressed(Key.A))
                orthoCamera.Position += new OpenTK.Mathematics.Vector3(1, 0, 0) * orthoCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(Key.D))
                orthoCamera.Position += new OpenTK.Mathematics.Vector3(-1, 0, 0) * orthoCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(Key.W))
                orthoCamera.Position += new OpenTK.Mathematics.Vector3(0, 1, 0) * orthoCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(Key.S))
                orthoCamera.Position += new OpenTK.Mathematics.Vector3(0, -1, 0) * orthoCamera.Speed * (EnableDeltaTime ? Time.deltaTime : 1);

            if (Input.IsKeyPressed(Key.Left))
                squarePos += new Vector3(-1, 0, 0) * squareSpeed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(Key.Right))
                squarePos += new Vector3(1, 0, 0) * squareSpeed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(Key.Up))         
                squarePos += new Vector3(0, 1, 0) * squareSpeed * (EnableDeltaTime ? Time.deltaTime : 1);
            if (Input.IsKeyPressed(Key.Down))
                squarePos += new Vector3(0, -1, 0) * squareSpeed * (EnableDeltaTime ? Time.deltaTime : 1);

            Renderer.Clear();

            Renderer.BeginScene(orthoCamera);

            checkerboard.Bind();
            Renderer.Submit(vertexArray, shader, OpenTK.Mathematics.Matrix4.CreateTranslation(squarePos.ToVec3()));
            cherno.Bind();
            Renderer.Submit(vertexArray, shader, OpenTK.Mathematics.Matrix4.CreateTranslation(squarePos.ToVec3()));

            Renderer.EndScene();
            framebuffer.Unbind();
        }

        private static bool useDocking = true;
        private static bool dockspaceOpen = true;

        private static bool showViewportWindow = true;
        private static bool showConsoleWindow = true;
        private static bool showDebugWindow = true;
        private static bool showDemoWindow;

        string vendorString;
        string rendererString;
        string versionString;


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
            ImGui.SetMouseCursor(ImGui.IsAnyItemHovered() ? ImGuiMouseCursor.Hand : ImGuiMouseCursor.Arrow);

            if (useDocking)
            {
                ImGuiViewportPtr viewport = ImGui.GetMainViewport();
                ImGui.SetNextWindowPos(viewport.Pos);
                ImGui.SetNextWindowSize(viewport.Size);
                ImGui.SetNextWindowViewport(viewport.ID);
                //ImGui.SetNextWindowBgAlpha(0);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

                ImGui.Begin("Dockspace", ref dockspaceOpen, windowFlags);

                ImGui.PopStyleVar();
                ImGui.PopStyleVar();
                ImGui.PopStyleVar();
                ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 2.0f);
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
                    if (ImGui.MenuItem("Viewport", "", showViewportWindow)) showViewportWindow = !showViewportWindow;
                    if (ImGui.MenuItem("Console", "", showConsoleWindow)) showConsoleWindow = !showConsoleWindow;
                    if (ImGui.MenuItem("Debug", "", showDebugWindow)) showDebugWindow = !showDebugWindow;
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            // Viewport Window
            if (showViewportWindow)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
                ImGui.Begin("Viewport", ref showViewportWindow);

                viewportSize = ImGui.GetContentRegionAvail();
                

                int textureId = framebuffer.colorAttachment;
                ImGui.Image((IntPtr)textureId, viewportSize);

                ImGui.PopStyleVar();
                ImGui.End();
            }

            //Debug Window
            if (showDebugWindow)
            {
                ImGui.Begin("Debug", ref showDebugWindow);
                ImGui.Text("Renderer Info:");
                ImGui.Text("Vendor: " + vendorString);
                ImGui.Text("Renderer: " + rendererString);
                ImGui.Text("Version: " + versionString);
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

                //selectedCamera = selectedCameraIndex == 0 ? orthoCamera : (Camera)persCamera;

                // Camera Position Float3
                {
                    ImGui.Text("Camera Position");
                    var hovered = ImGui.IsItemHovered();
                    if (hovered) ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    Vector3 pos = new Vector3(orthoCamera.Position.X, orthoCamera.Position.Y, orthoCamera.Position.Z);
                    ImGui.DragFloat3("#position", ref pos, 0.1f);
                    if (hovered && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                        orthoCamera.Position = new OpenTK.Mathematics.Vector3(0, 0, -1);
                    else
                        orthoCamera.Position = new OpenTK.Mathematics.Vector3(pos.X, pos.Y, pos.Z);
                }

                {
                    ImGui.Text("Camera Rotation");
                    var hovered = ImGui.IsItemHovered();
                    if (hovered) ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    Vector3 rot = new Vector3(orthoCamera.Rotation.X, orthoCamera.Rotation.Y, orthoCamera.Rotation.Z);
                    ImGui.DragFloat3("#rotation", ref rot, 0.1f);
                    if (hovered && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                        orthoCamera.Rotation = new OpenTK.Mathematics.Vector3(0, 0, 0);
                    else
                        orthoCamera.Rotation = new OpenTK.Mathematics.Vector3(rot.X, rot.Y, rot.Z);
                }
                    

                ImGui.DragFloat("Camera Speed", ref orthoCamera.Speed, 0.1f);

                ImGui.Checkbox("Enable deltaTime", ref EnableDeltaTime);

                ImGui.Separator();

                ImGui.Text("Square:");
                ImGui.DragFloat3("Square Position", ref squarePos, 0.1f);
                ImGui.DragFloat("Square Speed", ref squareSpeed, 0.1f);

                ImGui.Separator();

                var clearColor = Renderer.GetClearColor();
                ImGui.ColorPicker4("Clear Color", ref clearColor);
                Renderer.SetClearColor(clearColor);

                ImGui.End();
            }

            //Console Window
            if (showConsoleWindow)
            {
                ImGui.Begin("Console", ref showConsoleWindow);

                if (ImGui.Button("Clear")) Logger.Logs.Clear(); 

                foreach (var log in Logger.Logs)
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
            if (useDocking)
            {
                ImGui.PopStyleVar();
                ImGui.End();
            }
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
