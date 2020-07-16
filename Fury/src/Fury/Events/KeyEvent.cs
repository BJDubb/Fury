namespace Fury.Events
{
    public abstract class KeyEvent : Event
    {
        protected int keyCode;

        protected KeyEvent(int keyCode)
        {
            this.keyCode = keyCode;
        }

        public int KeyCode => keyCode;
    }

    public class KeyPressedEvent : KeyEvent
    {
        public KeyPressedEvent(int keyCode) : base(keyCode) { }
        public override EventType GetEventType()
        {
            return EventType.KeyPressed;
        }

        public override string ToString()
        {
            return "KeyPressedEvent: " + keyCode;
        }
    }

    public class KeyReleasedEvent : KeyEvent
    {
        public KeyReleasedEvent(int keyCode) : base(keyCode) { }
        public override EventType GetEventType()
        {
            return EventType.KeyReleased;
        }

        public override string ToString()
        {
            return "KeyReleasedEvent: " + keyCode;
        }
    }
}