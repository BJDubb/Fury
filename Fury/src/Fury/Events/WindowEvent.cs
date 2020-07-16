namespace Fury.Events
{
    public abstract class WindowEvent : Event { }

    public class WindowCreatedEvent : WindowEvent
    {
        protected int width;
        protected int height;

        public WindowCreatedEvent(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override EventType GetEventType()
        {
            return EventType.WindowCreated;
        }

        public override string ToString()
        {
            return $"WindowResized {width}, {height}";
        }
        public int Width => width;
        public int Height => height;
    }

    public class WindowResizeEvent : WindowEvent
    {
        protected int width;
        protected int height;

        public WindowResizeEvent(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override EventType GetEventType()
        {
            return EventType.WindowResized;
        }

        public override string ToString()
        {
            return $"WindowResized {width}, {height}";
        }
        public int Width => width;
        public int Height => height;
    }

    public class WindowCloseEvent : WindowEvent
    {
        public override EventType GetEventType()
        {
            return EventType.WindowResized;
        }

        public override string ToString()
        {
            return $"WindowClosed";
        }
    }
}
