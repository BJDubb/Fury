using OpenTK.Graphics.OpenGL4;

using System;

namespace Fury.Rendering
{
    public class VertexBuffer
    {
        int id;
        public int Count;
        public int Size;
        public BufferLayout Layout;

        public VertexBuffer(int size)
        {
            id = GL.GenBuffer();
            Size = size;
            this.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public VertexBuffer(float[] verticies, int size)
        {
            id = GL.GenBuffer();
            Count = verticies.Length;
            Size = size;
            this.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, size, verticies, BufferUsageHint.StaticDraw);
        }

        ~VertexBuffer()
        {
            GL.DeleteBuffer(this);
        }

        public void SetData(IntPtr data, int size)
        {
            this.Bind();
            Size += size;
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, size, data);
        }

        public void SetLayout(BufferLayout layout)
        {
            Layout = layout;
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public static implicit operator int(VertexBuffer b) => b.id;
    }
}
