using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

using System;
using System.IO;

namespace Fury.Utils
{
    public static class Extensions
    {
        public static System.Numerics.Vector3 ToVec3(this OpenTK.Mathematics.Vector3 v)
        {
            return new System.Numerics.Vector3(v.X, v.Y, v.Z);
        }

        public static OpenTK.Mathematics.Vector3 ToVec3(this System.Numerics.Vector3 v)
        {
            return new OpenTK.Mathematics.Vector3(v.X, v.Y, v.Z);
        }

        public static System.Numerics.Vector2 ToVec2(this OpenTK.Mathematics.Vector2 v)
        {
            return new System.Numerics.Vector2(v.X, v.Y);
        }

        public static OpenTK.Mathematics.Vector2 ToVec2(this System.Numerics.Vector2 v)
        {
            return new OpenTK.Mathematics.Vector2(v.X, v.Y);
        }

        public static System.Numerics.Vector4 ToVec4(this System.Drawing.Color color)
        {
            return new System.Numerics.Vector4(color.R, color.G, color.B, color.A);
        }

        public static System.Drawing.Color ToColor(this System.Numerics.Vector3 color)
        {
            return System.Drawing.Color.FromArgb(255, (int)color.X, (int)color.Y, (int)color.Z);
        }

        public static System.Drawing.Color ToColor(this System.Numerics.Vector4 color)
        {
            return System.Drawing.Color.FromArgb((int)color.W, (int)color.X, (int)color.Y, (int)color.Z);
        }
    }
}
