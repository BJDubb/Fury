namespace Fury
{
    public class EntryPoint
    {
        public static Application app;

        public static void CreateApplication(Application application)
        {
            app = application;
            app.Run();
        }
    }
}
