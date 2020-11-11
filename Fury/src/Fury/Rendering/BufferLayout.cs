using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fury.Rendering
{
    public class BufferLayout : IEnumerable
    {
        public int Stride = 0;

        private List<BufferElement> bufferElements;
        public List<BufferElement> GetBufferElements() => bufferElements;

        public IEnumerator GetEnumerator() => bufferElements.GetEnumerator();

        public BufferLayout(params BufferElement[] elements)
        {
            bufferElements = elements.ToList();
            CalculateOffsetAndStride();
        }

        public void CalculateOffsetAndStride()
        {
            int offset = 0;
            Stride = 0;
            for (int i = 0; i < bufferElements.Count; i++)
            {
                BufferElement element = bufferElements[i];
                element.Offset = offset;
                offset += element.Size;
                Stride += element.Size;
                bufferElements[i] = element;
            }
        }
    }

    public struct BufferElement
    {
        public string Name;
        public ShaderDataType Type;
        public int Size;
        public bool Normalized;
        public int Offset;

        public BufferElement(string name, ShaderDataType type, bool normalized = false)
        {
            Name = name;
            Type = type;
            Size = ShaderDataTypeSize(type);
            Normalized = normalized;
            Offset = 0;
        }

        public int GetComponentCount()
        {
            switch (Type)
            {
                case ShaderDataType.Float:
                    return 1;
                case ShaderDataType.Float2:
                    return 2;
                case ShaderDataType.Float3:
                    return 3;
                case ShaderDataType.Float4:
                    return 4;
                case ShaderDataType.Mat3:
                    return 3;
                case ShaderDataType.Mat4:
                    return 4;
                case ShaderDataType.Int:
                    return 1;
                case ShaderDataType.Int2:
                    return 2;
                case ShaderDataType.Int3:
                    return 3;
                case ShaderDataType.Int4:
                    return 4;
                case ShaderDataType.Bool:
                    return 1;
                case ShaderDataType.UnsignedByte:
                    return 1;
                case ShaderDataType.UnsignedByte4:
                    return 4;
                default:
                    throw new Exception("Component is invalid");
            }
        }

        public static int ShaderDataTypeSize(ShaderDataType type)
        {
            switch (type)
            {
                case ShaderDataType.None:
                    return 0;
                case ShaderDataType.Float:
                    return 4;
                case ShaderDataType.Float2:
                    return 8;
                case ShaderDataType.Float3:
                    return 12;
                case ShaderDataType.Float4:
                    return 16;
                case ShaderDataType.Mat3:
                    return 9;
                case ShaderDataType.Mat4:
                    return 32;
                case ShaderDataType.Int:
                    return 4;
                case ShaderDataType.Int2:
                    return 8;
                case ShaderDataType.Int3:
                    return 12;
                case ShaderDataType.Int4:
                    return 16;
                case ShaderDataType.Bool:
                    return 1;
                case ShaderDataType.UnsignedByte:
                    return 1;
                case ShaderDataType.UnsignedByte4:
                    return 4;
                default:
                    return 0;
            }
        }
    }

    public enum ShaderDataType
    {
        None = 0,
        Float,
        Float2,
        Float3,
        Float4,
        Mat3,
        Mat4,
        Int,
        Int2,
        Int3,
        Int4,
        Bool,
        UnsignedByte,
        UnsignedByte4
    }
}
