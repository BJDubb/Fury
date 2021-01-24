using OpenTK.Graphics.OpenGL4;

using System;

namespace Fury.Rendering
{
    public abstract class Texture
    {
        protected int id;
        protected string path;

        public abstract int GetWidth();
        public abstract int GetHeight();

        public Texture(string path)
        {
            this.path = path;
        }

        public abstract void Bind(int slot = 0);


        public static implicit operator int(Texture t) => t.id;
    }
}
