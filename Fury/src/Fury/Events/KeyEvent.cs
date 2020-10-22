using System.Text;

namespace Fury.Events
{
    public abstract class KeyEvent : Event
    {
        protected int keyCode;
        protected bool ctrl, alt, shift;

        protected KeyEvent(int keyCode, bool ctrl, bool alt, bool shift)
        {
            this.keyCode = keyCode;
            this.ctrl = ctrl;
            this.alt = alt;
            this.shift = shift;
        }

        public int KeyCode => keyCode;
        public bool Control => ctrl;
        public bool Alt => alt;
        public bool Shift => shift;
    }

    public class KeyPressedEvent : KeyEvent
    {
        public KeyPressedEvent(int keyCode, bool ctrl, bool alt, bool shift) : base(keyCode, ctrl, alt, shift) { }
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
        public KeyReleasedEvent(int keyCode, bool ctrl, bool alt, bool shift) : base(keyCode, ctrl, alt, shift) { }
        public override EventType GetEventType()
        {
            return EventType.KeyReleased;
        }

        public override string ToString()
        {
            return "KeyReleasedEvent: " + keyCode;
        }
    }

    public class TextInputEvent : Event
    {
        private int unicode;

        public TextInputEvent(int unicode)
        {
            this.unicode = unicode;
        }

        public override EventType GetEventType()
        {
            return EventType.TextInput;
        }

        public override string ToString()
        {
            return $"TextInputEvent: {unicode}({(char)unicode})";
        }

        public int Unicode => unicode;
    }
}