using Fury;

using System;
using System.Threading;

namespace FuryEditor
{
    public class FuryEditor : Application
    {

        static void Main(string[] args)
        {
            var app = EntryPoint.CreateApplication(new FuryEditor());
            app.PushLayer(new EditorLayer());
            app.Run();
        }
    }


}
