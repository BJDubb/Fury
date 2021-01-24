using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System;
using System.Collections.Generic;
using System.IO;

namespace Fury.Rendering
{
    public class Shader
    {
        public string VertexPath { get; }
        public string FragmentPath { get; }

        public int VertexID { get; }
        public int FragmentID { get; }

        public int ProgramID { get; }

        #region Constructor

        public Shader(string vertexPath, string fragmentPath, bool fromFile)
        {
            VertexPath = vertexPath;
            FragmentPath = fragmentPath;


            // Shader Creation
            if (fromFile)
            {
                VertexID = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(VertexID, File.ReadAllText(vertexPath));

                FragmentID = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(FragmentID, File.ReadAllText(fragmentPath));
            }
            else
            {
                VertexID = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(VertexID, vertexPath);

                FragmentID = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(FragmentID, fragmentPath);
            }

            // Shader Compilation
            GL.CompileShader(VertexID);

            var vertexLog = GL.GetShaderInfoLog(VertexID);
            if (vertexLog != "") Console.WriteLine("Vertex: " + vertexLog);

            GL.CompileShader(FragmentID);

            var fragmentLog = GL.GetShaderInfoLog(FragmentID);
            if (fragmentLog != "") Console.WriteLine("Fragment: " + fragmentLog);

            // Shader Program Creation
            ProgramID = GL.CreateProgram();

            GL.AttachShader(this, VertexID);
            GL.AttachShader(this, FragmentID);

            GL.LinkProgram(this);

            // Clean up
            GL.DetachShader(this, VertexID);
            GL.DetachShader(this, FragmentID);
            GL.DeleteShader(VertexID);
            GL.DeleteShader(FragmentID);
        }

        #endregion

        ~Shader()
        {
            GL.DeleteProgram(this);
        }

        public void Bind()
        {
            GL.UseProgram(this);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        Dictionary<string, int> uniformLocations = new Dictionary<string, int>(); 

        public int GetUniformLocation(string uniform)
        {
            if (!uniformLocations.ContainsKey(uniform))
            {
                uniformLocations[uniform] = GL.GetUniformLocation(this, uniform);
            }
            return uniformLocations[uniform];
        }

        public void SetInt(string name, int data)
        {
            Bind();

            if (!uniformLocations.ContainsKey(name))
            {
                uniformLocations[name] = GL.GetUniformLocation(this, name);
            }

            GL.Uniform1(uniformLocations[name], data);
        }

        public void SetMatrix4(string name, bool transpose, ref Matrix4 data)
        {
            Bind();
            GL.UniformMatrix4(GetUniformLocation(name), transpose, ref data);
        }

        public static implicit operator int(Shader s) => s.ProgramID;

    }
}
