using System;
using System.Collections.Generic;
using System.Text;

namespace Fury.Math
{

    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public static Vector3 Zero => new Vector3(0);
        public static Vector3 One => new Vector3(1);

        public Vector3(float value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator System.Numerics.Vector3(Vector3 vector)
        {
            return new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
        }

        public static implicit operator OpenToolkit.Mathematics.Vector3(Vector3 vector)
        {
            return new OpenToolkit.Mathematics.Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}
