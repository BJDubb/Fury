﻿using Fury;

namespace Sandbox
{
    public class Sandbox : Application
    {
        static void Main(string[] args)
        {
            var app = EntryPoint.CreateApplication(new Sandbox());
            app.PushLayer(new EditorLayer());
            app.Run();
        }
    }


}
