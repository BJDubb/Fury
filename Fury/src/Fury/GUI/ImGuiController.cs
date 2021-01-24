using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Fury.Utils;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.InteropServices;
using Fury.Rendering;

namespace Fury.GUI
{
    public static class ImGuiController
    {
        private static IWindow window;
        private static bool frameBegun;
        private static Application app;

        private static Texture fontTexture;
        private static Shader shader;

        private static VertexArray vertexArray;
        private static VertexBuffer vertexBuffer;
        private static IndexBuffer indexBuffer;

        private static int vertexBufferSize;
        private static int indexBufferSize;

        public static void Init(IWindow wnd)
        {
            window = wnd;

            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            var io = ImGui.GetIO();
            var font = io.Fonts.AddFontFromFileTTF(@"C:\Windows\Fonts\segoeui.ttf", 18);
            io.Fonts.AddFontDefault(font.ConfigData);

            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            io.ConfigWindowsMoveFromTitleBarOnly = true;
            io.ConfigWindowsResizeFromEdges = true;

            CreateDeviceResources();
            SetKeyMappings();

            app = Application.GetApplication();

            SetPerFrameImGuiData(0, window);

            ImGui.NewFrame();
            frameBegun = true;
        }



        private static void CreateDeviceResources()
        {
            vertexArray = new VertexArray();

            vertexBufferSize = 10000;
            indexBufferSize = 2000;

            //GL.CreateBuffers(1, out vertexBuffer);

            vertexBuffer = new VertexBuffer(vertexBufferSize);

            indexBuffer = new IndexBuffer(indexBufferSize);

            //GL.CreateBuffers(1, out indexBuffer);

            //GL.NamedBufferData(vertexBuffer, vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            //GL.NamedBufferData(indexBuffer, indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

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

            shader = new Shader(VertexSource, FragmentSource, false);

            // Old Code

            //GL.VertexArrayVertexBuffer(vertexArray, 0, vertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
            //GL.VertexArrayElementBuffer(vertexArray, indexBuffer);

            //GL.EnableVertexArrayAttrib(vertexArray, 0);
            //GL.VertexArrayAttribBinding(vertexArray, 0, 0);
            //GL.VertexArrayAttribFormat(vertexArray, 0, 2, VertexAttribType.Float, false, 0);

            //GL.EnableVertexArrayAttrib(vertexArray, 1);
            //GL.VertexArrayAttribBinding(vertexArray, 1, 0);
            //GL.VertexArrayAttribFormat(vertexArray, 1, 2, VertexAttribType.Float, false, 8);

            //GL.EnableVertexArrayAttrib(vertexArray, 2);
            //GL.VertexArrayAttribBinding(vertexArray, 2, 0);
            //GL.VertexArrayAttribFormat(vertexArray, 2, 4, VertexAttribType.UnsignedByte, true, 16);


            // New Code

            vertexArray.Bind();

            //vertexBuffer.Bind();
            //indexBuffer.Bind();

            BufferLayout layout = new BufferLayout(
                    new BufferElement("position", ShaderDataType.Float2, false),
                    new BufferElement("texCoords", ShaderDataType.Float2, false),
                    new BufferElement("color", ShaderDataType.UnsignedByte4, true)
                );

            vertexBuffer.SetLayout(layout);

            vertexArray.AddVertexBuffer(vertexBuffer);
            vertexArray.SetIndexBuffer(indexBuffer);

            //GL.EnableVertexAttribArray(0);
            //GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<ImDrawVert>(), 0);
            //
            //GL.EnableVertexAttribArray(1);
            //GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<ImDrawVert>(), 8);
            //
            //GL.EnableVertexAttribArray(2);
            //GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, Unsafe.SizeOf<ImDrawVert>(), 16);
        }

        public static ImFontPtr LoadFontFromTTF(string path, float size)
        {
            var font = ImGui.GetIO().Fonts.AddFontFromFileTTF(path, size);
            RecreateFontDeviceTexture();

            return font;
        }

        public static void RecreateFontDeviceTexture()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            fontTexture = new Texture("ImGui Text Atlas", width, height, pixels);
            fontTexture.SetMagFilter(TextureMagFilter.Linear);
            fontTexture.SetMinFilter(TextureMinFilter.Linear);

            io.Fonts.SetTexID((IntPtr)fontTexture.ID);

