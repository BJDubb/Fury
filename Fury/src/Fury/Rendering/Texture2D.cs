using OpenTK.Graphics.OpenGL4;


using System.Diagnostics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using SixLabors.ImageSharp.Formats.Png;
using System.Collections.Generic;
using System;

namespace Fury.Rendering
{
    public class Texture2D : Texture
    {
        int width, height;

        public Texture2D(string path) : base(path)
        {
            this.path = path;

            List<byte> data = new List<byte>();

            using (Image<Rgba32> image = Image.Load<Rgba32>(path))
            {
                width = image.Width;
                height = image.Height;

                int bits = image.PixelType.BitsPerPixel;

                image.Mutate(x => x.Flip(FlipMode.Vertical));

                for (int y = 0; y < image.Height; y++)
                {
                    Span<Rgba32> row = image.GetPixelRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        data.Add(row[x].R);
                        data.Add(row[x].G);
                        data.Add(row[x].B);
                        if (bits == 32) data.Add(row[x].A);
                    }
                }

                GL.CreateTextures(TextureTarget.Texture2D, 1, out id);
                GL.TextureStorage2D(id, 1, SizedInternalFormat.Rgba8, width, height);

                GL.TextureSubImage2D(this, 0, 0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, data.ToArray());
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        ~Texture2D()
        {
            GL.DeleteTexture(this);
        }

        public override void Bind(int slot = 0)
        {
            GL.BindTextureUnit(slot, this);
        }

        public override int GetHeight()
        {
            return height;
        }

        public override int GetWidth()
        {
            return width;
        }
    }
}
