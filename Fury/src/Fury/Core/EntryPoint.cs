namespace Fury.Core
{
    public class EntryPoint
    {
        public static Application app;

        public static Application CreateApplication(Application application)
        {
            app = application;
            return app;
        }
    }
}
