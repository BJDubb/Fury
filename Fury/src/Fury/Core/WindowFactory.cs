using Fury.Utils;

using ImGuiNET;

using System;
using System.Runtime.InteropServices;

namespace Fury
{
    public class WindowFactory
    {
        public static IWindow CreateWindow()
        {
            switch (Core.platform)
            {
                case Core.Platform.Windows:
                    return new WindowsWindow();

                case Core.Platform.OSX:
                    return new MacOSWindow();

                case Core.Platform.Linux:
                    Logger.Error("Linux not implemented");
                    return null;

                default:
                    Logger.Error("Runtime Platform is Undefined");
                    return null;
            }
        }
    }
}