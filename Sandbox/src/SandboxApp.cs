using System.Buffers.Text;
using Fury.Core;

namespace Sandbox
{
    public class Sandbox : Application
    {
        public Sandbox()
        {
            PushLayer(new EditorLayer());
        }

        static void Main(string[] args)
        {
            EntryPoint.CreateApplication(new Sandbox());
        }
    }


}
