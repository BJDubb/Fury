using System;
using System.Collections.Generic;
using System.Text;

namespace Fury.Math
{
    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static Vector2 Zero => new Vector2(0);
        public static Vector2 One => new Vector2(1);

        public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator System.Numerics.Vector2(Vector2 vector)
        {
            return new System.Numerics.Vector2(vector.X, vector.Y);
        }

        public static implicit operator OpenToolkit.Mathematics.Vector2(Vector2 vector)
        {
            return new OpenToolkit.Mathematics.Vector2(vector.X, vector.Y);
        }
    }
}
