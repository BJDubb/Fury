using System;
using System.Collections.Generic;
using System.Text;

namespace Fury.Events
{
    public abstract class Event
    {
        public bool Handled = false;

        public enum EventType
        {
            None = 0,
            WindowCreated, WindowClosed, WindowResized, WindowFocused, WindowUnfocused, WindowMoved,
            ConsoleLogged,
            KeyPressed, KeyReleased,
            MouseButtonPressed, MouseButtonReleased, MouseMoved, MouseScrolled,
        }

        public abstract EventType GetEventType();
        public abstract override string ToString();

        public static EventType GetStaticType()
        {
            return EventType.KeyPressed;
        }
    }

    public class EventDispatcher
    {
        private Event Event;
        public EventDispatcher(Event e)
        {
            Event = e;
        }

        public bool Dispatch<TEvent>(Event e, Func<TEvent, bool> func) where TEvent : Event
        {
            if (!(e is TEvent) || Event.GetEventType() != e.GetEventType()) return false;
            Event.Handled = func((TEvent)e);
            return true;
        }
    }
}
