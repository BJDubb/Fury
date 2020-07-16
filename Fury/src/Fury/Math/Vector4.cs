using System;
using System.Collections.Generic;
using System.Text;

namespace Fury.Math
{
    public class Vector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public static Vector4 Zero => new Vector4(0);
        public static Vector4 One => new Vector4(1);

        public Vector4(float value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static implicit operator System.Numerics.Vector4(Vector4 vector)
        {
            return new System.Numerics.Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }

        public static implicit operator OpenToolkit.Mathematics.Vector4(Vector4 vector)
        {
            return new OpenToolkit.Mathematics.Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }
    }
}