            io.Fonts.ClearTexData();
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
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filter);
            }

            public void SetMagFilter(TextureMagFilter filter)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)filter);
            }
        }

        private static void SetKeyMappings()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Space] = (int)Keys.Space;
            io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
            io.KeyMap[(int)ImGuiKey.KeyPadEnter] = (int)Keys.KeyPadEnter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
        }

        private static void SetPerFrameImGuiData(float deltaTime, IWindow window)
        {
            var io = ImGui.GetIO();
            io.DisplaySize = new Vector2(window.Width / 1, window.Height/ 1).ToVec2();
            io.DisplayFramebufferScale = new Vector2(1).ToVec2();
            io.DeltaTime = deltaTime;
        }

        public static void Begin(IWindow window)
        {
            if (frameBegun)
                ImGui.Render();

            SetPerFrameImGuiData(Time.deltaTime, window);

            ImGui.NewFrame();
            frameBegun = true;
        }

        public static void End()
        {
            if (frameBegun)
            {
                frameBegun = false;

                ImGui.Render();
                
                RenderImDrawData(ImGui.GetDrawData());

                var cursor = ImGui.GetMouseCursor();
                switch (cursor)
                {
                    case ImGuiMouseCursor.Arrow:
                        ((WindowsWindow)app.GetWindow().GetNativeWindow()).Cursor = MouseCursor.Default;
                        break;
                    case ImGuiMouseCursor.TextInput:
                        ((WindowsWindow)app.GetWindow().GetNativeWindow()).Cursor = MouseCursor.IBeam;
                        break;
                    case ImGuiMouseCursor.ResizeNS:
                        ((WindowsWindow)app.GetWindow().GetNativeWindow()).Cursor = MouseCursor.VResize;
                        break;
                    case ImGuiMouseCursor.ResizeEW:
                        ((WindowsWindow)app.GetWindow().GetNativeWindow()).Cursor = MouseCursor.HResize;
                        break;
                    case ImGuiMouseCursor.Hand:
                        ((WindowsWindow)app.GetWindow().GetNativeWindow()).Cursor = MouseCursor.Hand;
                        break;
                    default:
                        ((WindowsWindow)app.GetWindow().GetNativeWindow()).Cursor = MouseCursor.Default;
                        break;
                }
            }
        }

        private static void RenderImDrawData(ImDrawDataPtr data)
        {
            if (data.CmdListsCount == 0) return;

            for (int i = 0; i < data.CmdListsCount; i++)
            {
                ImDrawListPtr cmd_list = data.CmdListsRange[i];

                int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if (vertexSize > vertexBufferSize)
                {
                    int newSize = (int)MathHelper.Max(vertexBufferSize * 1.5f, vertexSize);

                    // New Code

                    vertexBuffer.SetData(IntPtr.Zero, newSize);

                    //GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                    //GL.BufferData(BufferTarget.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                    //

                    //GL.NamedBufferData(vertexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    vertexBufferSize = newSize;
                }

                int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
                if (indexSize > indexBufferSize)
                {
                    int newSize = (int)MathHelper.Max(indexBufferSize * 1.5f, indexSize);

                    // New Code

                    indexBuffer.SetData(IntPtr.Zero, newSize);

                    //GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                    //GL.BufferData(BufferTarget.ElementArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                    //

                    //GL.NamedBufferData(indexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    indexBufferSize = newSize;
                }
            }

            GL.Viewport(0, 0, (int)data.DisplaySize.X, (int)data.DisplaySize.Y);

            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();
            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(
                data.DisplayPos.X,
                data.DisplayPos.X + data.DisplaySize.X,
                data.DisplayPos.Y + data.DisplaySize.Y,
                data.DisplayPos.Y,
                -1.0f,
                1.0f);

            shader.Bind();
            shader.SetMatrix4("projection_matrix", false, ref mvp);
            shader.SetInt("in_fontTexture", 0);

            vertexArray.Bind();

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

                // New Code

                vertexBuffer.SetSubData(cmd_list.VtxBuffer.Data, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>());
                indexBuffer.SetData(cmd_list.IdxBuffer.Data, cmd_list.IdxBuffer.Size * sizeof(ushort));

                //GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                //GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);
                //
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                //GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

                //

                //GL.NamedBufferSubData(vertexBuffer, IntPtr.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);
                //GL.NamedBufferSubData(indexBuffer, IntPtr.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

                int vtx_offset = 0;
                int idx_offset = 0;

                for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);

                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, window.Height - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

                        if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                        {
                            GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(idx_offset * sizeof(ushort)), vtx_offset);
                        }
                        else
                        {
                            GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                        }
                    }

                    idx_offset += (int)pcmd.ElemCount;
                }
            }

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);

            vertexArray.Unbind();
        }
    }
}
