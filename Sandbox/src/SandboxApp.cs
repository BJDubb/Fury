using System.Buffers.Text;
using Fury;

namespace Sandbox
{
    public class Sandbox : Application
    {
        public Sandbox()
        {
            PushLayer(new Layer());
        }

        static void Main(string[] args)
        {
            EntryPoint.CreateApplication(new Sandbox());
        }
    }


}
