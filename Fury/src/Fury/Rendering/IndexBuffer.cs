using OpenTK.Graphics.OpenGL4;

using System;

namespace Fury.Rendering
{
    public class IndexBuffer
    {
        int id;
        int count;

        public IndexBuffer(uint[] indices, int size)
        {
            count = indices.Length;
            id = GL.GenBuffer();
            this.Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, size, indices, BufferUsageHint.StaticDraw);
        }

        public IndexBuffer(int size)
        {
            id = GL.GenBuffer();
            this.Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        ~IndexBuffer()
        {
            GL.DeleteBuffer(this);
        }

        public int GetCount() => count;

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void SetData(IntPtr data, int size)
        {
            this.Bind();
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, size, data);
        }

        public static implicit operator int(IndexBuffer i) => i.id;

    }
}
