using Fury.Utils;

using OpenTK.Graphics.OpenGL4;

using System;

namespace Fury.Rendering
{
    public class Framebuffer
    {
        public FramebufferData FramebufferData;

        private int id = 0;
        public int colorAttachment = 0;
        public int depthAttachment = 0;


        public Framebuffer(FramebufferData data)
        {
            FramebufferData = data;
            Invalidate();
        }

        ~Framebuffer()
        {
            GL.DeleteFramebuffer(id);
            GL.DeleteTextures(1, ref colorAttachment);
            GL.DeleteTextures(1, ref depthAttachment);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);
            GL.Viewport(0, 0, FramebufferData.Width, FramebufferData.Height);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Resize(int width, int height)
        {
            FramebufferData.Width = width;
            FramebufferData.Height = height;
            Invalidate();
        }

        public void Invalidate()
        {
            if (id != 0)
            {
                GL.DeleteFramebuffer(id);
                GL.DeleteTextures(1, ref colorAttachment);
                GL.DeleteTextures(1, ref depthAttachment);
            }

            GL.CreateFramebuffers(1, out id);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out colorAttachment);
            GL.BindTexture(TextureTarget.Texture2D, colorAttachment);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, FramebufferData.Width, FramebufferData.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colorAttachment, 0);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out depthAttachment);
            GL.BindTexture(TextureTarget.Texture2D, depthAttachment);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, FramebufferData.Width, FramebufferData.Height, 0, PixelFormat.DepthStencil, PixelType.UnsignedInt248, IntPtr.Zero);
            //GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat., FramebufferData.Width, FramebufferData.Height);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, depthAttachment, 0);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete) Logger.Error("Framebuffer is incomplete");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }

    public struct FramebufferData
    {
        public int Width;
        public int Height;
        public bool SwapChainTarget;

        public FramebufferData(int width, int height, bool swapChainTarget = false)
        {
            Width = width;
            Height = height;
            SwapChainTarget = swapChainTarget;
        }
    }
}
