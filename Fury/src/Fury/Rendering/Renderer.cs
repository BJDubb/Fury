using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System;
using System.Drawing;

namespace Fury.Rendering
{
    public class Renderer
    {
        static Matrix4 viewMatrix;
        static Matrix4 projectionMatrix;
        static System.Numerics.Vector4 clearColor;

        public static void Init()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);
            GL.Enable(EnableCap.Blend);
        }

        public static void BeginScene(Camera camera)
        {
            viewMatrix = camera.ViewMatrix;
            projectionMatrix = camera.ProjectionMatrix;
        }

        public static void Submit(VertexArray vertexArray, Shader shader, Matrix4 transform)
        {
            shader.Bind();
            shader.SetMatrix4("uView", true, ref viewMatrix);
            shader.SetMatrix4("uProjection", true, ref projectionMatrix);
            shader.SetMatrix4("uTransform", true, ref transform);

            var indexBuffer = vertexArray.GetIndexBuffer();

            DrawIndexed(vertexArray, indexBuffer);
        }

        private static void DrawIndexed(VertexArray vertexArray, IndexBuffer indexBuffer)
        {
            vertexArray.Bind();
            indexBuffer.Bind();

            GL.DrawElements(PrimitiveType.Triangles, indexBuffer.GetCount(), DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void EndScene()
        {

        }

        public static void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public static void SetClearColor(System.Numerics.Vector4 color)
        {
            clearColor = color;
            GL.ClearColor(color.X, color.Y, color.Z, color.W);
        }

        public static System.Numerics.Vector4 GetClearColor()
        {
            return clearColor;
        }

        public static void Resize(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }

        
    }
}
