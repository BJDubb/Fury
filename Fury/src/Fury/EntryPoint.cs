using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Fury
{
    public class EntryPoint
    {

#if FURY_PLATFORM_WINDOWS

        public static void CreateApplication(Application application)
        {
            var app = application;
            app.Run();
        }

#endif

//#elif FURY_PLATFORM_OTHER

//#endif
    }
}
