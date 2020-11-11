using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System;
using System.Drawing;

namespace Fury.Rendering
{
    public class Renderer
    {
        static Matrix4 viewProjectionMatrix;
        static System.Numerics.Vector4 clearColor;

        public static void Init()
        {
            GL.Enable(EnableCap.DepthTest);
        }

        public static void BeginScene(ref Camera camera)
        {
            viewProjectionMatrix = camera.ViewProjectionMatrix;
        }

        public static void Submit(VertexArray vertexArray, Shader shader, Matrix4 transform)
        {
            shader.Bind();
            shader.SetMatrix4("uViewProjection", true, ref viewProjectionMatrix);
            shader.SetMatrix4("uTransform", true, ref transform);

            var indexBuffer = vertexArray.GetIndexBuffer();

            DrawIndexed(vertexArray, indexBuffer);
        }

        private static void DrawIndexed(VertexArray vertexArray, IndexBuffer indexBuffer)
        {
            vertexArray.Bind();
            indexBuffer.Bind();

            GL.DrawElements(PrimitiveType.Triangles, indexBuffer.GetCount(), DrawElementsType.UnsignedInt, IntPtr.Zero);
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
