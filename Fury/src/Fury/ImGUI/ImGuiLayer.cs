using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Fury.Core;
using Fury.Events;
using Fury.Platform.Windows;
using Fury.Utils;
using ImGuiNET;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common.Input;
using Vector2 = Fury.Math.Vector2;
using Vector3 = Fury.Math.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace Fury.ImGUI
{
    public unsafe class ImGuiLayer : Layer
    {
        public ImGuiLayer(int width, int height, void* handle) : base("ImGuiLayer")
        {
            this.windowWidth = width;
            this.windowHeight = height;
            this.windowHandle = handle;
        }

        private int windowWidth;
        private int windowHeight;
        private void* windowHandle;
        private Application app;

        private bool frameBegun;

        private Texture fontTexture;
        private ImGuiShader shader;

        private int vertexArray;
        private int vertexBuffer;
        private int indexBuffer;

        private int vertexBufferSize;
        private int indexBufferSize;

        private Vector2 scaleFactor = Vector2.One;

        private MouseState PrevMouseState;
        private KeyboardState PrevKeyboardState;
        private List<Log> logs = new List<Log>();

        public override void OnAttach()
        {
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            ImGuiIOPtr io = ImGui.GetIO();
            
            ImGui.StyleColorsDark();

            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            //io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            //io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;
            //io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            //io.BackendFlags |= ImGuiBackendFlags.PlatformHasViewports;
            //io.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;

            ImGuiPlatformIOPtr platformIO = ImGui.GetPlatformIO();
            ImGuiViewportPtr mainViewport = platformIO.MainViewport;
            mainViewport.PlatformHandle = (IntPtr)windowHandle;

            InitOpenGL(); // OPENGL : Remove

            SetKeyMappings();

            SetPerFrameImGuiData(1f / 60f);

            app = Application.GetApplication();

            ImGui.NewFrame();
            frameBegun = true;
        }

        private void InitOpenGL()
        {
            CreateGLResources();
        }


        public void Begin(double deltaTime)
        {
            if (frameBegun)
                ImGui.Render();

            SetPerFrameImGuiData((float)deltaTime);
            UpdateImGuiInput(Application.GetApplication().GetWindow().GetNativeWindow() as GLFWWindow); // OPENGL: Remove

            ImGui.NewFrame();
            frameBegun = true;
        }

        public void End()
        {
            if (frameBegun)
            {
                frameBegun = false;

                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData());
            }
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
                        case Severity.Warn: col = new Vector4(251f/255f, 195f/255f, 42/255f, 1); break;
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
            //Logger.Info($"[IMGUI] {e.GetType().Name}: {e.ToString()}");

            EventDispatcher dispatcher = new EventDispatcher(e);

            dispatcher.Dispatch<WindowResizeEvent>(e, WindowResized);
            dispatcher.Dispatch<MouseMovedEvent>(e, MouseMoved);
            dispatcher.Dispatch<MouseButtonPressedEvent>(e, MouseButtonPressed);
            dispatcher.Dispatch<MouseButtonReleasedEvent>(e, MouseButtonReleased);
            dispatcher.Dispatch<MouseScrolledEvent>(e, MouseScrolled);
            dispatcher.Dispatch<KeyPressedEvent>(e, KeyPressed);
            dispatcher.Dispatch<KeyReleasedEvent>(e, KeyReleased);
            dispatcher.Dispatch<ConsoleLoggedEvent>(e, ConsoleLog);

            base.OnEvent(e);
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
            ImGui.GetIO().KeysDown[e.KeyCode] = false;
            return false;
        }

        private bool KeyPressed(KeyPressedEvent e)
        {
            ImGui.GetIO().KeysDown[e.KeyCode] = true;
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



    #region GL Specific Initialization

        public class ImGuiShader : Shader
        {
            public ImGuiShader(string fragmentPath, string vertexPath, ShaderSource source) : base(fragmentPath, vertexPath, source)
            {
            }

            protected override void GetUniformLocations()
            {
            }

            protected override void BindAttributes()
            {
            }
        }

        public abstract class Shader : IDisposable
        {
            private readonly string fragmentPath;
            private readonly string vertexPath;

            private int programID;
            private int vertexShaderID;
            private int fragmentShaderID;

            public int ProgramID => programID;
            public int VertexShaderID => vertexShaderID;
            public int FragmentShaderID => fragmentShaderID;

            protected Shader(string fragment, string vertex, ShaderSource source)
            {
                if (source == ShaderSource.Path)
                {
                    fragmentPath = fragment;
                    vertexPath = vertex;

                    BuildShaderFromFile();
                }
                else if (source == ShaderSource.Source)
                {
                    BuildShaderFromSource(fragment, vertex);
                }

            }

            public void Bind()
            {
                GL.UseProgram(ProgramID);
            }

            public void Unbind()
            {
                GL.UseProgram(0);
            }

            private void BuildShaderFromFile()
            {
                vertexShaderID = LoadShader(vertexPath, ShaderType.VertexShader);
                fragmentShaderID = LoadShader(fragmentPath, ShaderType.FragmentShader);

                programID = GL.CreateProgram();

                GL.AttachShader(programID, vertexShaderID);
                GL.AttachShader(programID, fragmentShaderID);

                BindAttributes();

                GL.LinkProgram(programID);
                GL.ValidateProgram(programID);

                GetUniformLocations();
            }

            private void BuildShaderFromSource(string fragment, string vertex)
            {
                vertexShaderID = CompileShader(vertex, ShaderType.VertexShader);
                fragmentShaderID = CompileShader(fragment, ShaderType.FragmentShader);

                programID = GL.CreateProgram();

                GL.AttachShader(programID, vertexShaderID);
                GL.AttachShader(programID, fragmentShaderID);

                BindAttributes();

                GL.LinkProgram(programID);
                GL.ValidateProgram(programID);

                GetUniformLocations();
            }

            private int CompileShader(string source, ShaderType shaderType)
            {
                int shaderID = GL.CreateShader(shaderType);
                GL.ShaderSource(shaderID, source);
                GL.CompileShader(shaderID);

                string log = GL.GetShaderInfoLog(shaderID);

                if (log != string.Empty)
                {
                    Logger.Error(Enum.GetName(typeof(ShaderType), shaderType) + ": " + log);
                }

                return shaderID;
            }

            protected abstract void GetUniformLocations();

            public int GetUniformLocation(string uniformName)
            {
                return GL.GetUniformLocation(programID, uniformName);
            }

            protected abstract void BindAttributes();

            protected void BindAttribute(int attributeIndex, string variableName)
            {
                GL.BindAttribLocation(programID, attributeIndex, variableName);
            }

            private int LoadShader(string shaderPath, ShaderType shaderType)
            {
                if (!File.Exists(shaderPath))
                    throw new FileNotFoundException("Shader not found at '" + shaderPath + "'");

                string shaderSource;

                using (StreamReader reader = new StreamReader(shaderPath, Encoding.UTF8))
                {
                    shaderSource = reader.ReadToEnd();
                }

                int shaderID = GL.CreateShader(shaderType);
                GL.ShaderSource(shaderID, shaderSource);
                GL.CompileShader(shaderID);

                string log = GL.GetShaderInfoLog(shaderID);

                if (log != string.Empty)
                {
                    Logger.Error(Enum.GetName(typeof(ShaderType), shaderType) + ": " + log);
                }

                return shaderID;
            }

            public void Dispose()
            {
                Unbind();

                GL.DetachShader(ProgramID, VertexShaderID);
                GL.DetachShader(ProgramID, FragmentShaderID);
                GL.DeleteShader(VertexShaderID);
                GL.DeleteShader(FragmentShaderID);
                GL.DeleteProgram(ProgramID);
            }

            #region Uniform Set Functions

            public void SetFloat(int location, float value)
            {
                GL.Uniform1(location, value);
            }

            public void SetBool(int location, bool value)
            {
                if (value)
                    GL.Uniform1(location, 1);
                else
                    GL.Uniform1(location, 0);
            }

            public void SetVector2(int location, Vector2 value)
            {
                GL.Uniform2(location, value);
            }

            public void SetVector3(int location, Vector3 value)
            {
                GL.Uniform3(location, value);
            }

            public void SetMatrix(int location, Matrix4 value)
            {
                GL.UniformMatrix4(location, false, ref value);
            }

            #endregion Uniform Set Functions
        }

        public class Texture
        {

            public readonly string Name;
            public readonly int ID;
            public readonly int Width, Height;

            public Texture(string name, int width, int height, IntPtr data)
            {
                Name = name;
                Width = width;
                Height = height;

                ID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, ID);
                GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba8, width, height);

                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }

            public void SetMinFilter(TextureMinFilter filter)
            {
                GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)filter);
            }

            public void SetMagFilter(TextureMagFilter filter)
            {
                GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)filter);
            }
        }

        public enum ShaderSource
        {
            Source,
            Path
        }

        private void CreateGLResources()
        {
            GL.CreateVertexArrays(1, out vertexArray);
            //vertexArray = GL.GenVertexArray();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, vertexArray);

            vertexBufferSize = 10000;
            indexBufferSize = 2000;

            GL.CreateBuffers(1, out vertexBuffer);
            GL.CreateBuffers(1, out indexBuffer);
            //vertexBuffer = GL.GenBuffer();
            //indexBuffer = GL.GenBuffer();

            //GL.BufferData(BufferTarget.ArrayBuffer, vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            //GL.BufferData(BufferTarget.ArrayBuffer, indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.NamedBufferData(vertexBuffer, vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.NamedBufferData(indexBuffer, indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            RecreateFontDeviceTexture();

            string VertexSource = @"

                #version 330 core
                uniform mat4 projection_matrix;
                layout(location = 0) in vec2 in_position;
                layout(location = 1) in vec2 in_texCoord;
                layout(location = 2) in vec4 in_color;
                out vec4 color;
                out vec2 texCoord;
                void main()
                {
                    gl_Position = projection_matrix * vec4(in_position, 0, 1);
                    color = in_color;
                    texCoord = in_texCoord;
                }
            ";

            string FragmentSource = @"

                #version 330 core
                uniform sampler2D in_fontTexture;
                in vec4 color;
                in vec2 texCoord;
                out vec4 outputColor;
                void main()
                {
                    outputColor = color * texture(in_fontTexture, texCoord);
                }
            ";

            shader = new ImGuiShader(FragmentSource, VertexSource, ShaderSource.Source);

            GL.VertexArrayVertexBuffer(vertexArray, 0, vertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
            GL.VertexArrayElementBuffer(vertexArray, indexBuffer);

            GL.EnableVertexArrayAttrib(vertexArray, 0);
            GL.VertexArrayAttribBinding(vertexArray, 0, 0);
            GL.VertexArrayAttribFormat(vertexArray, 0, 2, VertexAttribType.Float, false, 0);

            GL.EnableVertexArrayAttrib(vertexArray, 1);
            GL.VertexArrayAttribBinding(vertexArray, 1, 0);
            GL.VertexArrayAttribFormat(vertexArray, 1, 2, VertexAttribType.Float, false, 8);

            GL.EnableVertexArrayAttrib(vertexArray, 2);
            GL.VertexArrayAttribBinding(vertexArray, 2, 0);
            GL.VertexArrayAttribFormat(vertexArray, 2, 4, VertexAttribType.UnsignedByte, true, 16);
        }

        public void RecreateFontDeviceTexture()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out _);

            fontTexture = new Texture("ImGui Text Atlas", width, height, pixels);
            fontTexture.SetMagFilter(TextureMagFilter.Linear);
            fontTexture.SetMinFilter(TextureMinFilter.Linear);

            io.Fonts.SetTexID((IntPtr)fontTexture.ID);

            io.Fonts.ClearTexData();
        }

        private void SetPerFrameImGuiData(float deltaTime)
        {
            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(windowWidth / scaleFactor.X, windowHeight / scaleFactor.Y);
            io.DisplayFramebufferScale = scaleFactor;
            io.DeltaTime = deltaTime;
        }

        private void UpdateImGuiInput(GLFWWindow window)
        {
            ImGuiIOPtr io = ImGui.GetIO();

            MouseState mouseState = window.MouseState;
            KeyboardState keyboardState = window.KeyboardState;

            io.MouseDown[0] = mouseState.IsButtonDown(MouseButton.Left);
            io.MouseDown[1] = mouseState.IsButtonDown(MouseButton.Right);
            io.MouseDown[2] = mouseState.IsButtonDown(MouseButton.Middle);

            io.KeyCtrl = keyboardState.IsKeyDown(Key.ControlLeft) || keyboardState.IsKeyDown(Key.ControlRight);
            io.KeyAlt = keyboardState.IsKeyDown(Key.AltLeft) || keyboardState.IsKeyDown(Key.AltRight);
            io.KeyShift = keyboardState.IsKeyDown(Key.ShiftLeft) || keyboardState.IsKeyDown(Key.ShiftRight);
            io.KeySuper = keyboardState.IsKeyDown(Key.WinLeft) || keyboardState.IsKeyDown(Key.WinRight);

            PrevMouseState = mouseState;
            PrevKeyboardState = keyboardState;
        }

        private void RenderImDrawData(ImDrawDataPtr data)
        {
            if (data.CmdListsCount == 0) return;

            for (int i = 0; i < data.CmdListsCount; i++)
            {
                ImDrawListPtr cmd_list = data.CmdListsRange[i];

                int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if (vertexSize > vertexBufferSize)
                {
                    int newSize = (int)Math.Math.Max(vertexBufferSize * 1.5f, vertexSize);
                    GL.NamedBufferData(vertexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    vertexBufferSize = newSize;
                }

                int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
                if (indexSize > indexBufferSize)
                {
                    int newSize = (int)Math.Math.Max(indexBufferSize * 1.5f, indexSize);
                    GL.NamedBufferData(indexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    indexBufferSize = newSize;
                }
            }

            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();
            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(
                0.0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            shader.Bind();
            GL.UniformMatrix4(shader.GetUniformLocation("projection_matrix"), false, ref mvp);
            GL.Uniform1(shader.GetUniformLocation("in_fontTexture"), 0);

            GL.BindVertexArray(vertexArray);

            data.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            // Render command lists
            for (int n = 0; n < data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = data.CmdListsRange[n];

                GL.NamedBufferSubData(vertexBuffer, IntPtr.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);

                GL.NamedBufferSubData(indexBuffer, IntPtr.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

                int vtx_offset = 0;
                int idx_offset = 0;

                for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);

                    // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                    var clip = pcmd.ClipRect;
                    GL.Scissor((int)clip.X, windowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
                    //GL.Scissor(0, 0, windowWidth, windowHeight);

                    if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                    {
                        GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(idx_offset * sizeof(ushort)), vtx_offset);
                    }
                    else
                    {
                        GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                    }

                    idx_offset += (int)pcmd.ElemCount;
                }
            }

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);

            GL.BindVertexArray(0);
        }

        private void SetKeyMappings()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int) ImGuiKey.Space] = (int) Key.Space;
            io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.BackSpace;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;
        }

        #endregion
    }
}