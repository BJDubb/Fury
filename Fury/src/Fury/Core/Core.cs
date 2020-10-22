using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Fury.Core
{
    public class Core
    {
        public static Platform platform;

        public static void GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) platform = Platform.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) platform = Platform.OSX;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) platform = Platform.Linux;
            if (platform == 0) throw new Exception("Runtime Platform is Undefined");
        }
        public enum Platform
        {
            Windows = 1,
            OSX,
            Linux
        }
    }
}
