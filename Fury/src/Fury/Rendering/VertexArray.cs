using OpenTK.Graphics.OpenGL4;

using System;

namespace Fury.Rendering
{
    public class VertexArray
    {
        int id;
        IndexBuffer indexBuffer;

        int attribIndex = 0;

        public VertexArray()
        {
            id = GL.GenVertexArray();
        }

        ~VertexArray()
        {
            GL.DeleteVertexArray(this);
        }

        public void AddVertexBuffer(VertexBuffer vertexBuffer)
        {
            this.Bind();
            vertexBuffer.Bind();

            var layout = vertexBuffer.Layout;

            foreach (var element in layout.GetBufferElements())
            {
                GL.EnableVertexAttribArray(attribIndex);
                GL.VertexAttribPointer(attribIndex, element.GetComponentCount(), GLTypeFromLayout(element.Type), element.Normalized, layout.Stride, element.Offset);
                attribIndex++;
            }
        }

        public void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            this.Bind();
            indexBuffer.Bind();
            this.indexBuffer = indexBuffer;
        }

        public IndexBuffer GetIndexBuffer() => indexBuffer;

        public void Bind()
        {
            GL.BindVertexArray(this);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public static implicit operator int(VertexArray v) => v.id;

        public static VertexAttribPointerType GLTypeFromLayout(ShaderDataType type)
        {
            switch (type)
            {
                case ShaderDataType.None:
                    throw new Exception("ShaderDataType is none in vertex array.");
                case ShaderDataType.Float:
                case ShaderDataType.Float2:
                case ShaderDataType.Float3:
                case ShaderDataType.Float4:
                case ShaderDataType.Mat3:
                case ShaderDataType.Mat4:
                    return VertexAttribPointerType.Float;
                case ShaderDataType.Int:
                case ShaderDataType.Int2:
                case ShaderDataType.Int3:
                case ShaderDataType.Int4:
                    return VertexAttribPointerType.Int;
                case ShaderDataType.UnsignedByte:
                case ShaderDataType.UnsignedByte4:
                    return VertexAttribPointerType.UnsignedByte;
                case ShaderDataType.Bool:
                    throw new Exception("ShaderDataType is bool in vertex array.");

                default:
                    throw new Exception("ShaderDataType is null in vertex array.");
            }
        }
    }
}
